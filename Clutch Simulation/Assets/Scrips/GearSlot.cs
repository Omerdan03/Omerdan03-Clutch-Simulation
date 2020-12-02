using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearSlot : MonoBehaviour, IDropHandler
{
    public GameObject gear;
    private GearManager gearManager;

    void Awake()
    {
        gearManager = gear.GetComponent<GearManager>();
    }
    public void OnDrop(PointerEventData eventData)
    {

        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("gear: " + gameObject.name);
            gearManager.SetShift(gameObject.name);
        }
            

    }

}
