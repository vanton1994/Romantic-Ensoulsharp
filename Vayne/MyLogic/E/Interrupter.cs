using EnsoulSharp.SDK;
using EnsoulSharp;
using System;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnPossibleToInterrupt(AIBaseClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (args.DangerLevel == Interrupter.DangerLevel.High && Program.E.IsReady() &&
                Program.E.IsInRange(args.Sender) && args.Sender.CharacterName != "Shyvana" &&
                args.Sender.CharacterName != "Vayne")
            {
                Program.E.Cast(args.Sender);
            }
        }
    }
}