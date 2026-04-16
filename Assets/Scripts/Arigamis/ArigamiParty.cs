using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArigamiParty : MonoBehaviour
{
    [SerializeField] List<Arigami> arigamis;
    
    public List<Arigami> Arigamis
    {
        get { return arigamis; }
    }

    private void Start()
    {
        foreach(var arigami in arigamis)
        {
            arigami.Init();
        }
    }

    public Arigami GetHealthyArigami()
    {
        return arigamis.Where(x => x.HP > 0).FirstOrDefault();
    }
}
