using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropTarget : MonoBehaviour, IDropHandler
{
	[Range(1, 10)]
	public int SlotCount = 1;
    public Image unitImage;
    public TMP_Text costText;

	public void OnDrop(PointerEventData eventData)
	{
		GameObject dropped = eventData.pointerDrag;

		if (dropped != null && dropped.GetComponent<UIDrag>() != null)
		{
			Sprite unit = dropped.GetComponent<Image>().sprite;
			PlayerPrefs.SetString("slot_" + (SlotCount - 1), unit.name);
			PlayerPrefs.Save();
            ChangeSlot(unit);
		}
	}

    void ChangeSlot(Sprite unit)
    {
        unitImage.sprite = unit;
        costText.text = "<sprite=0> " + Resources.Load<StatsObject>("Stats/" + unit.name).Cost;
    }

    void Awake()
    {
        string unitID = PlayerPrefs.GetString("slot_" + (SlotCount - 1));
        Sprite unit;
        if (unitID != "")
        {
            unit = Resources.Load<Sprite>("Icons/" + unitID);
        }
        else
        {
            unit = Resources.Load<Sprite>("Icons/Character_00000");
        }
        ChangeSlot(unit);
    }
}