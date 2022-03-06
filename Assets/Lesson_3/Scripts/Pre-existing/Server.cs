using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{  
    private const int MAX_CONNECTION = 10;

    private int port = 5805;

    private int hostID;
    private int reliableChannel;

    private bool isStarted = false;
    private byte error;

    List<int> connectionIDs = new List<int>();

    private Dictionary<int, string> _userNamesByConnectionID = new Dictionary<int, string>();

    public void StartServer()
    {
        //192.168.0.121:2000

        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);         
        hostID = NetworkTransport.AddHost(topology, port);

        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    connectionIDs.Add(connectionId);

                    SendMessageToAll($"Player {connectionId} has connected.");
                    Debug.Log($"Player {connectionId} has connected.");
                    break;

                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);

                    if (!_userNamesByConnectionID.ContainsKey(connectionId))
                    {
                            _userNamesByConnectionID.Add(connectionId,
                                !string.IsNullOrEmpty(message) ? message: $"Player {connectionId}");

                        SendMessageToAll($"Player {connectionId} is now known as {_userNamesByConnectionID[connectionId]}");
                        Debug.Log($"Player {connectionId} is now known as {_userNamesByConnectionID[connectionId]}");
                    }
                    else
                    {
                        //SendMessageToAll($"Player {connectionId}: {message}");
                        //Debug.Log($"Player {connectionId}: {message}");

                        SendMessageToAll($"{_userNamesByConnectionID[connectionId]}: {message}");
                        Debug.Log($"{_userNamesByConnectionID[connectionId]}: {message}");
                    }
                    break;

                case NetworkEventType.DisconnectEvent:
                    connectionIDs.Remove(connectionId);

                    SendMessageToAll($"Player {connectionId} has disconnected.");
                    Debug.Log($"Player {connectionId} has disconnected.");
                    break;

                case NetworkEventType.BroadcastEvent:
                    break;

            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    public void ShutDownServer()
    {
        if (!isStarted)
            return;

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.Log((NetworkError)error);

    }

    public void SendMessage(Message message, int connectionID)
    {
        string jsonMessage = JsonUtility.ToJson(message);
        byte[] buffer = Encoding.Unicode.GetBytes(jsonMessage);

        
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, jsonMessage.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.Log((NetworkError)error);

    }

    public void SendMessageToAll(string message)
    {
        for (int i = 0; i < connectionIDs.Count; i++)        
            SendMessage(message, connectionIDs[i]);        
    }

   public void RegisterHandler(short msgType, IMessageHandler messageHandler)
    {

    }
}

public interface IMessageHandler
{
    IMessage Deserialize(byte[] bytes);

    byte[] Serialize(IMessage message);
}

public interface IMessage
{

}

[Serializable]
public class Message
{
    public int i;
    public float j;
    public float k;
}