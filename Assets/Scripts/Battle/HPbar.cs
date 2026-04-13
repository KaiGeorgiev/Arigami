using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image health;

    // Diese Methode rufst du von außen auf (z.B. wenn der Gegner angreift)
    public void SetHP(float hpNormalized)
    {
        health.fillAmount = hpNormalized;
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHp = health.fillAmount;
        float changeAmt = curHp - newHp;
        
        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;  
            health.fillAmount = curHp;
            yield return null;
        }
        health.fillAmount = newHp;
    }
}
