using EnsoulSharp;
using SharpDX;
using System.Linq;
using EnsoulSharp.SDK.MenuUI;

namespace PRADA_Vayne.MyLogic.Q
{
    public static class Tumble
    {
        public static Vector3 TumbleOrderPos = Vector3.Zero;

        public static void Cast(Vector3 position)
        {
            if (!Program.ComboMenu["QCombo"].GetValue<MenuBool>().Enabled) return;
            TumbleOrderPos = position;
            if (position != Vector3.Zero) Program.Q.Cast(TumbleOrderPos);
            if (position == Vector3.Zero && ObjectManager.Player.Buffs.Any(b => b.Name.ToLower().Contains("vayneinquisition"))) Program.Q.Cast(Game.CursorPos);
        }
    }
}