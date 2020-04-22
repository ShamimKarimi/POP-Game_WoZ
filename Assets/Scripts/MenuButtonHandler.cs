using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{
    public void LoadMenu()
    {
        GameObject udpGameObject = GameObject.Find("UDP");
        if (udpGameObject != null)
        {
            UDP.UDPReceiver udpReceiver = (UDP.UDPReceiver)udpGameObject.GetComponent(typeof(UDP.UDPReceiver));
            udpReceiver.CloseSocket();
        }
        GameObject mainCamera = GameObject.Find("Main Camera");
        WebcamStream webcamStream = (WebcamStream)mainCamera.GetComponent(typeof(WebcamStream));
        webcamStream.StopStreaming();
        SceneManager.LoadScene("Menu");
    }
}
