using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Arigami/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;
    [TextArea]
    [SerializeField] string beschreibung;

    [Header("Characteristics")]
    [SerializeField] ArigamiType typ;
    [SerializeField] MoveCategory category ;
    
    [Header("Stats")]
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;




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
}
