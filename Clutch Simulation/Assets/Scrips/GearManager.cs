using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearManager : MonoBehaviour
{
    public GameObject stick;
    public GameObject N;
    public GameObject one;
    public GameObject two;
    public GameObject three;
    public GameObject four;
    public GameObject five;
    public GameObject R;
    private string shift;
    private Dictionary<string, float> gearRatio = new Dictionary<string, float>();


    // Start is called before the first frame update
    void Start() // inserting the rations on the Dictionarty
    {
        shift = "N";
        gearRatio["N"] = 0;
        gearRatio["One"] = 0.05f;
        gearRatio["Two"] = 0.12f;
        gearRatio["Three"] = 0.2f;
        gearRatio["Four"] = 0.35f;
        gearRatio["Five"] = 0.5f;
        gearRatio["R"] = -0.06f;

    }

    // Update is called once per frame
    void Update() // listen to user inputs
    {
        if (Input.GetKeyDown("0"))
        {
            SetShift("N");
            stick.transform.position = N.transform.position;
        }
        if (Input.GetKeyDown("1"))
        {
            SetShift("One");
            stick.transform.position = one.transform.position;
        }
        if (Input.GetKeyDown("2"))
        {
            SetShift("Two");
            stick.transform.position = two.transform.position;
        }
        if (Input.GetKeyDown("3"))
        {
            SetShift("Three");
            stick.transform.position = three.transform.position;
        }
        if (Input.GetKeyDown("4"))
        {
            SetShift("Four");
            stick.transform.position = four.transform.position;
        }
        if (Input.GetKeyDown("5"))
        {
            SetShift("Five");
            stick.transform.position = five.transform.position;
        }
        if (Input.GetKeyDown("6"))
        {
            SetShift("R");
            stick.transform.position = R.transform.position;
        }
    }
        
    public void SetShift(string newShift) 
    {
        shift = newShift;
    }    
    public float GetGearRatio()
    {
        return gearRatio[shift];
    }
    public void ResetGear() // reset gear back to neutral
    {
        SetShift("N");
        stick.transform.position = N.transform.position;
    }
}
