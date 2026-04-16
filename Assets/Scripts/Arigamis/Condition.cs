using System;
using UnityEngine;

public class Condition 
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string StartMessage { get; set; }

    public Action<Arigami> OnAfterTurn {  get; set; }

}
