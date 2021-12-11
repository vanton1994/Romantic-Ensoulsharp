using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using PRADA_Vayne.MyLogic.Q;
using PRADA_Vayne.MyUtils;
using PRADA_Vayne.Utils;

namespace PRADA_Vayne.MyLogic.Others
{
    public static partial class Events
    {
        public static void OnProcessSpellcast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            #region ward brush after condemn

            if (sender.IsMe && args.SData.Name.ToLower().Contains("condemn") && args.Target is AIHeroClient)
            {
                var target = args.Target as AIHeroClient;
                if (Program.ComboMenu["EQ"].GetValue<MenuBool>().Enabled && target.IsVisible && !target.HasBuffOfType(BuffType.Stun) && Program.Q.IsReady()) //#TODO: fix
                {
                    var tumblePos = target.GetTumblePos();
                    Tumble.Cast(tumblePos);
                }

                if (NavMesh.IsWallOfType(args.End, CollisionFlags.Grass, 100))
                {
                    var blueTrinket = ItemId.Farsight_Alteration;
                    if (ObjectManager.Player.HasItem(ItemId.Farsight_Alteration) &&
                        ObjectManager.Player.CanUseItem((int)ItemId.Farsight_Alteration))
                    {
                        blueTrinket = ItemId.Farsight_Alteration;
                    }
                    
                    var yellowTrinket = ItemId.Stealth_Ward;
                    if (ObjectManager.Player.HasItem(ItemId.Control_Ward) &&
                        ObjectManager.Player.CanUseItem((int)ItemId.Control_Ward))
                    {
                        yellowTrinket = ItemId.Control_Ward;
                    }

                    if (ObjectManager.Player.CanUseItem((int) blueTrinket))
                    {
                        ObjectManager.Player.UseItem((int) blueTrinket, args.End);
                    }
                    if (ObjectManager.Player.CanUseItem((int) yellowTrinket))
                    {
                        ObjectManager.Player.UseItem((int) yellowTrinket, args.End);
                    }
                }
            }

            #endregion ward brush after condemn

            #region Anti-Stealth

            if (args.SData.Name.ToLower().Contains("talonshadow")) //#TODO get the actual buff name
            {
                if (ObjectManager.Player.HasItem(ItemId.Oracle_Lens) &&
                    ObjectManager.Player.CanUseItem((int) ItemId.Oracle_Lens))
                {
                    ObjectManager.Player.UseItem((int) ItemId.Oracle_Lens, ObjectManager.Player.Position);
                }
                else if (ObjectManager.Player.HasItem(ItemId.Control_Ward))
                {
                    ObjectManager.Player.UseItem((int) ItemId.Control_Ward, ObjectManager.Player.Position);
                }
            }

            #endregion Anti-Stealth

            /*if (MyWizard.ShouldSaveCondemn()) return;
            if (sender.Distance(Heroes.Player) > 1500 || !args.Target.IsMe || args.SData == null)
                return;
            //how to milk alistar/thresh/everytoplaner
            var spellData = SpellDb.GetByName(args.SData.Name);
            if (spellData != null && !Heroes.Player.IsUnderEnemyTurret() && !Lists.UselessChamps.Contains(sender.CharacterName))
                if (spellData.CcType == CcType.Knockup || spellData.CcType == CcType.Stun || spellData.CcType == CcType.Knockback || spellData.CcType == CcType.Suppression)
                    Program.E.Cast(sender);*/
        }
    }
}