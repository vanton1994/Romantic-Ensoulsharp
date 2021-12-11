using System.Media;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using PRADA_Vayne.MyLogic.Q;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnGapcloser(AIBaseClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (ObjectManager.Player == null || ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
            {
                return;
            }

            if (sender == null || !sender.IsValidTarget())
            {
                return;
            }

            // ignore two champion
            if (sender.CharacterName == "Caitlyn" || sender.CharacterName == "Vayne")
            {
                return;
            }

            if (args.EndPosition.DistanceToPlayer() > 300)
            {
                return;
            }

            var qCasted = false;

            if (Program.GapcloserMenu["QAntiGapcloser"].GetValue<MenuBool>().Enabled && Program.Q.IsReady())
            {
                if (Program.Q.Cast(ObjectManager.Player.Position.Extend(sender.Position, -300f)))
                {
                    qCasted = true;
                }
            }

            if (Program.GapcloserMenu["EAntiGapcloser"].GetValue<MenuBool>().Enabled && Program.E.IsReady())
            {
                if (qCasted)
                {
                    DelayAction.Add(500, () => Program.E.CastOnUnit(sender));
                }
                else
                {
                    Program.E.CastOnUnit(sender);
                }
            }
        }
    }
}