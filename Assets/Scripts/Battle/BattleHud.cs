using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpbar;

    Arigami _arigami;
    public void setData(Arigami arigami)
    {
        _arigami = arigami;
        nameText.text = arigami.Base.ArigamiName;
        levelText.text = "lv. " + arigami.Level;
        hpbar.SetHP((float) arigami.HP / arigami.MaxHp);
    }

    public IEnumerator UpdateHP() 
    {
        yield return hpbar.SetHPSmooth((float)_arigami.HP / _arigami.MaxHp);
    }
}
