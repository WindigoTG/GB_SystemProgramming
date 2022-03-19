using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SolarSystemNetworkManager : NetworkManager
{
    public class MyMessage : MessageBase
    {
        public int value;
        public int value2;
        public int value3;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(value);
            writer.Write(value2);
            writer.Write(value3);
        }

        public override void Deserialize(NetworkReader reader)
        {
            value = reader.ReadInt32();
            value2 = reader.ReadInt16();
            value3 = reader.ReadInt32();
        }
    }


    public override void OnStartClient(NetworkClient client)
    {
        //NetworkServer.
        NetworkClient networkClient = new NetworkClient();
        networkClient.RegisterHandler(100, (MyMessage) => { Debug.Log(MyMessage.ReadMessage<MyMessage>().value); });
        networkClient.Send(100, new MyMessage() { value = 10 });


        NetworkMessageDelegate networkMessageDelegate;
        base.OnStartClient(client);
    }



    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (playerPrefab == null)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
            }
        }
        else if (playerPrefab.GetComponent<NetworkIdentity>() == null)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
            }
        }
        else if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("There is already a player at that playerControllerId for this connections.");
            }
        }
        else
        {
            Transform startPosition = GetStartPosition();
            GameObject player = (!(startPosition != null)) ? Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) : Object.Instantiate(playerPrefab, startPosition.position, startPosition.rotation);
            var playerCharacter = player.GetComponentInChildren<PlayerCharacter>();
            playerCharacter.connection = conn;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }
}
