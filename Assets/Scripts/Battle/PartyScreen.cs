using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText; 

    PartyMemberUI[] memberSlots;
    List<Arigami> arigamis;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Arigami> arigamis)
    {
        this.arigamis = arigamis;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if(i < arigamis.Count) 
            {
                memberSlots[i].setData(arigamis[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Wähle ein Arigami";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < arigamis.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            } else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
