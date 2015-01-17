using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;

namespace Launcher
{
        public static class ProcessInteraction
        {
                #region pinvoke declarations

                [DllImport("kernel32.dll")]
                private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

                [DllImport("kernel32.dll")]
                private static extern uint ResumeThread(IntPtr hThread);

                [DllImport("kernel32.dll")]
                private static extern IntPtr GetModuleHandle(string lpModuleName);

                [DllImport("kernel32.dll")]
                private static extern int CloseHandle(IntPtr hObject);

                [DllImport("kernel32.dll")]
                private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

                [DllImport("kernel32.dll")]
                private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

                [DllImport("kernel32.dll")]
                private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

                [DllImport("kernel32.dll")]
                private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten);

                [DllImport("kernel32.dll", EntryPoint = "LoadLibraryExW", SetLastError = true)]
                private static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, IntPtr hFile, int dwFlags);

                [DllImport("kernel32", EntryPoint = "WaitForSingleObject")]
                private static extern uint WaitForSingleObject(IntPtr hObject, uint dwMilliseconds);

                [DllImport("kernel32.dll")]
                private static extern uint SuspendThread(IntPtr hThread);

                [DllImport("kernel32.dll")]
                private static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);

                #endregion

                /// <summary>
                /// runs an executable, injects the dll and then executes the specified function. 
                /// the created process is resumed after the call to the function completed.
                /// it is assumed that the called function does not take any arguments.
                /// </summary>
                public static bool Run(string executable, string arguments, string dll, string function)
                {
                        var process = Process.Start(executable, arguments);
                        if (!Inject(process, dll)) return false;

                        Thread.Sleep(500);
                        SuspendProcess(process);

                        var success = CallExport(process, dll, function);

                        ResumeProcess(process);

                        return success;
                }

                /// <summary>
                /// injects the dll into the process and then executes the specified function.
                /// </summary>
                public static bool Inject(Process process, string dll, string function)
                {
                        if (!Inject(process, dll)) return false;

                        return CallExport(process, dll, function);
                }

                /// <summary>
                /// injects the dll into the process.
                /// </summary>
                public static bool Inject(Process process, string dll)
                {
                        IntPtr loadLibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                        if (loadLibraryAddress == IntPtr.Zero) return false;

                        IntPtr argument = VirtualAllocEx(process.Handle, IntPtr.Zero, dll.Length, (0x1000 | 0x2000), 0x40);
                        if (argument == IntPtr.Zero) return false;

                        byte[] argumentValue = Encoding.ASCII.GetBytes(dll);
                        if (WriteProcessMemory(process.Handle, argument, argumentValue, argumentValue.Length, 0) == 0) return false;

                        IntPtr thread = CreateRemoteThread(process.Handle, IntPtr.Zero, IntPtr.Zero, loadLibraryAddress, argument, 0, IntPtr.Zero);
                        if (thread == IntPtr.Zero) return false;

                        WaitForSingleObject(thread, 0xFFFFFFFF);
                        VirtualFreeEx(thread, argument, argumentValue.Length, 0x10000);
                        CloseHandle(thread);

                        return true;
                }

                /// <summary>
                /// calls an exported function.
                /// it is assumed that the called function does not take any arguments.
                /// </summary>
                public static bool CallExport(Process process, string dll, string function)
                {
                        IntPtr module = LoadLibraryEx(dll, IntPtr.Zero, 1);
                        if (module == IntPtr.Zero) return false;

                        IntPtr functionAddress = GetProcAddress(module, function);
                        if (functionAddress == IntPtr.Zero) return false;
                        functionAddress = GetModule(process, dll).BaseAddress + (int)functionAddress - (int)module;

                        IntPtr thread = CreateRemoteThread(process.Handle, IntPtr.Zero, IntPtr.Zero, functionAddress, IntPtr.Zero, 0, IntPtr.Zero);
                        if (thread == IntPtr.Zero) return false;

                        WaitForSingleObject(thread, 0xFFFFFFFF);
                        CloseHandle(thread);

                        return true;
                }

                private static ProcessModule GetModule(Process process, string dll)
                {
                        return process.Modules.Cast<ProcessModule>().FirstOrDefault(module => module.FileName.Equals(dll));
                }

                private static void SuspendProcess(Process process)
                {
                        foreach (IntPtr thread in process.Threads.Cast<ProcessThread>().Select(t => OpenThread(2, false, t.Id)).Where(t => t != IntPtr.Zero))
                        {
                                SuspendThread(thread);
                                CloseHandle(thread);
                        }
                }

                private static void ResumeProcess(Process process)
                {
                        foreach (IntPtr thread in process.Threads.Cast<ProcessThread>().Select(t => OpenThread(2, false, t.Id)).Where(t => t != IntPtr.Zero))
                        {
                                uint suspendCount;
                                do
                                {
                                        suspendCount = ResumeThread(thread);
                                } while (suspendCount > 0);

                                CloseHandle(thread);
                        }
                }
        }
}