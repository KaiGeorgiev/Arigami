using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Arigami
{

    public ArigamiBase Base { get; set; }
    public int Level { get; set; }
    public int HP {  get; set; }
    public List<Move> Moves { get; set; }


    public Arigami(ArigamiBase aBase, int aLevel)
    {
        Base = aBase;
        Level = aLevel;
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
    //  spAtt
    //  modifier
    //  kein krit 
    // etc. 
    public bool TakeDamage(Move move, Arigami attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

}
