using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace UDP
{
    public class UDPReceiver : MonoBehaviour
    {

        public GameObject universe;
        public static Queue<Task> taskQueue = new Queue<Task>();
        UDPSocket receiver;

        // Start is called before the first frame update
        void Start()
        {
            receiver = new UDPSocket();
            receiver.Bind("127.0.0.1", 2000);
        }

        // Update is called once per frame
        void Update()
        {
            if (taskQueue.Count > 0)
            {
                universe.SendMessage("OnMovement", taskQueue.Dequeue().GetMessage());
            }

        }

        public void CloseSocket()
        {
            receiver.CloseSocket();
        }

        public class Task
        {
            readonly string message;

            public Task(string message)
            {
                this.message = message;
            }

            public string GetMessage()
            {
                return message;
            }
        }
    }
}
