using System;
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
        public static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.IsValid)
            {
                return;
            }

            if (sender.DistanceToPlayer() < 300)
            {

            }

            if (sender.Name.ToLower().Contains("ssssssss"))
            {
                Program.VeigarHouse = sender as EffectEmitter;
                return;
            }

            if (sender.Name.ToLower().Contains("anivia_base_w_tar"))
            {
                Program.AniviaWall = sender as EffectEmitter;
                return;
            }

            if (sender.Name.ToLower().Contains("trundlewall"))
            {
                Program.TrundleWall = sender;
            }
        }
    }
}