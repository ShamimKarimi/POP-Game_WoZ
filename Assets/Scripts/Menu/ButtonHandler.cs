using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public void LoadCalibration()
    {
        SceneManager.LoadScene("Calibration");
    }

    public void LoadGame1()
    {
        SceneManager.LoadScene("1");
        Debug.Log("game 1 button clicked");
    }

    public void LoadGame2()
    {
        SceneManager.LoadScene("2");
    }

    public void LoadGame3()
    {
        SceneManager.LoadScene("3");
    }

    public void LoadGame4()
    {
        SceneManager.LoadScene("4");
    }

}

