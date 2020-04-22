using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamStream : MonoBehaviour
{
    public RawImage background;

    WebCamTexture webCamTexture;

    void Start()
    {

        WebCamDevice[] devices = WebCamTexture.devices;

        // setting default camera
        webCamTexture = new WebCamTexture(devices[0].name);

        for (int i = 0; i < devices.Length; i++)
        {
            // setting Many Webcam as camera if found
            if (devices[i].name == "ManyCam Virtual Webcam")
            {
                webCamTexture = new WebCamTexture(devices[i].name);
            }
        }

        //webCamTexture = new WebCamTexture(devices[0].name);

        webCamTexture.Play();
        background.texture = webCamTexture;
    }

    public void StopStreaming()
    {
        webCamTexture.Stop();
    }
}
