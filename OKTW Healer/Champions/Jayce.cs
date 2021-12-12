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
    class Jayce : Base
    {
        private Spell  Qext, QextCol;
        private float  QMANA2 = 0, WMANA2 = 0, EMANA2 = 0;
        private float Qcd, Wcd, Ecd, Q2cd, W2cd, E2cd;
        private float Qcdt, Wcdt, Ecdt, Q2cdt, W2cdt, E2cdt;
        private Vector3 EcastPos;
        private int Etick = 0;
        public int Muramana = 3042;
        public int Tear = 3070;
        public int Manamune = 3004;

        #region Menu1
        private readonly MenuBool showcd = new MenuBool("showcd", "Show cooldown", true);
        private readonly MenuBool noti = new MenuBool("noti", "Show notification & line", true);
        private readonly MenuBool onlyRdy = new MenuBool("onlyRdy", "Draw only ready spells", true);
        private readonly MenuBool qRange = new MenuBool("qRange", "Q range", true);

        private readonly MenuBool autoQ = new MenuBool("autoQ", "Auto Q range", true);
        private readonly MenuBool autoQm = new MenuBool("autoQm", "Auto Q melee", true);
        private readonly MenuBool QEforce = new MenuBool("QEforce", "force E + Q", true);
        private readonly MenuBool QEsplash = new MenuBool("QEsplash", "Q + E splash minion damage", true);
        private readonly MenuSlider QEsplashAdjust = new MenuSlider("QEsplashAdjust", "Q + E splash minion radius", 150, 250, 50);
        private readonly MenuKeyBind useQE = new MenuKeyBind("useQE", "Semi-manual Q + E near mouse key", Keys.T, KeyBindType.Press, true); //32 == space

        private readonly MenuBool autoW = new MenuBool("autoW", "Auto W range", true);
        private readonly MenuBool autoWm = new MenuBool("autoWm", "Auto W melee", true);
        private readonly MenuBool autoWmove = new MenuBool("autoWmove", "Disable move if W range active", true);

        private readonly MenuBool autoE = new MenuBool("autoE", "Auto E range (Q + E)", true);
        private readonly MenuBool autoEm = new MenuBool("autoEm", "Auto E melee", true);
        private readonly MenuBool autoEks = new MenuBool("autoEks", "E melee ks only", true);

        private readonly MenuBool gapE = new MenuBool("gapE", "Gapcloser R + E", true);
        private readonly MenuBool intE = new MenuBool("intE", "Interrupt spells R + Q + E", true);

        private readonly MenuBool autoR = new MenuBool("autoR", "Auto R range", true);
        private readonly MenuBool autoRm = new MenuBool("autoRm", "Auto R melee", true);

        private readonly MenuBool stack = new MenuBool("stack", "Stack Tear if full mana", true);

        private readonly MenuSlider harassMana = new MenuSlider("harassMana", "Harass Mana", 80, 100, 0);

        private readonly MenuBool farmQ = new MenuBool("farmQ", "Lane clear Q + E range", false);
        private readonly MenuBool farmW = new MenuBool("farmW", "Lane clear W range && mele", false);

        private readonly MenuBool jungleQ = new MenuBool("jungleQ", "Jungle clear Q", true);
        private readonly MenuBool jungleW = new MenuBool("jungleW", "Jungle clear W", true);
        private readonly MenuBool jungleE = new MenuBool("jungleE", "Jungle clear E", true);
        private readonly MenuBool jungleR = new MenuBool("jungleR", "Jungle clear R", true);

        private readonly MenuBool jungleQm = new MenuBool("jungleQm", "Jungle clear Q melee", true);
        private readonly MenuBool jungleWm = new MenuBool("jungleWm", "Jungle clear W melee", true);
        private readonly MenuBool jungleEm = new MenuBool("jungleEm", "Jungle clear E melee", true);
        #endregion
        public Jayce()
        {
            #region SET SKILLS
            Q = new Spell(SpellSlot.Q, 1030);
            Qext = new Spell(SpellSlot.Q, 1650);
            QextCol = new Spell(SpellSlot.Q, 1650);
            Q1 = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            W1 = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 650);
            W1 = new Spell(SpellSlot.E, 240);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 70, 1450, true, SpellType.Line);
            Qext.SetSkillshot(0.30f, 80, 2000, false, SpellType.Line);
            QextCol.SetSkillshot(0.30f, 100, 1600, true, SpellType.Line);
            Q1.SetTargetted(0.25f, float.MaxValue);
            E.SetSkillshot(0.1f, 120, float.MaxValue, false, SpellType.Circle);
            E1.SetTargetted(0.25f, float.MaxValue);
            #endregion

            #region Menu2
            Local.Add(new Menu("draw", "Draw")
            {
                showcd,
                noti,
                onlyRdy,
                qRange

            });

            Local.Add(new Menu("qConfig", "Q Config")
            {
                autoQ,
                autoQm,
                QEforce,
                QEsplash,
                QEsplashAdjust,
                useQE
            });

            Local.Add(new Menu("wConfig", "W Config")
            {
                autoW,
                autoWm,
                autoWmove
            });

            Local.Add(new Menu("eConfig", "E Config")
            {
                autoE,
                autoEm,
                autoEks
            });

            Local.Add(new Menu("stack", "Stack")
            {
                stack
            });


            Local.Add(new Menu("harassMana", "Harass Mana")
            {
                harassMana
            });


            FarmMenu.Add(farmQ);
            FarmMenu.Add(farmW);
            FarmMenu.Add(jungleQ);
            FarmMenu.Add(jungleW);
            FarmMenu.Add(jungleE);
            FarmMenu.Add(jungleR);
            FarmMenu.Add(jungleQm);
            FarmMenu.Add(jungleWm);
            FarmMenu.Add(jungleEm);
            #endregion

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += OnUpdate;
            Orbwalker.OnBeforeAttack += BeforeAttack;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Interrupter.OnInterrupterSpell += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender != null && sender.IsMe && args.SData.Name.ToLower().Contains("jayceshockblast"))
            {
                if (Range && E.IsReady() && autoE.Enabled)
                {
                    EcastPos = Player.Position.Extend(args.End, 130 + (Game.Ping / 2));
                    Etick = Variables.GameTimeTickCount;
                    E.Cast(EcastPos);

                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (!OktwCommon.CheckGapcloser(sender, args))
            {
                return;
            }
            if (gapE.Enabled || E2cd > 0.1)
                return;

            if(Range && !R.IsReady())
                return;


            if (sender.IsValidTarget(400))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                    E.Cast(sender);
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (intE.Enabled || E2cd > 0.1)
                return;

            if (Range && !R.IsReady())
                return;

            if (sender.IsValidTarget(300))
            {
                if (Range)
                {
                    R.Cast();
                }
                else 
                    E.Cast(sender);

            }
            else if (Q2cd < 0.2 && sender.IsValidTarget(Q1.Range))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                {
                    Q.Cast(sender);
                    if(sender.IsValidTarget(E1.Range))
                        E.Cast(sender);
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                if (W.IsReady() && !Range && Player.Mana > 80)
                    W.Cast();
                if (E.IsReady() && Range && QEforce.Enabled)
                    E.Cast(Player.Position.Extend(args.EndPosition, 120));
            }
        }


        private void BeforeAttack(object sender, BeforeAttackEventArgs args)
        {
            if (W.IsReady() && autoW.Enabled && Range && args.Target is AIBaseClient)
            {
                if(Program.Combo)
                    W.Cast();
                else if (args.Target.Position.Distance(Player.Position)< 500)
                    W.Cast();
            }
        }
        private static AttackableUnit OrbTarget = null;
        private void OnUpdate(EventArgs args)
        {
            OrbTarget = Orbwalker.GetTarget();
            if (Range && E.IsReady() && Variables.GameTimeTickCount - Etick < 250 + Game.Ping)
            {
                E.Cast(EcastPos);
            }


            if (Range)
            {
                
                if (autoWmove.Enabled && OrbTarget != null && Player.HasBuff("jaycehyperchargevfx"))
                    Orbwalker.MoveEnabled = false;
                else
                    Orbwalker.MoveEnabled = true;

                if (Program.LagFree(1) && Q.IsReady() && autoQ.Enabled)
                    LogicQ();

                if (Program.LagFree(2) && W.IsReady() && autoW.Enabled)
                    LogicW();
            }
            else
            {
                Orbwalker.MoveEnabled = true;
                if (Program.LagFree(1) && E1.IsReady() && autoEm.Enabled)
                    LogicE2();

                if (Program.LagFree(2) && Q1.IsReady() && autoQm.Enabled)
                    LogicQ2();
                if (Program.LagFree(3) && W1.IsReady() && autoWm.Enabled)
                    LogicW2();
            }

            if (Program.LagFree(4))
            {
                if (Program.None && stack.Enabled  && !Player.HasBuff("Recall") && Player.Mana > Player.MaxMana * 0.90)
                {
                    if(Variables.GameTimeTickCount - Q.LastCastAttemptTime > 4200 && Variables.GameTimeTickCount - W.LastCastAttemptTime > 4200 && Variables.GameTimeTickCount - E.LastCastAttemptTime > 4200)
                    {
                        if (Range)
                        {
                            if (W.IsReady())
                                W.Cast();
                            else if (E.IsReady() && (Player.InFountain() || Player.IsMoving))
                                E.Cast(Player.Position);
                            else if (Q.IsReady() && !E.IsReady())
                                Q.Cast(Player.Position.Extend(Game.CursorPos, 500));
                            else if (R.IsReady() && Player.InFountain())
                                R.Cast();
                        }
                        else
                        {
                            if (W.IsReady())
                                W.Cast();
                            else if (R.IsReady() && Player.InFountain())
                                R.Cast();
                        }
                    }
                }

                SetValue();
                if(R.IsReady())
                    LogicR();
            }

            Jungle();
            LaneClearLogic();
        }


        private void LogicQ()
        {
            var Qtype = Q;
            if (CanUseQE())
            {
                Qtype = Qext;

                if (useQE.Active)
                {
                    var mouseTarget = GameObjects.EnemyHeroes.Where(enemy =>
                        enemy.IsValidTarget(Qtype.Range)).OrderBy(enemy => enemy.Distance(Game.CursorPos)).FirstOrDefault();

                    if (mouseTarget != null)
                    {
                        CastQ(mouseTarget);
                        return;
                    }
                }
            }

            var t = TargetSelector.GetTarget(Qtype.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, Qtype);

                if (CanUseQE())
                {
                    qDmg = qDmg * 1.4f;
                }

                if (qDmg > t.Health)
                    CastQ(t);
                else if (Program.Combo && Player.Mana > EMANA + QMANA)
                    CastQ(t);
                else if (Program.Harass && Player.ManaPercent > harassMana.Value && OktwCommon.CanHarass())    
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(Qtype.Range)))
                    {
                        CastQ(t);
                    }
                }
                else if ((Program.Combo || Program.Harass) && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(Qtype.Range) && !OktwCommon.CanMove(enemy)))
                        CastQ(t);
                }
            }
        }

        private void LogicW()
        {
            if (Program.Combo && R.IsReady() && Range && OrbTarget.IsValidTarget() && OrbTarget is AIBaseClient)
            {
                W.Cast();
            }
        }

        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E1.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, E1);
                if (qDmg > t.Health)
                    E1.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    E1.Cast(t);
            }
        }

        private void LogicQ2()
        {
            var t = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q1) > t.Health)
                    Q1.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q1.Cast(t);
            }
        }

        private void LogicW2()
        {
            if (Player.CountEnemyHeroesInRange(300) > 0 && Player.Mana > 80)
                W.Cast();
        }

        private void LogicE2()
        {
            var t = TargetSelector.GetTarget(E1.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, E1) > t.Health)
                    E1.Cast(t);
                else if (Program.Combo && autoEks.Enabled && !Player.HasBuff("jaycehyperchargevfx"))
                    E1.Cast(t);
            }
        }

        private void LogicR()
        {
            if (Range && autoRm.Enabled)
            {
                var t = TargetSelector.GetTarget(Q1.Range + 200, DamageType.Physical);
                if (Program.Combo && Qcd > 0.5  && t.IsValidTarget() && ((!W.IsReady() && !t.IsMelee ) || (!W.IsReady() && !Player.HasBuff("jaycehyperchargevfx") && t.IsMelee)))
                {
                    if (Q2cd < 0.5 && t.CountEnemyHeroesInRange(800) < 3)
                        R.Cast();
                    else if (Player.CountEnemyHeroesInRange(300) > 0 && E2cd < 0.5)
                        R.Cast();
                }
            }
            else if (Program.Combo && autoR.Enabled)
            {

                var t = TargetSelector.GetTarget(1400, DamageType.Physical);
                if(t.IsValidTarget()&& !t.IsValidTarget(Q1.Range + 200) && Q.GetDamage(t) * 1.4 > t.Health && Qcd < 0.5 && Ecd < 0.5)
                {
                    R.Cast();
                }

                if (!Q.IsReady() && (!E.IsReady() || autoEks.Enabled))
                {
                    R.Cast();
                }   
            }
        }

        private void LaneClearLogic()
        {
            if (!Program.LaneClear)
                return;

            if (Range && Q.IsReady() && E.IsReady() && FarmSpells && farmQ.Enabled)
            {
                var minionList = GameObjects.GetMinions(Player.Position, Q.Range);
                var farmPosition = QextCol.GetCircularFarmLocation(minionList, 150);

                if (farmPosition.MinionsHit >= 0)
                    Q.Cast(farmPosition.Position);
            }

            if (W.IsReady() && FarmSpells && farmW.Enabled)
            {
                if (Range)
                {
                    var mobs = GameObjects.GetMinions(Player.Position, 550);
                    if (mobs.Count >= 0)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var mobs = GameObjects.GetMinions(Player.Position, 300);
                    if (mobs.Count >= 0)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + WMANA)
            {
                var mobs = GameObjects.GetJungles(700);
                if (mobs.Count > 0)
                {
                    var mob = mobs.First();
                    if (Range)
                    {
                        if (Q.IsReady() && jungleQ.Enabled)
                        {
                            Q.Cast(mob.Position);
                            return;
                        }
                        if (W.IsReady() && jungleE.Enabled)
                        {
                                W.Cast();
                            return;
                        }
                        if (jungleR.Enabled)
                            R.Cast();
                    }
                    else
                    {
                        if (Q1.IsReady() && jungleQm.Enabled && mob.IsValidTarget(Q1.Range))
                        {
                            Q1.Cast(mob);
                            return;
                        }

                        if (W1.IsReady() && jungleWm.Enabled)
                        {
                            if(mob.IsValidTarget(300))
                                W.Cast();
                            return;
                        }
                        if (E1.IsReady() && jungleEm.Enabled && mob.IsValidTarget(E1.Range))
                        {
                            if( mob.IsValidTarget(E1.Range))
                                E1.Cast(mob);
                            return;
                        }
                        if (jungleR.Enabled)
                            R.Cast();
                    }
                }
            }
        }

        private void CastQ(AIBaseClient t)
        {
            if (!CanUseQE())
            {
                Program.CastSpell(Q, t);
                return; 
            }

            bool cast = true;

            if (QEsplash.Enabled)
            {
                var poutput = QextCol.GetPrediction(t);

                foreach (var minion in poutput.CollisionObjects.Where(minion => minion.IsEnemy && minion.Distance(poutput.CastPosition) > QEsplashAdjust.Value))
                {
                    cast = false;
                    break;
                }
            }
            else
                cast = false;

            if (cast)
                Program.CastSpell(Qext, t);
            else
                Program.CastSpell(QextCol, t);

        }

        private float GetComboDMG(AIBaseClient t)
        {
            float comboDMG = 0;

            if (Qcd < 1 && Ecd < 1)
                comboDMG = Q.GetDamage(t) * 1.4f;
            else if (Qcd < 1)
                comboDMG = Q.GetDamage(t);

            if (Q2cd < 1)
                comboDMG = Q.GetDamage(t, 1);

            if (Wcd < 1)
                comboDMG += (float)Player.GetAutoAttackDamage(t) * 3;

            if (W2cd < 1)
                comboDMG += W.GetDamage(t) * 2;

            if (E2cd < 1)
                comboDMG += E.GetDamage(t) * 3;
            return comboDMG;
        }

        private bool CanUseQE()
        {
            if(E.IsReady() && Player.Mana > QMANA + EMANA && autoE.Enabled)
                return true;
            else
                return false;
        }

        private float SetPlus(float valus)
        {
            if (valus < 0)
                return 0;
            else
                return valus;
        }

        private void SetValue()
        {
            if (Range)
            {
                Qcdt = Q.Instance.CooldownExpires;
                Wcdt = W.Instance.CooldownExpires;
                Ecd = E.Instance.CooldownExpires;

                QMANA = Q.Instance.ManaCost;
                WMANA = W.Instance.ManaCost;
                EMANA = E.Instance.ManaCost;
            }
            else
            {
                Q2cdt = Q.Instance.CooldownExpires;
                W2cdt = W.Instance.CooldownExpires;
                E2cdt = E.Instance.CooldownExpires;

                QMANA2 = Q.Instance.ManaCost;
                WMANA2 = W.Instance.ManaCost;
                EMANA2 = E.Instance.ManaCost;
            }

            Qcd = SetPlus(Qcdt - Game.Time);
            Wcd = SetPlus(Wcdt - Game.Time);
            Ecd = SetPlus(Ecdt - Game.Time);
            Q2cd = SetPlus(Q2cdt - Game.Time);
            W2cd = SetPlus(W2cdt - Game.Time);
            E2cd = SetPlus(E2cdt - Game.Time);
        }

        private bool Range { get { return Q.Instance.Name.ToLower() == "jayceshockblast"; } }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (showcd.Enabled)
            {
                string msg = " ";

                if (Range)
                {
                    msg = "Q " + (int)Q2cd + "   W " + (int)W2cd + "   E " + (int)E2cd;
                    Drawing.DrawText(Drawing.Width * 0.5f - 50, Drawing.Height * 0.3f, System.Drawing.Color.Orange, msg);
                }
                else
                {
                    msg = "Q " + (int)Qcd + "   W " + (int)Wcd + "   E " + (int)Ecd;
                    Drawing.DrawText(Drawing.Width * 0.5f - 50, Drawing.Height * 0.3f, System.Drawing.Color.Aqua, msg);
                }
            }
            

            if (qRange.Enabled)
            {
                if (onlyRdy.Enabled)
                {
                    if (Q.IsReady())
                    {
                        if (Range)
                        {
                            if (CanUseQE())
                                Render.Circle.DrawCircle(Player.Position, Qext.Range, System.Drawing.Color.Cyan, 1);
                            else
                                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
                        }
                        else
                            Render.Circle.DrawCircle(Player.Position, Q1.Range, System.Drawing.Color.Orange, 1);
                    }
                }
                else
                {
                    if (Range)
                    {
                        if (CanUseQE())
                            Render.Circle.DrawCircle(Player.Position, Qext.Range, System.Drawing.Color.Cyan, 1);
                        else
                            Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1);
                    }
                    else
                        Render.Circle.DrawCircle(Player.Position, Q1.Range, System.Drawing.Color.Cyan, 1);
                }
            }

            if (noti.Enabled)
            {
                var t = TargetSelector.GetTarget(1600, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var damageCombo = GetComboDMG(t);
                    if (damageCombo > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Combo deal  " + damageCombo + " to " + t.CharacterName);
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }

                }
            }
        }
    }
}
