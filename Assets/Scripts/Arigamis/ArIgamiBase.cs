using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Arigami", menuName = "Arigami/Create new Arigami")]
public class ArigamiBase : ScriptableObject
{
    [SerializeField] string arigamiName;
    [TextArea]
    [SerializeField] string beschreibung;
    
    [Header("Scaling")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] float battleSpriteScale = 100f;
    [SerializeField] bool isFlying;
    
    [Header("Characteristics")]
    [SerializeField] ArigamiType typ1;
    [SerializeField] ArigamiType typ2;

    [Header("Stats")]
    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] List<LernableMove> lernableMove;


    public string ArigamiName => arigamiName;
    public string Beschreibung => beschreibung;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public float BattleSpriteScale => battleSpriteScale;
    public bool IsFlying => isFlying;
    public ArigamiType Typ1 => typ1;
    public ArigamiType Typ2 => typ2;
    public int MaxHp => maxHp;
    public int Attack => attack;
    public int Defense => defence;
    public int SpAttack => spAttack;
    public int SpDefence => spDefence;
    public int Speed => speed;
    public List<LernableMove> LernableMoves => lernableMove;

}

[System.Serializable]
public class LernableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
