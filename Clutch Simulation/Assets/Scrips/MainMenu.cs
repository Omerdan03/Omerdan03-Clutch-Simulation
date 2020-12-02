using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameManager GM;
    public GameObject gameManager;
    void Start()
    {
        GM = gameManager.GetComponent<GameManager>();
    }
    public void Continue()
    {
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Reset()
    {
        GM.Reset();
        Continue();
    }
}
