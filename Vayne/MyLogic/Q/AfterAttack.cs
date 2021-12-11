using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System;
using System.Linq;
using EnsoulSharp.SDK.MenuUI;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void AfterAttack(object sender, AfterAttackEventArgs args)
        {
            if (!Program.Q.IsReady()) return;
            if (args.Target is AIHeroClient && (Orbwalker.ActiveMode == OrbwalkerMode.Combo || !Program.ComboMenu["OnlyQinCombo"].GetValue<MenuBool>().Enabled))
            {
                var tg = args.Target as AIHeroClient;
                if (tg == null) return;
                var mode = Program.ComboMenu["QMode"].GetValue<MenuList>().SelectedValue;
                var tumblePosition = Game.CursorPos;
                switch (mode)
                {
                    case "PRADA":
                        tumblePosition = tg.GetTumblePos();
                        break;

                    default:
                        tumblePosition = Game.CursorPos;
                        break;
                }

                Tumble.Cast(tumblePosition);
            }

            var m = args.Target as AIMinionClient;
            if (m != null && Program.LaneClearMenu["QLastHit"].GetValue<MenuBool>().Enabled && ObjectManager.Player.ManaPercent >= Program.LaneClearMenu["QLastHitMana"].GetValue<MenuSlider>().Value && Orbwalker.ActiveMode == OrbwalkerMode.LastHit || Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                var dashPosition = Game.CursorPos;
                var mode = Program.ComboMenu["QMode"].GetValue<MenuList>().SelectedValue;
                switch (mode)
                {
                    case "PRADA":
                        dashPosition = m.GetTumblePos();
                        break;

                    default:
                        dashPosition = Game.CursorPos;
                        break;
                }

                if (m.Team == GameObjectTeam.Neutral) Program.Q.Cast(dashPosition);
                foreach (var minion in ObjectManager.Get<AIMinionClient>().Where(minion => m.NetworkId != minion.NetworkId && minion.IsEnemy && minion.IsValidTarget(615)))
                {
                    if (minion == null)
                        break;
                    var time = (int)(ObjectManager.Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 * (int) Math.Max(0, ObjectManager.Player.Distance(minion) - ObjectManager.Player.BoundingRadius) / (int)ObjectManager.Player.BasicAttack.MissileSpeed;
                    var predHealth = HealthPrediction.GetPrediction(minion, time);
                    if (predHealth < ObjectManager.Player.GetAutoAttackDamage(minion) + Program.Q.GetDamage(minion) && predHealth > 0)
                        Program.Q.Cast(dashPosition);
                }
            }
        }
    }
}