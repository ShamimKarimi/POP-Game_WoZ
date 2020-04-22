using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace UDP
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];

        }

        public void Bind(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();

        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (asynchResult) =>
            {

                State so = (State)asynchResult.AsyncState;

                int bytes = _socket.EndReceiveFrom(asynchResult, ref epFrom);

                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);

                UDPReceiver.taskQueue.Enqueue(new UDPReceiver.Task(Encoding.ASCII.GetString(so.buffer, 0, bytes)));
                
                // Debug.Log("From: " + epFrom.ToString() + ", Message: " + Encoding.ASCII.GetString(so.buffer, 0, bytes));

            }, state);
        }

        public void CloseSocket()
        {
            _socket.Close();
        }
    }
}