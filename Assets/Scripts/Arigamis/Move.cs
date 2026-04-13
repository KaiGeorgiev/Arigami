using UnityEngine;

public class Move 
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase aBase)
    {
        Base = aBase;
        PP = aBase.PP;
    }
}
