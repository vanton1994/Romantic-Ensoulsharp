using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System.Linq;
using EnsoulSharp.SDK.MenuUI;

namespace PRADA_Vayne.MyLogic.R
{
    public static partial class Events
    {
        public static void BeforeAttack(object sender, BeforeAttackEventArgs args)
        {
            if (Program.Q.IsReady() || Program.ComboMenu["QCombo"].GetValue<MenuBool>().Enabled)
                if (ObjectManager.Player.HasBuff("vaynetumblefade") && Program.EscapeMenu["QUlt"].GetValue<MenuBool>().Enabled && Heroes.EnemyHeroes.Any(h => h.IsMelee && h.Distance(Heroes.Player) < h.AttackRange + h.BoundingRadius))
                    args.Process = false;
        }
    }
}