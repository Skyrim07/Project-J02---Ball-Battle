using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLogic : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(600, 1000, false);
    }
    public void LoadMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
    public void LoadPenaltyScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Penalty");
    }
}
