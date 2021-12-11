using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System.Linq;
using EnsoulSharp.SDK.MenuUI;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void BeforeAttack(object sender, BeforeAttackEventArgs args)
        {
            if (Program.Q.IsReady() && Program.ComboMenu["QCombo"].GetValue<MenuBool>().Enabled)
                if (args.Target is AIHeroClient)
                {
                    var target = args.Target as AIHeroClient;
                    if (Program.ComboMenu["RCombo"].GetValue<MenuBool>().Enabled && Program.R.IsReady() && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                        if (!target.IsUnderEnemyTurret())
                            Program.R.Cast();
                    if (target.IsMelee && target.IsFacing(Heroes.Player))
                        if (target.Distance(Heroes.Player.Position) < 325)
                        {
                            var tumblePosition = target.GetTumblePos();
                            args.Process = false;
                            Tumble.Cast(tumblePosition);
                        }

                    var closestJ4Wall = ObjectManager.Get<AIMinionClient>().FirstOrDefault(m => m.CharacterName == "jarvanivwall" && ObjectManager.Player.Position.Distance(m.Position) < 100);
                    if (closestJ4Wall != null)
                    {
                        args.Process = false;
                        Program.Q.Cast(ObjectManager.Player.Position.Extend(closestJ4Wall.Position, 300));
                    }
                }
        }
    }
}