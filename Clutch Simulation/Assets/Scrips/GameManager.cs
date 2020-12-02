using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject menu;
    public GameObject errorMsg;
    private bool errorOn = false;
    private float errorOnTime;
    private float errorShowTime = 2;
    private bool isPause;

    // Engine items
    private float engineRPM = 0;
    private string engineStatus = "Off";
    private readonly float[,] torqueGrid = {
        {20, 90, 107, 109, 110, 111, 114, 116 },
        {25, 105, 132, 133, 134, 136, 138, 141 },
        {35, 89, 133, 141, 142, 144, 145, 149 },
        {19, 70, 133, 147, 148, 150, 151, 155 },
        {3, 55, 133, 153, 159, 161, 163, 165 },
        {0, 41, 126, 152, 161, 165, 167, 171 },
        {0, 33, 116, 150, 160, 167, 170, 175 },
        {0, 26, 110, 155, 169, 176, 180, 184 },
        {0, 18, 106, 155, 174, 179, 185, 190 },
        {0, 12, 96, 147, 167, 175, 181, 187 },
        {0, 4, 84, 136, 161, 170, 175, 183 },
        {0, 0, 72, 120, 145, 153, 159, 171 },
        {0, 0, 0, 0, 0, 0, 0, 0}};  // torque map for a gasoline, spark ignition (SI) engine: from https://x-engineer.org/automotive-engineering/internal-combustion-engines/performance/power-vs-torque/
    private float[] pedalGrid = { 5, 10, 20, 30, 40, 50, 60, 100 };
    private float[] rpmGrid = { 800, 1300, 1800, 2300, 2800, 3300, 3800, 4300, 4800, 5300, 5800, 6300, 10000 };
    private float torqueFactor = 1.05f;
    private float torque;
    private float minRPM = 600f;
    private float maxSpeedDelta = 1000f;
    private float engineDrag = (171) / (6300f);
    private float engineIner = (0.005f);
    public AudioSource engineSound;
    private float startingTime = 1.5f;
    private float startTime;
    private Boolean attached;


    // Pedals
    public GameObject gasPedal;
    private Slider gasPedalSlider;
    public GameObject breakPedal;
    private Slider breakPedalSlider;
    public GameObject clutchPedal;
    private Slider clutchPedalSlider;
    public AudioSource breakSound;

    // Wheels items
    private float wheelsRPM = 0;
    private float clutchFactor;
    private float carDrag = 0.005f;// 171 / 6300f;
    public float breakPower = 500f;
    private float wheelsIner = 1f;

    // car items
    public GameObject car;
    private Vector3[] carPos = { new Vector3(-30f, -30f, -200f) , new Vector3(260f, -30f, -200f) , new Vector3(1650f, -30f, -200f) };
    
    // Gear items
    public GameObject gear;
    private GearManager gearManager;
    private float clutchForce;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        gearManager = gear.GetComponent<GearManager>();
        gasPedalSlider = gasPedal.GetComponent<Slider>();
        breakPedalSlider = breakPedal.GetComponent<Slider>();
        clutchPedalSlider = clutchPedal.GetComponent<Slider>();
        Reset();
    }
    void Update()
    {
        EngineListener(); // start/ stops engine and update torque according to gasPedal and RPM
        if (Input.GetKeyDown("escape"))
        {
            OpenMenu(); // open main manu
        } 
        if (errorOn)
        {
            if (Time.realtimeSinceStartup - errorOnTime > errorShowTime)
            {
                errorMsg.SetActive(false);
                errorOn = false;
            }
        }
    }
    private void FixedUpdate()
    {
        Physics(); 
        //WheelsPhysics();
        MoveCar();
    }
    private float GetTorque(float rpm, float pedal)
    {
        int iPedal = 0;
        while (pedal > pedalGrid[iPedal + 1] && iPedal < pedalGrid.Length - 2)
            iPedal++;
        float pedalDelta = ((pedal - pedalGrid[iPedal]) / (pedalGrid[iPedal + 1] - pedalGrid[iPedal]));

        int iRPM = 0;
        while (rpm > rpmGrid[iRPM + 1] && iRPM < rpmGrid.Length - 2)
            iRPM++;
        float rpmDelta = ((rpm - rpmGrid[iRPM]) / (rpmGrid[iRPM + 1] - rpmGrid[iRPM]));

        float returnTorque = torqueGrid[iRPM, iPedal];
        returnTorque += pedalDelta * (torqueGrid[iRPM, iPedal + 1] - torqueGrid[iRPM, iPedal]);
        returnTorque += rpmDelta * (torqueGrid[iRPM + 1, iPedal] - torqueGrid[iRPM, iPedal]);
        //returnTorque = 0.0006f * Mathf.Pow(pedal, 3) + -0.1159f * Mathf.Pow(pedal, 2) + 7.7158f * Mathf.Pow(pedal, 1);
        return returnTorque * torqueFactor; // on rpm, gas = 0 iterpulation return -24

    }
    public void DevFunction()
    {
        engineRPM = 2000;
        wheelsRPM = 2000;
    }
    public float GetEngineRPM()
    {
        return (int)(engineRPM);
    }
    public string GetEngineStatus()
    {
        return engineStatus;
    }
    public float GetWheelsSpeed()
    {
        return (int)(wheelsRPM) / 10;
    }
    public void ShowErrorMsg(String msg) // popping an error msg for a given time
    {
        errorOn = true;
        errorOnTime = Time.realtimeSinceStartup;
        errorMsg.GetComponent<Text>().text = msg;
        errorMsg.SetActive(true);
    }
    public void StartEngine()
    {
        if (engineStatus == "On" || engineStatus == "Starting")
        {
            torque = 0;
            engineStatus = "Off";
        }
        else
        {
            if (gearManager.GetGearRatio() != 0)
                ShowErrorMsg("Gear must be neutral");
            else
            {
                engineStatus = "Starting";
                starterCheck(true);
                engineRPM = 20;
            }
        }
    }
    void EngineListener()
    {
        if (Input.GetKeyDown("space"))
        {
            StartEngine();
        }
        if (engineStatus != "Off") // sets engine torque
            torque = GetTorque(engineRPM, gasPedalSlider.value);
        else
            torque = 0;
    }
    void starterCheck(bool start=false) // check if the engine is in "Starting" status
    {
        if (start)
            startTime = Time.realtimeSinceStartup;
        if (Time.realtimeSinceStartup - startTime > startingTime && engineStatus == "Starting")
        {
            Debug.Log("starting over");
            engineStatus = "On";
        }
    }
    void EngineOff(string engineOffText) // shutting down the enging and showing why the engine stoped
    {
        engineStatus = "Off";
        torque = 0;
        ShowErrorMsg(engineOffText);
    }
    void Physics() // calculation the change of the rotating of the engine accurding to inlet power, clutch and drag
    {

        engineSound.pitch = engineRPM / 2500;
        starterCheck();
        if (engineRPM <= minRPM && engineStatus == "On")
        {
            EngineOff("Low RPM, engine: OFF");
        }// Shuts down engine if RPM drops below a certian value
        float totalTourqueE;
        float deltaSpeedE;
        float dragE = engineRPM * engineDrag;
        float gearRatio = gearManager.GetGearRatio();

        float totalTourqueW;
        float deltaSpeedW;
        float dragW = wheelsRPM * carDrag;
        float breaks = Mathf.Sign(wheelsRPM) * (breakPedalSlider.value / 100) * breakPower;

        if (Mathf.Abs(wheelsRPM) < 50f && breaks != 0) // prevent RPM to rotate arond zero value
        {
            wheelsRPM = 0;
            breaks = 0;
        }
        breakSound.volume = Mathf.Abs(breaks) / 500;

        float targetedWheelRPM = engineRPM * gearRatio;
        if (clutchForce > torque )
        {
            //EngineOff("Too much power");
        }
        if (clutchPedalSlider.value < 5 && gearRatio != 0)
            attached = true;
        else
            attached = false;

        if (attached)
        {
            float changeForce = (targetedWheelRPM - wheelsRPM) * wheelsIner / Time.deltaTime;
            totalTourqueE = torque - dragE - dragW - breaks - changeForce;
            deltaSpeedE = totalTourqueE / (engineIner + wheelsIner * Mathf.Abs(gearRatio)) * Time.deltaTime;
            engineRPM += deltaSpeedE;
            wheelsRPM = engineRPM* gearRatio;
        }

        else
        {
            if (gearRatio == 0)
                clutchForce = 0;
            else
                clutchForce = (targetedWheelRPM - wheelsRPM) * (100 - clutchPedalSlider.value) / 100;
            totalTourqueE = torque - dragE - clutchForce;
            deltaSpeedE = totalTourqueE / engineIner * Time.deltaTime;
            engineRPM += deltaSpeedE;

            totalTourqueW = clutchForce - dragW - breaks;
            deltaSpeedW = totalTourqueW / wheelsIner * Time.deltaTime;
            wheelsRPM += deltaSpeedW;
        }    

        if (Mathf.Abs(engineRPM) < 10f) // prevent RPM to rotate arond zero value
            engineRPM = 0;
        if (Mathf.Abs(wheelsRPM) < 10f && breaks > 0) // prevent RPM to rotate arond zero value
            wheelsRPM = 0;

    }

    void MoveCar() // change the position of the car accurding to wheels RPM
    {
        if (car.transform.position.x > carPos[2].x)
            car.transform.position = carPos[0];
        else if (car.transform.position.x < carPos[0].x)
            car.transform.position = carPos[2];
        car.transform.Translate(Vector3.right * wheelsRPM * (carPos[2].x- carPos[0].x) * Time.deltaTime / 500); // on WheelsRPM = 500 the car whall go from pos 0 to pos 2 in deltaTima
        //Transform wheels = car.GetComponentInChildren<Transform>();
        //wheels.RotateAround(wheels.position, wheels.forward, Time.deltaTime * 90f);

    }
    public void Reset() // setting the simulator back to starting position
    {
        car.transform.position = carPos[1];
        engineRPM = 0;
        wheelsRPM = 0;
        gasPedalSlider.value = gasPedalSlider.minValue;
        breakPedalSlider.value = breakPedalSlider.minValue;
        clutchPedalSlider.value = clutchPedalSlider.minValue;
        engineStatus = "Off";
        gearManager.ResetGear();


    }

    public void OpenMenu()
    {
        menu.SetActive(true);
    }
}