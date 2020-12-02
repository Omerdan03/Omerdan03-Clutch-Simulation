using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
    private GameManager GM;
    public GameObject gameManager;
    public Text engineRPM;
    public Text wheelsRPM;
    public Text engineStatus;
    public Text startButton;

    // Start is called before the first frame update
    void Start()
    {
        GM = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()  // every update the texts on the panel updates
    {
        engineRPM.text = "Engine RPM: " + (int)GM.GetEngineRPM(); // showing engine RPM on the Panel
        wheelsRPM.text = "Car speed: " + (int)GM.GetWheelsSpeed(); // showing engine RPM on the Panel
        engineStatus.text = "Engine: "+ GM.GetEngineStatus(); // showing engine RPM on the Panel
    }

    public void StartEngine()
    {
        GM.StartEngine();
    }
}
