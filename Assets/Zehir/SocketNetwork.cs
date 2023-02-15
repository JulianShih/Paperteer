using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class SocketNetwork : MonoBehaviour
{
    Socket serverSocket;
    IPAddress ip;
    IPEndPoint ipEnd;
    Thread connectThread;

    void Start() {
        InitSocket();
    }

    void InitSocket()
    {
        ip = IPAddress.Parse("127.0.0.1");
        ipEnd = new IPEndPoint(ip, 50005);
        connectThread = new Thread(new ThreadStart(SocketConnect));
        connectThread.Start();
    }

    void SocketConnect()
    {
        if (serverSocket != null)
            serverSocket.Close();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Connect(ipEnd);
    }

    void SocketSend(string sendStr)
    {
        byte[] sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(sendStr);
        Debug.Log("socket: " + sendStr);
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
    }

    void SocketQuit()
    {
        //close thread
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //close socket
        if (serverSocket != null)
            serverSocket.Close();
        Debug.Log("socket disconnect");
    }

    
    public void SendRecord(string filename) {
        SocketSend("RECORD " + filename);
    }

    public void SendSave() {
        SocketSend("SAVE");
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}