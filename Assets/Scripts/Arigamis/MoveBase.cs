using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Arigami/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;
    [TextArea]
    [SerializeField] string beschreibung;

    [Header("Characteristics")]
    [SerializeField] ArigamiType typ;
    [SerializeField] MoveCategory category;

    [Header("Stats")]
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    [Header("Effects")]
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;







    public string MoveName
    {
        get { return moveName; }
    }

    public string Beschreibung
    {
        get { return beschreibung; }
    }

    public ArigamiType Typ
    {
        get { return typ; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int PP
    {
        get { return pp; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;

    public List<StatBoost> Boosts { get { return boosts; } }

    public ConditionID Status { get { return status; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory { Physical, Special, Status }

public enum MoveTarget { Foe, Self }
