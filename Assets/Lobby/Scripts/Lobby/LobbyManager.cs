using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;



public class LobbyManager : NetworkLobbyManager 
{
    static short MsgKicked = MsgType.Highest + 1;

    static public LobbyManager s_Singleton;

    [Header("Unity UI Lobby")]
    [Tooltip("Time in second between all players ready & match start")]
    public float prematchCountdown = 5.0f;

    [Space]
    [Header("UI Reference")]
    public LobbyTopPanel topPanel;

    public RectTransform mainMenuPanel;
    public RectTransform lobbyPanel;

    public LobbyInfoPanel infoPanel;
    public LobbyCountdownPanel countdownPanel;
    public GameObject addPlayerButton;
    public GameObject hostButton;
    public GameObject joinButton;
    public GameObject background;

    protected RectTransform currentPanel;

    public Button backButton;
    public Button settingsButton;
    public Text statusInfo;
    public Text hostInfo;

    //Client numPlayers from NetworkManager is always 0, so we count (through connect/destroy in LobbyPlayer) the number
    //of players, so that even client know how many player there is.
    [HideInInspector]
    public int _playerNumber = 0;

    protected bool _disconnectServer = false;
    
    protected LobbyHook _lobbyHooks;
    
    // Game Settings
    public Text roundTimeText, BaseInstructionNumberText, InstructionNumberIncreasePerRoundText, BaseInstructionTimeText, InstructionTimeReductionPerRoundText, InstructionTimeIncreasePerPlayerText, MinimumInstructionTimeText;

    public Text playerCountText;
    public Slider playerCountSlider;
    public Text toggleText;
    public Toggle easyPhoneInteractions;
    public Text phoneInteractionText;

    public Text Feedback;

    private static List<string> feedbackList = new List<string>(new string[] { "Pretty much the perfect game", "This game gets 4.7/5 tomatoes - Tomato Critic",  "Good game, needs more nuggets and cheese", "The only kitchen game worth your time", "As far as mobile games go, this is suprisingly decent", "The developers have certainly earned a few pints for this", "Graphics to rival any console game", "I don't think these people know how to spell chaos", "Angry birds would be scared if it was still 2010", "This game is powered by coffee, redbull and pro plus", "Space team? Overcooked? Never heard of them", "Look mum I finally made a game!", "Cat in the wall? Now you’re talking my language!", "Brexits a mess, but at least its not Trump", "Basically one step away from photo realism ", "The original NFC capable mobile game!", "KFC is better than McDonalds. Fight me.", "Fun Fact: Our planet is dying!", "This shout out goes to spoons for being an OG", "help me, im trapped in the phone, send rescue" });

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        s_Singleton = this;
        _lobbyHooks = GetComponent<LobbyHook>();
        currentPanel = mainMenuPanel;

        backButton.gameObject.SetActive(false);
        GetComponent<Canvas>().enabled = true;

        DontDestroyOnLoad(gameObject);

        SetServerInfo("Offline", "None");
        background.SetActive(true);

        int quoteNo = UnityEngine.Random.Range(0, feedbackList.Count - 1);
        Feedback.text = "\"" + feedbackList[quoteNo] + "\"";



#if UNITY_ANDROID
        settingsButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        joinButton.GetComponent<RectTransform>().localPosition -= new Vector3(0, 60, 0);
#else
        //joinButton.gameObject.SetActive(false);
#endif
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        background.SetActive(true);

        if (SceneManager.GetSceneAt(0).name == lobbyScene)
        {
            if (topPanel.isInGame)
            {
                ChangeTo(lobbyPanel);
               
                if (conn.playerControllers[0].unetView.isClient)
                {
                    backDelegate = StopHostClbk;
                }
                else
                    {
                        backDelegate = StopClientClbk;
                    }
                
            }
            else
            {
                ChangeTo(mainMenuPanel);
            }

            topPanel.ToggleVisibility(true);
            topPanel.isInGame = false;
        }
        else
        {
            ChangeTo(null);

            Destroy(GameObject.Find("MainMenuUI(Clone)"));

            topPanel.isInGame = true;
            topPanel.ToggleVisibility(false);
        }

    }

    public void ChangeTo(RectTransform newPanel)
    {
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(false);
        }

        if (newPanel != null)
        {
            newPanel.gameObject.SetActive(true);
        }

        currentPanel = newPanel;

        if (currentPanel != mainMenuPanel)
        {
            backButton.gameObject.SetActive(true);
        }
        else
        {
            backButton.gameObject.SetActive(false);
            SetServerInfo("Offline", "None");
        }
    }

    public void DisplayIsConnecting()
    {
        var _this = this;
        infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
    }

    public void SetServerInfo(string status, string host)
    {
        statusInfo.text = status;
        hostInfo.text = host;
    }

    public delegate void BackButtonDelegate();
    public BackButtonDelegate backDelegate;
    public void GoBackButton()
    {
        backDelegate();
        topPanel.isInGame = false;
    }

    // Server management

    public void AddLocalPlayer()
    {
        TryToAddPlayer();
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        player.RemovePlayer();
    }

    public void SimpleBackClbk()
    {
        ChangeTo(mainMenuPanel);
    }
             
    public void StopHostClbk()
    {
        StopHost();
      
        ChangeTo(mainMenuPanel);
    }

    public void StopClientClbk()
    {
        StopClient();

        ChangeTo(mainMenuPanel);
    }

    public void StopServerClbk()
    {
        StopServer();
        ChangeTo(mainMenuPanel);
    }

    class KickMsg : MessageBase { }
    public void KickPlayer(NetworkConnection conn)
    {
        conn.Send(MsgKicked, new KickMsg());
    }

    public void KickedMessageHandler(NetworkMessage netMsg)
    {
        infoPanel.Display("Kicked by Server", "Close", null);
        netMsg.conn.Disconnect();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();

        ChangeTo(lobbyPanel);
        backDelegate = StopHostClbk;
        SetServerInfo("Hosting", networkAddress);
    }

    public void OnPlayersNumberModified(int count)
    {
        _playerNumber += count;

        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

        addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
    }

    // Server callbacks

    // Disable the JOIN button if we don't have enough players
    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

        LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
        newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
            }
        }

        return obj;
    }

    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
    {
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
            }
        }
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

            if (p != null)
            {
                p.RpcUpdateRemoveButton();
                p.ToggleJoinButton(numPlayers >= minPlayers);
            }
        }
    }


    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        if (_lobbyHooks)
            _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

        return true;
    }


    // Countdown management
    public override void OnLobbyServerPlayersReady()
    {
        bool allready = true;
        for(int i = 0; i < lobbySlots.Length; ++i)
        {
            if(lobbySlots[i] != null)
                allready &= lobbySlots[i].readyToBegin;
        }

        if(allready)
            StartCoroutine(ServerCountdownCoroutine());
    }

    public IEnumerator ServerCountdownCoroutine()
    {
        float remainingTime = prematchCountdown;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime > 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {
                // To avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;

                for (int i = 0; i < lobbySlots.Length; ++i)
                {
                    if (lobbySlots[i] != null)
                    {
                        // There is maxPlayer slots, so some could be == null, need to test it before accessing!
                        (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                    }
                }
            }
        }

        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            if (lobbySlots[i] != null)
            {
                (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
            }
        }

        topPanel.enabled = false;

        ServerChangeScene(playScene);
    }

    // Client callbacks 
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        infoPanel.gameObject.SetActive(false);

        conn.RegisterHandler(MsgKicked, KickedMessageHandler);

        if (!NetworkServer.active)
        {
            // Only to do on pure client (not self hosting client)
            ChangeTo(lobbyPanel);
            backDelegate = StopClientClbk;
            SetServerInfo("Client", networkAddress);
        }
    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ChangeTo(mainMenuPanel);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        ChangeTo(mainMenuPanel);
        infoPanel.Display("Client error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
    }
    
    // Game Settings
    public void SetSettings()
    {
        GameSettings.RoundTime = string.IsNullOrEmpty(roundTimeText.text) ? 90 : int.Parse(roundTimeText.text);
        GameSettings.BaseInstructionNumber = string.IsNullOrEmpty(BaseInstructionNumberText.text) ? 20 : int.Parse(BaseInstructionNumberText.text);
        GameSettings.InstructionNumberIncreasePerRound = string.IsNullOrEmpty(InstructionNumberIncreasePerRoundText.text) ? 8 : int.Parse(InstructionNumberIncreasePerRoundText.text);
        GameSettings.BaseInstructionTime = string.IsNullOrEmpty(BaseInstructionTimeText.text) ? 15 : int.Parse(BaseInstructionTimeText.text);
        GameSettings.InstructionTimeReductionPerRound = string.IsNullOrEmpty(InstructionTimeReductionPerRoundText.text) ? 2 : int.Parse(InstructionTimeReductionPerRoundText.text);
        GameSettings.InstructionTimeIncreasePerPlayer = string.IsNullOrEmpty(InstructionTimeIncreasePerPlayerText.text) ? 2 : int.Parse(InstructionTimeIncreasePerPlayerText.text);
        GameSettings.MinimumInstructionTime = string.IsNullOrEmpty(MinimumInstructionTimeText.text) ? 3 : int.Parse(MinimumInstructionTimeText.text);
        GameSettings.PlayerCount = (int)playerCountSlider.value;
        GameSettings.EasyPhoneInteractions = easyPhoneInteractions.isOn;
        GameSettings.PhoneInteractionProbability = string.IsNullOrEmpty(phoneInteractionText.text) ? 12 : 3*int.Parse(phoneInteractionText.text);

        s_Singleton.minPlayers = GameSettings.PlayerCount;
    }
    
    public void UpdatePlayerCountText()
    {
        playerCountText.text = "Player Count: " + playerCountSlider.value;
    }

    public void UpdateToggleText()
    {
        if(easyPhoneInteractions.isOn){
            toggleText.text = "On";
        }
        else{
            toggleText.text = "Off";
        }
    }
    
}

