using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using PRADA_Vayne.MyLogic.Q;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyLogic.R
{
    public static partial class Events
    {
        public static void OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (spellbook.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.R && Program.ComboMenu["QR"].GetValue<MenuBool>().Enabled)
                {
                    var target = TargetSelector.GetTarget(300, DamageType.Physical);
                    var tumblePos = target != null ? target.GetTumblePos() : Game.CursorPos;
                    Tumble.Cast(tumblePos);
                }
            }
        }
    }
}