
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Button buttonStartServer;
    [SerializeField]
    private Button buttonShutDownServer;
    [SerializeField]
    private Button buttonConnectClient;
    [SerializeField]
    private Button buttonDisconnectClient;
    [SerializeField]
    private Button buttonSendMessage;

    [SerializeField] TMP_InputField _nameInputField;

    [SerializeField]
    private TMP_InputField inputField;


    [SerializeField]
    private TextField textField;

    [SerializeField]
    private Server server;
    [SerializeField]
    private Client client;

    private void Start()
    {
        buttonStartServer.onClick.AddListener(() => StartServer());
        buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
        buttonConnectClient.onClick.AddListener(() => Connect());
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => SendMessage());
        inputField.onEndEdit.AddListener((text) =>SendMessage());
        client.onMessageReceive += ReceiveMessage;
        client.onConnected += SendName;
    }

    private void StartServer() =>    
        server.StartServer();
    
    private void ShutDownServer() =>    
        server.ShutDownServer();
    
    private void Connect() =>    
        client.Connect();    

    private void Disconnect() =>    
        client.Disconnect();    

    private void SendMessage()
    {
        client.SendMessage(inputField.text);
        inputField.text = "";
    }

    private void SendName()
    {
        client.SendMessage(_nameInputField.text);
    }

    public void ReceiveMessage(object message) =>    
        textField.ReceiveMessage(message);
    
}
