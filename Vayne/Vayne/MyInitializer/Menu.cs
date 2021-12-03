using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void LoadMenu()
        {
            ConstructMenu();
            FinishMenuInit();
        }

        public static void ConstructMenu()
        {
            Program.MainMenu = new Menu("PRADA Vayne", "pradamenu", true);
            Program.ComboMenu = new Menu("Combo Settings", "combomenu");
            Program.LaneClearMenu =
                new Menu("Laneclear Settings", "laneclearmenu");
            Program.EscapeMenu = new Menu("Escape Settings", "escapemenu");

            Program.DrawingsMenu =
                new Menu("Drawing Settings", "drawingsmenu");
            Program.DrawingsMenu.Add(new MenuBool("streamingmode", "Disable All Drawings", false));
            Program.DrawingsMenu.Add(new MenuBool("drawenemywaypoints", "Draw Enemy Waypoints"));
            Program.SkinhackMenu = new Menu("Skin Hack", "skinhackmenu");
            Program.OrbwalkerMenu =
                new Menu("Orbwalker", "orbwalkermenu");
            Program.ComboMenu.Add(new MenuBool("QCombo", "Auto Tumble"));
            Program.ComboMenu.Add(
                new MenuList("QMode", "Q Mode: ", new[] {"PRADA", "TO MOUSE"}));
            Program.ComboMenu.Add(
                new MenuSlider("QMinDist", "Min dist from enemies", 375, 325, 525));
            Program.ComboMenu.Add(
                new MenuList("QOrderBy", "Q to position", new[] {"CLOSETOMOUSE", "CLOSETOTARGET"}));
            Program.ComboMenu.Add(new MenuBool("QChecks", "Q Safety Checks"));
            Program.ComboMenu.Add(new MenuBool("EQ", "Q After E", false));
            Program.ComboMenu.Add(new MenuBool("QR", "Q after Ult"));
            Program.ComboMenu.Add(new MenuBool("OnlyQinCombo", "Only Q in COMBO", false));
            Program.ComboMenu.Add(new MenuBool("FocusTwoW", "Focus 2 W Stacks"));
            Program.ComboMenu.Add(new MenuBool("ECombo", "Auto Condemn"));
            Program.ComboMenu.Add(
                new MenuKeyBind("ManualE", "Semi-Manual Condemn", Keys.E, KeyBindType.Press));
            Program.ComboMenu.Add(new MenuList("EMode", "E Mode",
                new[]
                {
                    "PRADASMART", "PRADAPERFECT", "MARKSMAN", "SHARPSHOOTER", "GOSU", "VHR", "PRADALEGACY", "FASTEST",
                    "OLDPRADA"
                }));
            Program.ComboMenu.Add(
                new MenuSlider("EPushDist", "E Push Distance", 450, 300, 475));
            Program.ComboMenu.Add(new MenuSlider("EHitchance", "E % Hitchance", 50));
            Program.ComboMenu.Add(new MenuBool("RCombo", "Auto Ult", false));
            Program.EscapeMenu.Add(new MenuBool("QUlt", "Smart Q-Ult"));
            Program.EscapeMenu.Add(new MenuBool("EInterrupt", "Use E to Interrupt"));
            //Program.GapcloserMenu = AntiGapcloser.Attach(Program.MainMenu, true);
            ////Program.GapcloserMenu.Add(new MenuBool("QAntiGapcloser", "Use Q"));
            //Program.GapcloserMenu.Add(new MenuBool("EAntiGapcloser", "Use E"));
            Program.LaneClearMenu.Add(new MenuBool("QLastHit", "Use Q to Lasthit"));
            Program.LaneClearMenu.Add(
                new MenuSlider("QLastHitMana", "Min Mana% for Q Lasthit", 45));
            Program.LaneClearMenu.Add(
                new MenuBool("EJungleMobs", "Use E on Jungle Mobs"));
            Program.SkinhackMenu.Add(new MenuBool("shkenabled", "Enabled"));
            Program.SkinhackMenu.Add(
                new MenuList("skin", "Skin: ",
                    new[]
                    {
                        "Classic", "Vindicator", "Aristocrat", "Dragonslayer", "Heartseeker", "SKT T1", "Arclight",
                        "Dragonslayer Green", "Dragonslayer Red", "Dragonslayer Azure", "Soulstealer"
                    }));
        }

        public static void FinishMenuInit()
        {
            Program.MainMenu.Add(Program.ComboMenu);
            Program.MainMenu.Add(Program.LaneClearMenu);
            Program.MainMenu.Add(Program.EscapeMenu);
            //Program.MainMenu.Add(Program.GapcloserMenu);
            Program.MainMenu.Add(Program.SkinhackMenu); // XD
            Program.MainMenu.Add(Program.DrawingsMenu);
            Program.MainMenu.Add(Program.OrbwalkerMenu);
            Program.MainMenu.Attach();
        }
    }
}
