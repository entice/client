using System.Threading;

namespace Entice.Misc
{
        internal static class MovementLimiter
        {
                private static readonly Timer _timer = new Timer(Tick);
                private static bool _fire;
                private static bool _idle = true;

                private static void Tick(object state)
                {
                        lock (_timer)
                        {
                                _idle = !_fire;

                                if (_fire)
                                {
                                        Networking.Area.Move();
                                        _fire = false;
                                }
                        }
                }

                public static void Trigger()
                {
                        lock (_timer)
                        {
                                if (_idle) _timer.Change(0, 200);

                                _fire = true;
                        }
                }
        }
}