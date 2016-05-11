using System.IO;
using System.Windows.Forms;

namespace Launcher
{
    internal static class Program
    {
        private static void Main()
        {
            ProcessInteraction.Run(Application.StartupPath + Path.DirectorySeparatorChar + "Gw.exe", "-oldauth", Application.StartupPath + Path.DirectorySeparatorChar + "Entice.dll", "Main");
        }
    }
}