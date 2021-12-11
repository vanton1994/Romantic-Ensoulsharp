using EnsoulSharp;
using System;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;

namespace PRADA_Vayne.MyLogic.Others
{
    public static class SkinHack
    {
        public static void Load()
        {
            DelayAction.Add(250, RefreshSkin);
            Game.OnUpdate += OnUpdate;
            Game.OnNotify += OnNotify;
        }

        private static void OnNotify(GameNotifyEventArgs args)
        {
        }

        public static void OnUpdate(EventArgs args)
        {
        }

        public static void RefreshSkin()
        {
            if (Program.SkinhackMenu["shkenabled"].GetValue<MenuBool>().Enabled)
            {
            }
        }
    }
}