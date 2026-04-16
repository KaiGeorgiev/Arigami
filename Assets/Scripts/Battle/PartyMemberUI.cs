using TMPro;
using UnityEngine;

public class PartyMemberUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpbar;

    [SerializeField] Color highlightedColor;

    Arigami _arigami;
    public void setData(Arigami arigami)
    {
        _arigami = arigami;
        nameText.text = arigami.Base.ArigamiName;
        levelText.text = "lv. " + arigami.Level;
        hpbar.SetHP((float)arigami.HP / arigami.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedColor;
        }else
        {
            nameText.color = Color.black;
        }

    }
}
