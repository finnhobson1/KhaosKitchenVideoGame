using UnityEngine;
using UnityEngine.Networking;

public class MyLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();
        Player player = gamePlayer.GetComponent<Player>();

        player.PlayerUserName = lp.playerName;
        player.PlayerColour = lp.playerColor;
        player.PlayerNetworkID = lp.netId.Value;
    }
}
