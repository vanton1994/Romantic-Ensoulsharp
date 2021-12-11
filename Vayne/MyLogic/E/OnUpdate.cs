using EnsoulSharp;
using PRADA_Vayne.MyUtils;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using Microsoft.Win32.SafeHandles;
using SharpDX;
using SharpDX.Mathematics;
using Geometry = EnsoulSharp.SDK.Geometry;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnUpdate(EventArgs args)
        {
            if (Program.E.IsReady())
            {
                if (Program.AniviaWall != null && Program.AniviaWall.IsValid)
                {
                    var wallLength = 400 + 100 * GameObjects.EnemyHeroes
                        .FirstOrDefault(x => x.IsValidTarget() && x.CharacterName == "Anivia").GetSpell(SpellSlot.W)
                        .Level - 1;

                    var orientitaion = Program.AniviaWall.Orientation;
                    var rectAngle = new Geometry.Rectangle(Program.AniviaWall.Position + orientitaion.Left,
                        Program.AniviaWall.Position + orientitaion.Right, wallLength);

                    var startPosition = ObjectManager.Player.Position;
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Program.E.Range)))
                    {
                        var rectAngleInside = new Geometry.Rectangle(startPosition,
                            startPosition + (target.Position - startPosition) * 425, target.BoundingRadius);
                        var castOn = false;

                        foreach (var point in rectAngle.Points)
                        {
                            if (rectAngleInside.IsInside(point))
                            {
                                castOn = true;
                            }
                        }

                        if (castOn)
                        {
                            Program.E.Cast(target);
                        }
                    }
                }

                if (Program.VeigarHouse != null && Program.VeigarHouse.IsValid)
                {
                    var circle = new Geometry.Circle(Program.VeigarHouse.Position, 375);

                    var startPosition = ObjectManager.Player.Position;
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Program.E.Range)))
                    {
                        var rectAngleInside = new Geometry.Rectangle(startPosition,
                            startPosition + (target.Position - startPosition) * 425, target.BoundingRadius);

                        var cast = false;
                        foreach (var point in circle.Points)
                        {
                            if (rectAngleInside.IsInside(point))
                            {
                                cast = true;
                            }
                        }

                        if (cast)
                        {
                            Program.E.Cast(target);
                        }
                    }
                }
                
                if (Program.ComboMenu["ManualE"].GetValue<MenuKeyBind>().Active)
                    foreach (var hero in Heroes.EnemyHeroes.Where(h => h.DistanceToPlayer() < 550))
                        if (hero != null)
                            for (var i = 40; i < 425; i += 125)
                            {
                                var flags = NavMesh.GetCollisionFlags(hero.Position.ToVector2().Extend(Heroes.Player.Position.ToVector2(), -i).ToVector3());
                                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                                {
                                    Program.E.Cast(hero);
                                    return;
                                }
                            }

                if (Program.ComboMenu["ECombo"].GetValue<MenuBool>().Enabled)
                    foreach (var enemy in Heroes.EnemyHeroes.Where(e => e.IsValidTarget(550)))
                        if (enemy != null && enemy.IsCondemnable())
                            Program.E.Cast(enemy);
                var kindredUltedDyingTarget = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.IsValidTarget(550) && h.HasBuff("KindredRNoDeathBuff") && h.Health < ObjectManager.Player.GetAutoAttackDamage(h));
                if (kindredUltedDyingTarget != null) Program.E.Cast(kindredUltedDyingTarget);
            }
        }
    }
}