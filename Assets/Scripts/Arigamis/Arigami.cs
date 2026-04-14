using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arigami
{
    [SerializeField] ArigamiBase _base;
    [SerializeField] int level;

    public ArigamiBase Base 
    { 
        get { return _base; }      
    }
    public int Level 
    { 
        get { return level; }
    }
    public int HP {  get; set; }
    public List<Move> Moves { get; set; }


    public void Init()
    {
        HP = MaxHp;

        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LernableMoves)
        {
            if(move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4)
                {
                    break;
                }
            }
        }
    }

    public int Attack {  
        get { return Mathf.FloorToInt((Base.Attack * Level / 100) + 5); } 
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level / 100) + 5); }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level / 100) + 5); }
    }

    public int SpDefence
    {
        get { return Mathf.FloorToInt((Base.SpDefence * Level / 100) + 5); }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level / 100) + 5); }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level / 100) + 10); }
    }

    // hier muss im nachgang nochmal genauer geschaut werden KEIN
    // spAtt
    // etc. 
    public DamageDetails TakeDamage(Move move, Arigami attacker)
    {
        float critical = 1f;
        //krit
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        // Effektivitäet berechnen 
        float type = TypeChart.GetEffectiveness(move.Base.Typ, this.Base.Typ1) * TypeChart.GetEffectiveness(move.Base.Typ, this.Base.Typ2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        int damage = 0;
        if (move.Base.Category != MoveCategory.Status) {
            float attackValue = (move.Base.Category == MoveCategory.Physical) ? attacker.Attack : attacker.SpAttack;
            float defenseValue = (move.Base.Category == MoveCategory.Physical) ? Defense : SpDefence;


            float modifiers = Random.Range(0.85f, 1f) * type * critical;
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.Base.Power * ((float)attackValue / defenseValue) + 2;
            damage = Mathf.FloorToInt(d * modifiers);
        } 
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
           damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

}

public class DamageDetails
{
    public bool Fainted {  get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
