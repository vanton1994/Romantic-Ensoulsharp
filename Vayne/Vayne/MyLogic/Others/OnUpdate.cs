using EnsoulSharp;
using PRADA_Vayne.MyUtils;
using System;
using EnsoulSharp.SDK;

namespace PRADA_Vayne.MyLogic.Others
{
    public static partial class Events
    {
        public static void OnUpdate(EventArgs args)
        {
            if (Heroes.Player.HasBuff("rengarralertsound"))
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
        }
    }
}