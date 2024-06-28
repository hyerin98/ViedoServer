using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    public static Server instance;
    public TMP_InputField PortInput;


    private const int port = 7777;

    public List<ServerClient> clients;
    List<ServerClient> disconnectList;

    TcpListener server;
    bool serverStarted;
    public Button ConnectedServerButton;

    void Awake()
    {
        instance = this;
    }

    public void ServerCreate()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            VideoPlayerController.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
            TraceBox.Log($"서버가 {port}에서 시작되었습니다.");

            // 서버 생성되면 안보이게하기
            PortInput.gameObject.SetActive(false);
            ConnectedServerButton.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            VideoPlayerController.instance.ShowMessage($"Socket error: {e.Message}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PortInput.ActivateInputField();
            ServerCreate();
        }

        if (!serverStarted) return;

        foreach (ServerClient c in clients)
        {
            if (!IsConnected(c.tcp))
            {
                TraceBox.Log("tcp연결x");
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                TraceBox.Log("tcp연결o");
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    string data = new StreamReader(s, true).ReadLine();
                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast($"{disconnectList[i].clientName} 연결이 끊어졌습니다", clients);
            TraceBox.Log($"{disconnectList[i].clientName} 연결이 끊어졌습니다", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
        TraceBox.Log("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
    }

    public void OnIncomingData(ServerClient c, string data)
    {
        if (data.Contains("&NAME"))
        {
            c.clientName = data.Split('|')[1];
            Broadcast($"{c.clientName}이 연결되었습니다", clients);
            TraceBox.Log($"{c.clientName}이 연결되었습니다", clients);
            return;
        }

        if (data.StartsWith("VIDEO"))
        {
            VideoPlayerController.instance.OnIncomingData(data);
            TraceBox.Log("영상 재생중");
            Broadcast("ENABLE_BUTTONS", clients);
        }
        else if (data.StartsWith("EXIT"))
        {
            VideoPlayerController.instance.OnIncomingData(data);
            TraceBox.Log("영상 종료");
            Broadcast("ENABLE_BUTTONS", clients);
        }
        else if (data.StartsWith("VIDEO_FINISHED"))
        {
            VideoPlayerController.instance.ServerNotifyVideoFinished(data);
            string name = data.Split('|')[1];

            TraceBox.Log($"영상 {name} 재생 종료");
            Broadcast("ENABLE_BUTTONS", clients);
        }
        else
        {
            Broadcast($"{c.clientName} : {data}", clients);
            TraceBox.Log("영상이 없어");
        }
    }

    public void Broadcast(string data, List<ServerClient> cl) 
    {
        foreach (var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                VideoPlayerController.instance.ShowMessage($"쓰기 에러 : {e.Message}를 클라이언트에게 {c.clientName}");
                TraceBox.Log($"쓰기 에러 : {e.Message}를 클라이언트에게 {c.clientName}");
            }
        }
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}