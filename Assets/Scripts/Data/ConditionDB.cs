using System.Collections.Generic;
using UnityEngine;

public class ConditionDB 
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
       { 
           ConditionID.psn,
           new Condition()
           {
               Name = "Vergiftet",
               StartMessage = "wurde vergiftet",
               OnAfterTurn = (Arigami arigami) =>
               {
                   arigami.UpdateHp(arigami.MaxHp / 8 );
                   arigami.StatusChanges.Enqueue($"{arigami.Base.ArigamiName} wurde durch Gift verletzt!");
               }
           }
       },
       {
           ConditionID.brn,
           new Condition()
           {
               Name = "Brennt",
               StartMessage = "wurde in brand gesetzt",
               OnAfterTurn = (Arigami arigami) =>
               {
                   arigami.UpdateHp(arigami.MaxHp / 16);
                   arigami.StatusChanges.Enqueue($"{arigami.Base.ArigamiName} wurde durch brand verletzt!");
               }
           }
       }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
