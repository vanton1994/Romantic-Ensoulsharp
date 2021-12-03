using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyLogic.E;
using PRADA_Vayne.MyLogic.Others;
using Events = PRADA_Vayne.MyLogic.Q.Events;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void LoadLogic()
        {
            #region Q

            Orbwalker.OnAfterAttack += Events.AfterAttack;
            Orbwalker.OnBeforeAttack += Events.BeforeAttack;
            Spellbook.OnCastSpell += Events.OnCastSpell;
            AIBaseClient.OnProcessSpellCast += Events.OnProcessSpellCast;
            Game.OnUpdate += Events.OnUpdate;

            #endregion Q

            #region E

            GameObject.OnCreate += AntiAssasins.OnCreateGameObject;
            AntiGapcloser.OnGapcloser += MyLogic.E.Events.OnGapcloser;
            Game.OnUpdate += MyLogic.E.Events.OnUpdate;
            //Interrupter.OnInterrupterSpell += MyLogic.E.Events.OnPossibleToInterrupt;
            Game.OnUpdate += MyLogic.E.Events.JungleUsage;
            GameObject.OnCreate += MyLogic.E.Events.OnCreate;
            GameObject.OnDelete += MyLogic.E.Events.OnDelete;

            #endregion E

            #region R

            Spellbook.OnCastSpell += MyLogic.R.Events.OnCastSpell;

            #endregion R

            #region Others

            Game.OnUpdate += MyLogic.Others.Events.OnUpdate;
            AIBaseClient.OnProcessSpellCast += MyLogic.Others.Events.OnProcessSpellcast;
            Drawing.OnDraw += MyLogic.Others.Events.OnDraw;
            Game.OnUpdate += SkinHack.OnUpdate;

            #endregion Others
        }
    }
}