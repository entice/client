using System;
using System.Threading;
using Entice.Debugging;
using Entice.Linking;
using GuildWarsInterface;
using GuildWarsInterface.Declarations;
using RGiesecke.DllExport;

namespace Entice
{
        internal class EntryPoint
        {
                [DllExport("Main")]
                internal static void Main()
                {
                        AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debug.Error(args.ExceptionObject.ToString());
                        GuildWarsInterface.Debugging.Debug.ThrowException += exception => Debug.Error(exception.ToString());

                        Game.Initialize();
                        Client.Initialize();

                        new Thread(() =>
                                {
                                        while (true)
                                        {
                                                if (Game.State == GameState.Playing) Game.TimePassed(100);
                                                Thread.Sleep(100);
                                        }
                                }).Start();
                }
        }
}