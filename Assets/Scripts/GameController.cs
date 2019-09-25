using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using Random = System.Random;



public class GameController : NetworkBehaviour
{
    //--------------------------------------------------------------------------------------------------------------
    // GLOBAL VARIABLES --------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    // Linked Controllers
    public InstructionController InstructionController;
    public AnimationController animationController;
    private GameStateHandler gameStateHandler;
    public MusicPlayer MusicPlayer;

    // UI Elements
    public Text scoreText, roundTimerText, scoreBarText, roundNumberText;
    public List<Text> groupDisplayNames = new List<Text>();
    public List<Text> groupDisplayTasks = new List<Text>();
    public List<Text> playerNames = new List<Text>();
    public GameObject roundTimerBar, gameOverText, backButton, lobbyTopPanel,groupShakePanel, groupRacePanel, scoreBar;
    public Image stars;

    // Gameplay variables
    public List<Player> playerList = new List<Player>();
    public List<string> raceWinnersList = new List<string>();
    private List<string> UserNames = new List<string>();
    [SyncVar] public int score = 0;
    [SyncVar] private int roundScore = 0;
    [SyncVar] public int roundNumber = 1;
    [SyncVar] public bool isRoundPaused;
    [SyncVar] public bool isGameStarted;
    public bool playersInitialised;
    public bool spawnChefs = false;
    private bool isGameOver;
    private bool step;
    private bool tenSec = false;
    private int stationCount = 6;
    [SyncVar] public int fireCount = 0;
    [SyncVar] private int buzzer;
    [SyncVar] public string currentTopChef;
    [SyncVar] public float customerSatisfaction = 60;
    public float pointMultiplier;
    private static int numberOfButtons = 4;
    public int roundStartScore;
    public int roundMaxScore;
   
    // Group activity variables
    private bool isGroupActiviy = true;
    [SyncVar] public bool isGroupDone;
    private bool isGroupActivityEnabled = true;
    [FormerlySerializedAs("startGroupActivity")] [SyncVar] public bool groupActivityStarted;
    public int numberOfGroupActivities = 2;
    [SyncVar] public int activityNumber = 1;
    public bool printCompleted;
   
    // Game balance + settings variables
    [SyncVar] public float roundTimeLeft;
    [SyncVar] public int roundStartTime;
    [SyncVar] public int instructionStartTime;
    [SyncVar] public int BaseInstructionNumber;
    [SyncVar] public int InstructionNumberIncreasePerRound;
    [SyncVar] public int BaseInstructionTime;
    [SyncVar] public int InstructionTimeReductionPerRound;
    [SyncVar] public int InstructionTimeIncreasePerPlayer;
    [SyncVar] public int MinimumInstructionTime;
    [SyncVar] public int playerCount;
    [SyncVar] public bool easyPhoneInteractions;



    //--------------------------------------------------------------------------------------------------------------
    // GETTERS FOR UNIT TESTING ------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    public int Score
    {
        get
        {
            return score;
        }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    public int RoundNumber
    {
        get
        {
            return roundNumber;
        }
    }

    public int FireCount
    {
        get
        {
            return fireCount;
        }
    }

    public bool IsRoundPaused
    {
        get
        {
            return isRoundPaused;
        }
    }

    public bool IsGameStarted
    {
        get
        {
            return isGameStarted;
        }
    }

    public float RoundTimeLeft
    {
        get
        {
            return roundTimeLeft;
        }
    }

    public int RoundStartTime
    {
        get
        {
            return roundStartTime;
        }
    }

    public int PlayerCount
    {
        get
        {
            return playerCount;
        }
    }

    public bool PlayersInitialised
    {
        get
        {
            return playersInitialised;
        }
    }



    //--------------------------------------------------------------------------------------------------------------
    // FUNCTIONS ---------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------


    private void Start()
    {
        // Begin game set up after 2 seconds
        StartCoroutine(SetupGame(2));
        GameObject.FindGameObjectWithTag("lobbyBackground").SetActive(false);

        // Only show restaurant scene on the central server screen
        if (isServer)
        {
            GetComponentInChildren<Canvas>().enabled = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animation>().Play();
            GameObject.FindGameObjectWithTag("LobbyTopPanel").SetActive(false);
        }
    }


    private IEnumerator SetupGame(int x)
    {
        yield return new WaitForSecondsRealtime(x);

        // Signal AnimationController to spawn chef prefabs
        spawnChefs = true;

        // Load settings from the main menu
        if (isServer)
        {
            LoadSettings();
        }

        // Create list of players
        var players = FindObjectsOfType<Player>();

        // Initialise players
        int playerIndex = 0;
        foreach (Player p in players)
        {
            playerList.Add(p);
            if (isServer) p.SetGameController(this);
            p.SetInstructionController(InstructionController);
            p.SetPlayerId(playerIndex);
            p.instStartTime = CalculateInstructionTime();
            p.playerCount = playerCount;
            p.PlayerScore = 0;
            if (!easyPhoneInteractions) p.DisableOkayButtonsOnPanels();

            // Display Chefs on Duty on central server screen
            if (isServer)
            {
                UserNames.Add(p.PlayerUserName);
                playerNames[playerIndex].text = p.PlayerUserName;
                playerNames[playerIndex].color = p.PlayerColour;
            }
            playerIndex++;
        }

        PlayersInitialisedFromStart();

        InstructionController.ICStart(playerCount, numberOfButtons, playerList, this);

        if (isServer)
        {
            // Instantiate GameStateHandler object on the server to hold gamestate data
            gameStateHandler = new GameStateHandler(UserNames);  

            // Begin gameplay sequence
            StartCoroutine(RoundCountdown(10, "3"));
            StartCoroutine(RoundCountdown(11, "2"));
            StartCoroutine(RoundCountdown(12, "1"));
            StartCoroutine(StartRound(13));
            StartCoroutine(StartGame(13));
        }
    }


    private IEnumerator StartGame(int x)
    {
        yield return new WaitForSecondsRealtime(x);

        StartRoundTimer();
        UpdateScoreBar();
        isGameStarted = true;
        Debug.Log("Game Started");
    }


    private void Update()
    {
        if (isGameStarted)
        {
            // Show gameplay information on central server screen
            scoreText.text = score.ToString();
            stars.fillAmount = customerSatisfaction / 100;
            roundNumberText.text = roundNumber.ToString();
            UpdateRoundTimeLeft();

            if (isServer)
            {
                // Initiate group activity
                int scoreRemaining = (roundMaxScore) - (roundScore - roundStartScore);
                if ((score % (30 * playerCount) == 50) && isGroupActiviy && isGroupActivityEnabled && (scoreRemaining > playerCount) ) 
                {
                    InitiateGroupActivity();
                    printCompleted = false;
                }

                // Check if group acitivity is completed
                else if (groupActivityStarted)
                {
                    if (activityNumber == 1)
                    {
                        DisplayGroupNfcActivityInstruction();
                    }
                    else if (activityNumber == 0) {DisplayGroupShakeActivityInstruction();}

                    CheckGroupActivity();
                }

                else ResetGroup();
            }

            if (roundMaxScore - roundScore <= 1)
            {
                PenultimateAction(true);
            }

            // Run countdown audio and vibration sequence
            if (roundTimeLeft <= 10.4 && roundTimeLeft > 0 && isServer && !isGameOver && !isRoundPaused)
            {
                if (!tenSec)
                {
                    tenSec = true;
                    RpcTenCount();
                    MusicPlayer.PlayTenSecondCountdown();
                }
                else
                {
                    if (roundTimeLeft > 5 && (buzzer % 100) == 0)
                    {
                        RpcTenCount();
                    }
                    else if (roundTimeLeft < 5 && buzzer % 30 == 0)
                    {
                        RpcTenCount();
                    }
                    buzzer++;
                }
            }

            // Check if round is completed
            if (IsRoundComplete())
            {
                OnRoundComplete();
            }

            // Check if game over
            else if (roundTimeLeft <= 0 || customerSatisfaction <= 0)
            {
                if (customerSatisfaction <= 0)
                {
                    foreach (Player p in playerList)
                    {
                        p.gameOverText.text = "TERRIBLE REVIEWS!\n\nGAME OVER!";
                    }
                }

                if (isServer) RpcGameOver();
                SetTimerText("0");
                roundTimerBar.GetComponent<RectTransform>().localScale = new Vector3(0, 1.09f, 1.09f);
                gameOverText.transform.SetAsLastSibling();
                gameOverText.SetActive(true);
                backButton.SetActive(true);

                if (!isGameOver)
                {
                    PlayGameOver();
                    GameOver();
                }

                groupShakePanel.SetActive(false);
                groupRacePanel.SetActive(false);
            }

            // Display time remaining in round
            else
            {
                SetTimerText(roundTimeLeft.ToString("F2"));
            }
        }
    }


    [ClientRpc]
    private void RpcGameOver()
    {
        foreach (Player p in playerList) p.GameOver();
    }


    public void OnClickBack()
    {
        RpcQuitGame();
        Application.Quit();
    }


    [ClientRpc]
    public void RpcQuitGame()
    {
        foreach (Player p in playerList)
        {
            p.BackToMainMenu();
        }
    }


    [ClientRpc]
    public void RpcIncreaseScoreStreak(int playerID)
    {
        playerList[playerID].IncreaseScoreStreak();
    }


    [ClientRpc]
    public void RpcResetScoreSteak(int playerID)
    {
        playerList[playerID].ResetScoreStreak();
    }


    [ClientRpc]
    public void RpcScoreStreakCheck(int playerID)
    {
        playerList[playerID].ScoreStreakCheck();
    }


    [Server]
    public void CheckAction(int i)
    {
        if (isRoundPaused) return; //Do not check if round paused.
        IncreaseScore();
        RpcIncreaseScoreStreak(i);
        RpcScoreStreakCheck(i);
    }


    [Server]
    public void IncreaseFireCount()
    {
        fireCount++;
    }


    [Server]
    public void IncreaseScore()
    {
        score += 10;
        roundScore++;
        customerSatisfaction += 2.0f;
        UpdateScoreBar();
    }


    public void StartRoundTimer()
    {
        roundTimeLeft = roundStartTime;
    }


    private void UpdateRoundTimeLeft()
    {
        roundTimeLeft -= Time.deltaTime;
        if (roundTimeLeft >= 0) roundTimerBar.GetComponent<RectTransform>().localScale = new Vector3(roundTimeLeft / roundStartTime, 1.09f, 1.09f);
    }


    public void PlayersInitialisedFromStart()
    {
        playersInitialised = true;
    }


    private void SetTimerText(string text)
    {
        roundTimerText.text = text;
    }


    private void UpdateScoreBar()
    {
        scoreBarText.text = (roundScore - roundStartScore).ToString() + " / " + roundMaxScore.ToString();
        scoreBar.GetComponent<RectTransform>().localScale = new Vector3((float)(roundScore - roundStartScore) / roundMaxScore, 1.09f, 1.09f);
    }


    private bool IsRoundComplete()
    {
        return (roundScore >= roundMaxScore);
    }


    private void OnRoundComplete()
    {
        //Only call this function once per round completion.
        if (!isServer || isRoundPaused) return; 

        // Reset round variables
        buzzer = 0;
        tenSec = false;
        roundNumber++;
        UpdateGamestate();
        isRoundPaused = true;

        RoundPaused();
        fireCount = 0;
        CancelInvoke();
        PauseMusic();
        PlayRoundBreakMusic();
        RpcPausePlayers();

        foreach (Player p in playerList)
        {
            p.instStartTime = CalculateInstructionTime();
        }

        ReadyInstructionController();

        // Begin next round
        StartCoroutine(PlayXCountAfterNSeconds(8, 2));
        StartCoroutine(StartNewRoundAfterXSeconds(8));
        StartCoroutine(RoundCountdown(9, "2"));
        StartCoroutine(RoundCountdown(10, "1"));
        StartCoroutine(StartRound(11));
    }


    public void RoundPaused()
    {
        isRoundPaused = true;
    }


    private IEnumerator Wait(float n)
    {
        yield return new WaitForSecondsRealtime(n);
    }


    private IEnumerator RoundCountdown(int n, string x)
    {
        yield return new WaitForSecondsRealtime(n);
        int count = 1;
        Int32.TryParse(x, out count);
        PlayCountDown(count - 1);
        RpcCountdown(x);
    }


    [ClientRpc]
    private void RpcCountdown(string x)
    {
        foreach (Player p in playerList) p.countdownText.text = x;
    }


    private IEnumerator StartNewRoundAfterXSeconds(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        RpcStartNewRound();
    }


    [ClientRpc]
    private void RpcStartNewRound()
    {
        foreach (Player p in playerList)
        {
            p.roundNumberText.text = roundNumber.ToString();
            p.countdownText.text = "3";
            p.roundStartPanel.SetActive(true);
            p.roundCompletePanel.SetActive(false);
            p.shopPanel.SetActive(false);
        }
    }


    private IEnumerator StartRound(int x)
    {
        yield return new WaitForSecondsRealtime(x);
        Debug.Log("StartRound");
        var players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            playerNames[i].text = players[i].PlayerUserName;
            playerNames[i].color = players[i].PlayerColour;
            players[i].SetPlayerId(i);
        }
        PlayRoundMusic();
        ResetPlayers();
        ResetServer();
        RpcUnpausePlayers();
        isRoundPaused = false;
        PenultimateAction(false);
        roundMaxScore = CalculateInstructionNumber();
        if (customerSatisfaction < 60) customerSatisfaction = 60;
        InvokeRepeating("DecreaseCustomerSatisfaction", 1.0f, 1.0f);
        UpdateScoreBar();
    }


    private void ResetServer()
    {
        Debug.Log("RESET SERVER");
        roundScore = 0;
        UpdateScoreBar();
        StartRoundTimer();
    }


    [ClientRpc]
    public void RpcPausePlayers()
    {
        foreach (var player in playerList)
        {
            player.roundCompletePanel.SetActive(true);
            if (player.topChefText.text == "YOU!!")
            {
                player.shopPanel.SetActive(true);
            }
            player.PausePlayer();
        }
    }


    [ClientRpc]
    public void RpcUnpausePlayers()
    {
        foreach (var player in playerList)
        {
            player.roundCompletePanel.SetActive(false);
            player.roundStartPanel.SetActive(false);
            player.UnpausePlayer();
        }
    }


    private void ReadyInstructionController()
    {
        InstructionController.PauseIC();
        InstructionController.RpcShowPaused();
        InstructionController.ResetIC();
    }


    private void ResetPlayers()
    {
        InstructionController.RpcResetPlayers();
        InstructionController.UnPauseIC();
    }


    private void PenultimateAction(bool action)
    {
        InstructionController.PenultimateAction(action);
    }


    private void UpdateGamestate()
    {
        // Store round info
        gameStateHandler.OnRoundComplete(score);

        // Calculate Top Chef
        int topScore = 0;
        string topChef = null;
        var players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player.PlayerScore > topScore)
            {
                topScore = player.PlayerScore;
                topChef = player.PlayerUserName;
            }
        }
        currentTopChef = topChef;
        RpcSetTopChef(topChef);
    }


    public void PrintOut(int buttonNumber)
    {
        Debug.Log(buttonNumber);
    }


    private int CalculateInstructionNumber()
    {
        float temp = ((BaseInstructionNumber + InstructionNumberIncreasePerRound * (roundNumber - 1))
                      * ((12f - (playerCount - 2f)) / 12f));

        int round = (int)Math.Ceiling(temp);

        return round > 0 ? round : 1; ;
    }


    public int CalculateInstructionTime()
    {
        int temp = BaseInstructionTime
                    - (roundNumber - 1) * InstructionTimeReductionPerRound
                    + (playerCount - 2) * InstructionTimeIncreasePerPlayer;

        return temp > MinimumInstructionTime ? temp : MinimumInstructionTime;
    }


    private void LoadSettings()
    {
        roundStartTime = GameSettings.RoundTime;
        playerCount = GameSettings.PlayerCount;
        easyPhoneInteractions = GameSettings.EasyPhoneInteractions;
        BaseInstructionNumber = GameSettings.BaseInstructionNumber;
        InstructionNumberIncreasePerRound = GameSettings.InstructionNumberIncreasePerRound;
        BaseInstructionTime = GameSettings.BaseInstructionTime;
        InstructionTimeReductionPerRound = GameSettings.InstructionTimeReductionPerRound;
        InstructionTimeIncreasePerPlayer = GameSettings.InstructionTimeIncreasePerPlayer;
        MinimumInstructionTime = GameSettings.MinimumInstructionTime;
    }


    private void DecreaseCustomerSatisfaction()
    {
        customerSatisfaction -= 1;
        if (customerSatisfaction > 100) customerSatisfaction = 100;
        if (customerSatisfaction < 0) customerSatisfaction = 0;
    }


    private IEnumerator PlayXCountAfterNSeconds(int n, int x)
    {
        yield return new WaitForSecondsRealtime(n);
        PauseMusic();
        PlayCountDown(x);
    }


    [Server]
    private void PlayRoundBreakMusic()
    {
        MusicPlayer.PlayRoundBreak();
    }


    [Server]
    private void PlayRoundMusic()
    {
        MusicPlayer.StartRoundMusic();
    }


    [Server]
    private void PlayCountDown(int x)
    {
        MusicPlayer.PlayCountDown(x);
    }


    [Server]
    private void PauseMusic()
    {
        MusicPlayer.PauseMusic();
    }


    [Server]
    private void PlayGameOver()
    {
        MusicPlayer.PlayGameOver();
    }


    public void GameOver()
    {
        isGameOver = true;
    }
    

    [ClientRpc]
    private void RpcSetTopChef(string topChef)
    {
        Debug.Log("TOP CHEF = " + topChef);
        foreach (var player in playerList)
        {
            if (topChef != null)
            {
                if (topChef == player.PlayerUserName)
                {
                    player.topChefText.text = "YOU!!";
                }
                else player.topChefText.text = topChef;
            }
        }
    }

       
    [ClientRpc]
    public void RpcSetGroupActivity(bool active)
    {
        foreach (var player in playerList)
        {
            player.isGroupActive = active;
        }
    }


    private void CheckGroupShake()
    {
        bool allReady = true;
        foreach (var player in playerList)
        {
            allReady &= player.isShaking;
        }

        if (allReady)
        {
            GroupActivityCompleted();
        }
    }


    private void CheckGroupNFCRace()
    {
        foreach (var player in playerList)
        {
            if (player.IsNFCRaceCompleted && !raceWinnersList.Contains(player.PlayerUserName))
            {
                raceWinnersList.Add(player.PlayerUserName);
               groupDisplayTasks[playerList.IndexOf(player)].text = InstructionController.GetPositionWord(raceWinnersList.IndexOf(player.PlayerUserName));
            }
        }

        if (raceWinnersList.Count == playerCount)
        {
            foreach (var player in playerList)
            {
                for (int i = 0; i < raceWinnersList.Count; i++)
                {
                    if (player.PlayerUserName == raceWinnersList[i])
                    {
                        int scoreAdjustment = (5 * (playerCount - i));
                        player.PlayerScore += scoreAdjustment;
                    }
                }
            }
            GroupActivityCompleted();
        }
    }


    private void IncrementGroupActivity()
    {
        activityNumber = (activityNumber + 1) % numberOfGroupActivities;
    }


    private void RpcUpdateActivityNumber(int number)
    {
        foreach (var player in playerList)
        {
            if (player != null) player.activityNumber = activityNumber;
            else Debug.Log("Not Player at UpdateActivityNumber");
        }
    }


    [Server]
    private void InitiateGroupActivity()
    {
        Debug.Log("Group Activity Initiated");
        Random rand = new Random();
        RpcNfcRaceAssignStation(rand.Next(0, stationCount));
        groupActivityStarted = true;
        RpcSetGroupActivity(true);
        isGroupActiviy = false;
    }


    [Server]
    private void CheckGroupActivity()
    {

        switch (activityNumber)
        {
            case 0:
                CheckGroupShake();
                break;

            case 1:
                //NFC group race
                CheckGroupNFCRace();
                break;

            default:
                Debug.Log("Error with Group Activity Number");
                break;
        }
    }


    [ClientRpc]
    private void RpcNfcRaceAssignStation(int i)
    {
        foreach (var player in playerList)
        {
            player.nfcStation = i;
            i = (i + 1) % stationCount;
        }
    }


    [Server]
    private void ResetGroup()
    {
        RpcResetGroupActivity();
    }


    public void GroupActivityCompleted()
    {
        Debug.Log("Group Activity Completed by All Players");

        groupActivityStarted = false;
        RpcResetGroupActivity();

        foreach (var player in playerList)
        {
            player.isNFCRaceStarted = false;
            player.IsNFCRaceCompleted = false;
        }

        isGroupActiviy = true;
        raceWinnersList = new List<string>();
        raceWinnersList.Clear();
        for (int i = 0; i < playerCount; i++) IncreaseScore();
        StartCoroutine(LeaveUpLeaderboard(5));
        groupShakePanel.SetActive(false);
        IncrementGroupActivity();
    }


    [ClientRpc]
    public void RpcResetGroupActivity()
    {
        foreach (var player in playerList)
        {
            player.instTime = instructionStartTime;
            player.isGroupActive = false;
            player.isNFCRaceStarted = false;
            player.IsNFCRaceCompleted = false;
            player.wait = false;
            player.activityNumber = activityNumber;
        }
    }


    private void DisplayGroupNfcActivityInstruction()
    {
        groupRacePanel.SetActive(true);
        for (int i = 0; i < playerCount; i++)
        {
            if (!playerList[i].IsNFCRaceCompleted)
            {
                groupDisplayNames[i].text = playerList[i].PlayerUserName;
                groupDisplayNames[i].color = playerList[i].PlayerColour;
                groupDisplayTasks[i].text = playerList[i].validNfcRace;
                groupDisplayTasks[i].color = playerList[i].PlayerColour;
            }
        }
    }
    

    private void DisplayGroupShakeActivityInstruction()
    {
        groupShakePanel.SetActive(true);
    }


    IEnumerator LeaveUpLeaderboard(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        groupRacePanel.SetActive(false);
    }


    [ClientRpc]
    private void RpcTenCount()
    {
        foreach (Player p in playerList) p.PlayTenSecondCountdown();
    }

}
