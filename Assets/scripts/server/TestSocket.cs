
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class TestSocket : MonoBehaviour
{
    public static TestSocket instance;
    public string ipAdress = "127.0.0.1";
    public int port = 8088;
    private byte[] data = new byte[1024];
    private Socket clientSocket;
    private Thread receiveT;
    public Queue<string> receiveMessage = new Queue<string>();
    private TestSocket()
    {
     }
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(IPAddress.Parse(ipAdress), port);
            Debug.Log("连接服务器成功");
            receiveT = new Thread(ReceiveMsg);
            receiveT.Start();
        }
        catch (System.Exception ex)
        {
            Debug.Log("连接服务器失败！");
            Debug.Log(ex.Message);
        }
    }

    private void ReceiveMsg()
    {
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                Debug.Log("与服务器断开了连接");
                break;
            }
            int lenght = 0;
            lenght = clientSocket.Receive(data);
            ByteBuffer bb = new ByteBuffer(data);
            int len = bb.ReadShort();
           string str= bb.ReadString();
            receiveMessage.Enqueue(str );
           // Debug.Log(str);
           // string str = Encoding.UTF8.GetString(data, 0, data.Length);
          
        }

    }
    void SendMes(string ms)
    {
        byte[] data = new byte[1024];
        data = Encoding.UTF8.GetBytes(ms);
        clientSocket.Send(data);
    }



    //void SendMes(object obj)

    //{

    //    byte[] data = new byte[1024];

    //    data = obj.SerializeToByteArray();

    //    clientSocket.Send(data, data.Length, 0);

    //}



    void OnDestroy()

    {
        try
        {
            if (clientSocket != null)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();//关闭连接
            }
            if (receiveT != null)
            {
                receiveT.Interrupt();
                receiveT.Abort();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

}
