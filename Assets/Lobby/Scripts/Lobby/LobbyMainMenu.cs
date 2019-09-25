using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
public class LobbyMainMenu : MonoBehaviour 
{
    public LobbyManager lobbyManager;

    public RectTransform lobbyServerList;
    public RectTransform lobbyPanel;

    public InputField ipInput;

    public void OnEnable()
    {
        lobbyManager.topPanel.ToggleVisibility(true);
        lobbyManager.background.SetActive(true);
        ipInput.onEndEdit.RemoveAllListeners();
        ipInput.onEndEdit.AddListener(onEndEditIP);

    }

    public void OnClickHost()
    {
        lobbyManager.StartHost();
    }

    public void OnClickJoin()
    {
        lobbyManager.ChangeTo(lobbyPanel);

        lobbyManager.networkAddress = ipInput.text;
        lobbyManager.StartClient();

        lobbyManager.backDelegate = lobbyManager.StopClientClbk;
        lobbyManager.DisplayIsConnecting();

        lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
    }

    public void OnClickDedicated()
    {
        lobbyManager.ChangeTo(null);
        lobbyManager.StartServer();

        lobbyManager.backDelegate = lobbyManager.StopServerClbk;

        lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
    }


    void onEndEditIP(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickJoin();
        }
    }

}

