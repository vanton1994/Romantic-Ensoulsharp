using System;
using System.Collections.Generic;
using System.Linq;


namespace OneKeyToWin_AIO_Sebby.Champions
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using SebbyLib;
    using SharpDX;
    class Anivia : Base
    {
        private float RCastTime = 0;
        private int Rwidth = 400;
        private static GameObject QMissile, RMissile;
        private static int QMissileCreateTick;
        private static SharpDX.Vector3 StartPosition, EndPosition;

        private readonly MenuBool onlyRdy = new MenuBool("onlyRdy", "Draw only ready spells");
        private readonly MenuBool qRange = new MenuBool("qRange", "Q range", false);
        private readonly MenuBool wRange = new MenuBool("wRange", "W range", false);
        private readonly MenuBool eRange = new MenuBool("eRange", "E range", false);
        private readonly MenuBool rRange = new MenuBool("rRange", "R range", false);

        private readonly MenuBool autoQ = new MenuBool("autoQ", "Auto Q", true);
        private readonly MenuBool AGCQ = new MenuBool("AGCQ", "Q gapcloser", true);
        private readonly MenuBool harassQ = new MenuBool("harassQ", "Harass Q", true);

        private readonly MenuBool autoW = new MenuBool("autoW", "Auto W", true);
        private readonly MenuBool AGCW = new MenuBool("AGCW", "AntiGapcloser W", true);
        private readonly MenuBool inter = new MenuBool("inter", "OnPossibleToInterrupt W", true);

        private readonly MenuBool autoE = new MenuBool("autoE", "Auto E", true);

        private readonly MenuBool autoR = new MenuBool("autoR", "Auto R", true);

        private readonly MenuBool farmR = new MenuBool("farmR", "Lane clear R", true);

        private readonly MenuBool jungleE = new MenuBool("jungleE", "Jungle clear E", true);
        private readonly MenuBool jungleQ = new MenuBool("jungleQ", "Jungle clear Q", true);
        private readonly MenuBool jungleW = new MenuBool("jungleW", "Jungle clear W", true);
        private readonly MenuBool jungleR = new MenuBool("jungleR", "Jungle clear R", true);


        private readonly MenuBool AACombo = new MenuBool("AACombo", "Disable AA if can use E", true);

        public Anivia()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 685);

            Q.SetSkillshot(0.25f, 110f, 870f, false, SpellType.Line);
            W.SetSkillshot(0.6f, 1f, float.MaxValue, false, SpellType.Line);
            R.SetSkillshot(0.5f, 200f, float.MaxValue, false, SpellType.Circle);

            Local.Add(new Menu("draw", "Draw")
            {
                onlyRdy,
                qRange,
                wRange,
                eRange,
                rRange

            });

            Local.Add(new Menu("qConfig", "Q Config")
            {
                autoQ,
                AGCQ,
                harassQ
            });


            Local.Add(new Menu("wConfig", "W Config")
            {
                autoW,
                AGCW,
                inter
            });


            Local.Add(new Menu("eConfig", "E Config")
            {
                autoE
            });

            Local.Add(new Menu("rConfig", "R Config")
            {
                autoR
            });

            Local.Add(new Menu("aaCombo", "Block AA")
            {
                AACombo
            });

            FarmMenu.Add(farmR);
            FarmMenu.Add(jungleQ);
            FarmMenu.Add(jungleE);
            FarmMenu.Add(jungleW);
            FarmMenu.Add(jungleR);

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Interrupter.OnInterrupterSpell += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (inter.Enabled && W.IsReady() && sender.IsValidTarget(W.Range))
                W.Cast(sender);
        }

        private void AntiGapcloser_OnEnemyGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (!OktwCommon.CheckGapcloser(sender, args))
            {
                return;
            }

            if (Q.IsReady() && AGCQ.Enabled)
            {
                if (sender.IsValidTarget(300))
                {
                    Q.Cast();
                }
            }
            else if (W.IsReady() && AGCW.Enabled)
            {
                if (sender.IsValidTarget(W.Range))
                {
                    W.Cast(ObjectManager.Player.Position.Extend(sender.Position, 50));
                }
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "FlashFrostSpell")
                {
                    var missile = obj as MissileClient;

                    if (missile != null)
                    {
                        QMissile = obj;
                        QMissileCreateTick = Variables.GameTimeTickCount + 25;
                        StartPosition = missile.Position;
                        EndPosition = missile.EndPosition;
                    }
                }
                if (obj.Name.StartsWith("Anivia") && obj.Name.EndsWith("R_indicator_ring"))
                {
                    RMissile = obj;
                }
            }
        }

        private SharpDX.Vector3 MissilePosition()
        {
            if (QMissile != null)
            {
                return StartPosition.Extend(EndPosition, (Variables.GameTimeTickCount - QMissileCreateTick) / 1000f * Q.Speed);
            }

            return SharpDX.Vector3.Zero;
        }

        private void Obj_AI_Base_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "FlashFrostSpell")
                {
                    QMissile = null;
                    QMissileCreateTick = 0;
                    StartPosition = SharpDX.Vector3.Zero;
                    EndPosition = SharpDX.Vector3.Zero;
                }

                if (obj.Name.StartsWith("Anivia") && obj.Name.EndsWith("R_indicator_ring"))
                    RMissile = null;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.Combo && AACombo.Enabled)
            {
                if (!E.IsReady())
                    Orbwalker.MoveEnabled = true;

                else
                    Orbwalker.MoveEnabled = false;
            }
            else
                Orbwalker.MoveEnabled = true;

            if (Q.IsReady()  && QMissile != null && MissilePosition().CountEnemyHeroesInRange(230) > 0)
                Q.Cast();
            
            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && R.IsReady() && autoR.Enabled)
                LogicR();

            if (Program.LagFree(2) && W.IsReady() && autoW.Enabled)
                LogicW();

            if (Program.LagFree(3) && Q.IsReady() && autoQ.Enabled)
                LogicQ();

            if (Program.LagFree(4) )
            {
                if(E.IsReady() && autoE.Enabled)
                    LogicE();

                Jungle();
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > EMANA + QMANA - 10)
                    Program.CastSpell(Q, t);
                else if (Program.Harass && harassQ.Enabled &&  Player.Mana > RMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarass())
                {
                    Program.CastSpell(Q, t);
                }
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t,Q);
                    var eDmg = E.GetDamage(t);
                    if (qDmg > t.Health)
                        Program.CastSpell(Q, t);
                    else if (qDmg + eDmg > t.Health && Player.Mana > QMANA + WMANA)
                        Program.CastSpell(Q, t);
                }
                if (!Program.None && Player.Mana > RMANA + EMANA)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    {
                        Q.Cast(enemy, true);
                    }
                }
            }
        }

        private void LogicW()
        {
            if (Program.Combo && Player.Mana > RMANA + EMANA + WMANA)
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (t.IsValidTarget(W.Range) && W.GetPrediction(t).CastPosition.Distance(t.Position) > 100)
                {
                    if (Player.Position.Distance(t.Position) > Player.Position.Distance(t.Position))
                    {
                        if (t.Position.Distance(Player.Position) < t.Position.Distance(Player.Position))
                            Program.CastSpell(W, t);
                    }
                    else
                    {
                        if (t.Position.Distance(Player.Position) > t.Position.Distance(Player.Position) && t.Distance(Player) < R.Range)
                            Program.CastSpell(W, t);
                    }
                }
            }
        }
        
        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qCd = Q.Instance.CooldownExpires - Game.Time;
                var rCd = R.Instance.CooldownExpires - Game.Time;
                if (Player.Level < 7)
                    rCd = 10;
                //debug("Q " + qCd + "R " + rCd + "E now " + E.Instance.Cooldown);
                var eDmg = OktwCommon.GetKsDamage(t, E);

                if (eDmg > t.Health)
                    E.Cast(t, true);
                
                if (t.HasBuff("chilled") || qCd > E.Instance.Cooldown - 1 && rCd > E.Instance.Cooldown - 1)
                {
                    if (eDmg * 3 > t.Health)
                        E.Cast(t, true);
                    else if (Program.Combo && (t.HasBuff("chilled") || Player.Mana > RMANA + EMANA))
                    {
                        E.Cast(t, true);
                    }
                    else if ( Program.Harass && Player.Mana > RMANA + EMANA + QMANA + WMANA && !Player.IsUnderEnemyTurret() && QMissile == null)
                    {
                        E.Cast(t, true);
                    }
                }
                else if (Program.Combo && R.IsReady() && Player.Mana > RMANA + EMANA && QMissile == null )
                {
                    R.Cast(t, true, true);
                }
            }
        }



        private void LogicR()
        {
            if (RMissile == null)
            {
                var t = TargetSelector.GetTarget(R.Range + 400, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (R.GetDamage(t) > t.Health)
                        R.Cast(t, true, true);
                    else if (Player.Mana > RMANA + EMANA && E.GetDamage(t) * 2 + R.GetDamage(t) > t.Health)
                        R.Cast(t, true, true);
                    if (Player.Mana > RMANA + EMANA + QMANA + WMANA && Program.Combo)
                        R.Cast(t, true, true);
                }
                if (FarmSpells && farmR.Enabled)
                {
                    var allMinions = GameObjects.GetMinions(Player.Position, R.Range);
                    var farmPos = R.GetCircularFarmLocation(allMinions, Rwidth);
                    if (farmPos.MinionsHit >= 0)
                        R.Cast(farmPos.Position);
                }
            }
            else
            {
                if (FarmSpells && farmR.Enabled)
                {
                    var allMinions = GameObjects.GetMinions(RMissile.Position, Rwidth);
                    var mobs = GameObjects.GetMinions(RMissile.Position, Rwidth);
                    if (mobs.Count > 0)
                    {
                        if (jungleR.Enabled)
                        {
                            R.Cast();
                        }
                    }
                    else if (allMinions.Count > 0)
                    {
                        if (allMinions.Count < 2 )
                            R.Cast();
                    }
                    else
                        R.Cast();

                }
                else if (!Program.None &&(RMissile.Position.CountEnemyHeroesInRange(470) == 0 || Player.Mana < EMANA + QMANA))
                {
                    R.Cast();
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = GameObjects.GetMinions(Player.Position, E.Range);
                if (mobs.Count > 0)
                {
                    var mob = mobs.First();
                    if (Q.IsReady() && jungleQ.Enabled)
                    {
                        if (QMissile != null)
                        {
                            if (MissilePosition().Distance(mob.Position) < 230)
                                Q.Cast();
                        }
                        
                        return;
                    }
                    if (R.IsReady() && jungleR.Enabled && RMissile == null)
                    {
                        R.Cast(mob.Position);
                        return;
                    }
                    if (E.IsReady() && jungleE.Enabled && mob.HasBuff("chilled"))
                    {
                        E.Cast(mob);
                        return;
                    }
                    if (W.IsReady() && jungleW.Enabled)
                    {
                        W.Cast(mob.Position.Extend(Player.Position , 100));
                        return;
                    }
                }
            }
        }

        private void SetMana()
        {
            if ((manaDisable.Enabled && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.ManaCost;
            WMANA = W.Instance.ManaCost;
            EMANA = E.Instance.ManaCost;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.ManaCost;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (qRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (Q.IsReady())
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
                }
                else
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
            }
            if (wRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (W.IsReady())
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1);
                }
                else
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1);
            }
            if (eRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (E.IsReady())
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1);
                }
                else
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1);
            }
            if (rRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (R.IsReady())
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1);
                }
                else
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1);
            }
        }
    }
}
