using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{
    public void LoadMenu()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        WebcamStream webcamStream = (WebcamStream)mainCamera.GetComponent(typeof(WebcamStream));
        webcamStream.StopStreaming();
        SceneManager.LoadScene("Menu");
    }
}
