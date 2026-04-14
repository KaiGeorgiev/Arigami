
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Arigami> wildArigamis;

    public Arigami GetRandomWildArigami()
    {//rarity wird noch implementiert 
        var wildArigami = wildArigamis[Random.Range(0, wildArigamis.Count)];
        wildArigami.Init();
        return wildArigami;
    }
}
