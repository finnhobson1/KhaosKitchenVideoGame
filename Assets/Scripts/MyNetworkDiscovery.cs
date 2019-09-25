using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class MyNetworkDiscovery : NetworkDiscovery {

    public LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = GetComponent<LobbyManager>();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("Found IP Address: " + fromAddress);
        lobbyManager.networkAddress = fromAddress;
    }

}
