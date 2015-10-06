using System;
using System.Threading;
using GuildWarsInterface;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;

namespace Entice
{
        internal static class Movement
        {
                private static readonly Timer _timer = new Timer(Tick);
                private static bool _fire;
                private static bool _idle = true;
                private const int CHECK_INTERVAL = 10;
                private const int SEND_INTERVAL = 200;

                private static Position _position;
                private static Position _goal;
                private static MovementType _type;
                private static float _speedModifier;

                private static void Tick(object state)
                {
                        lock (_timer)
                        {
                                _idle = !_fire;

                                if (_fire && Game.Player.Character.Transformation.Goal != null)
                                {
                                        if (_position != null && _goal != null)
                                        {
                                                Networking.Channels.Movement.Update(_position, _goal, _speedModifier, _type);
                                        }

                                        _fire = false;
                                }
                        }
                }

                public static void Task()
                {
                        while (true)
                        {
                                if (Game.State == GameState.Playing && Game.Player.Character.Transformation.Goal != null)
                                {
                                        CreateMovementState();

                                        while (Game.State == GameState.Playing)
                                        {
                                                bool immediately;
                                                if (Changed(out immediately))
                                                {
                                                         CreateMovementState();

                                                         lock (_timer)
                                                         {
                                                                 if (_idle || immediately) _timer.Change(0, SEND_INTERVAL);
                                                                 _fire = true;
                                                         }
                                                }

                                                Thread.Sleep(CHECK_INTERVAL);
                                        }  
                                }
                        }
                }

                private static void CreateMovementState()
                {
                        lock (_timer)
                        {
                                var c = Game.Player.Character;
                                var m = c.AgentClientMemory;
                                var t = c.Transformation;

                                _position = new Position(m.X, m.Y, m.Plane);
                                _goal = t.Goal;
                                _speedModifier = Game.Player.SpeedModifier;
                                _type = t.MovementType;
                        }
                }

                private static bool Changed(out bool immediately)
                {
                        var c = Game.Player.Character;
                        var m = c.AgentClientMemory;
                        var t = c.Transformation;

                        if (_position.Plane != m.Plane || _type != t.MovementType)
                        {
                                immediately = true;
                                return true;
                        }

                        immediately = false;
                        return _position.X != m.X || _position.Y != m.Y || _goal != t.Goal;
                }
        }
}