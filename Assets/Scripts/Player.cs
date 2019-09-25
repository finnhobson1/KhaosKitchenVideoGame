using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

/*  GAMES DAY NFC CODES:
 *
 *  MILK:                     BEZoSotfgA==
 *  CHEESE:                   BH5nSotfgA==
 *  PASTA:                    BDNoSotfgA==
 *  LENTILS:                  BJJnSotfgA==
 *  WHISK:                    BG5oSotfgA==
 *  CHOPPING BOARD:           BFpoSotfgA==
 *  SPOON:                    BJloSotfgA==
 *  PLATE:                    BFtoSotfgA==
 *  GLASS:                    BG9oSotfgA==
 *  FOOD WASTE:               BH1nSotfgA==
 *  NTERGALACTIC\nBLACK HOLE: BINoSotfgA==
 *  PLASTIC:                  BIRoSotfgA== 
 *  SPARE 1:                  BJhoSotfgA==
 *  SPARE 2:                  
 */

public class Player : NetworkBehaviour
{
    //--------------------------------------------------------------------------------------------------------------
    // GLOBAL VARIABLES --------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    // Linked Controllers
    public GameController gameController;
    public InstructionController InstructionController;
    public CameraController cameraController;

    // UI Elements
    public Button button1, button2, button3, button4;
    public Button[] AllButtons;
    public Button nfcButton, micButton, shakeButton;
    public Text scoreText, instructionText, timerText, gpsText, roundScoreText, topChefText, countdownText, roundNumberText, nameText, micVolumeText, groupMessageText, gameOverText;
    public GameObject nfcPanel, micPanel, shakePanel, gameOverPanel, roundCompletePanel, roundStartPanel, shopPanel, groupMessagePanel, cameraPanel, instBar;
    public Text nfcText, micText, shakeText, cameraText;
    public GameObject nfcOkayButton, micOkayButton, shakeOkayButton;
    public GameObject fullScreenPanel;
    public Text fullScreenPanelText;
    public GameObject backgroundPanel;

    // Audio Variables
    public AudioClip[] correctActions;
    public AudioClip[] incorrectActions;
    public AudioClip[] badNoises;
    public AudioClip TenSecondCountdown;
    private AudioSource source;
    private int switcher = 0;
    private int failSwitch = 0;
    private int numberOfFails;
    private int numberOfButtonSounds;
    public float VolumeOfSoundEffects { get; set; }
    private float Volume = 2f;

    // Player information
    [SyncVar] public string PlayerUserName;
    [SyncVar] public Color PlayerColour;
    [SyncVar (hook = "UpdateScore") ] public int PlayerScore;
    [SyncVar] public int PlayerId;
    public uint PlayerNetworkID;
    
    // NFC Variables
    private List<Station> GoodStations = new List<Station>();
    private List<Station> BadStations = new List<Station>();
    [SyncVar] private string nfcValue = "";
    private string validNfc = "";
    [SyncVar] public string validNfcRace = "";

    // Gameplay variables
    [SyncVar (hook = "DisplayTopChef")] private string topChef;
    public int cameraColour = -1;
    public int playerCount;
    public int instTime;
    public bool easyPhoneInteractions = true;
    public MicListener micListener;
    public float instTimeLeft, instStartTime;
    private int scoreStreak = 0;
    private const int scoreStreakMax = 3;
    public bool isGamePaused;
    private bool isServe = false;
    private bool timerStarted = false;
    [SyncVar] public bool isSetupComplete;

    // Group activity variables
    [SyncVar] public bool isGroupActive;
    [FormerlySerializedAs("isGroupComplete")] [SyncVar] public bool isGroupActivityPlayerComplete;
    [SyncVar] public bool isShaking;
    [SyncVar] public int activityNumber = 0;
    [FormerlySerializedAs("isNFCRace")] [SyncVar] public bool isNFCRaceStarted;
    [SyncVar] public int nfcStation;
    [SyncVar] public bool IsNFCRaceCompleted;
    [SyncVar] public bool wait;


    //--------------------------------------------------------------------------------------------------------------
    // GETTERS FOR UNIT TESTING ------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    public string TopChef
    {
        get { return topChef; }
        set { topChef = value; }
    }

    public int ScoreStreak
    {
        get
        {
            return scoreStreak;
        }
    }

    public bool IsGamePaused
    {
        get
        {
            return isGamePaused;
        }
    }

    public bool IsServe
    {
        get
        {
            return isServe;
        }
    }


    //--------------------------------------------------------------------------------------------------------------
    // FUNCTIONS ---------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------


    private void Awake()
    {
        source = GetComponent<AudioSource>();
        numberOfFails = badNoises.Length;
        numberOfButtonSounds = correctActions.Length;
    }

    private void Start()
    {
        // Link Player to GameController + InstructionController.
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        InstructionController = GameObject.FindGameObjectWithTag("InstructionController").GetComponent<InstructionController>();
        Screen.orientation = ScreenOrientation.Portrait;
        transform.SetAsLastSibling();

        // Initialise Player information
        if (isLocalPlayer)
        {
            CmdSetNetworkID(PlayerNetworkID);
            CmdSetNameAndColour(PlayerUserName, PlayerColour);
            CmdSetPlayerReady();        
        }

        VolumeOfSoundEffects = Volume;
        nameText.text += PlayerUserName;
        nameText.color = PlayerColour;
        scoreText.color = PlayerColour;

        micListener.enabled = false;
        cameraController.enabled = false;

        // Start Game
        StartInstTimer();
    }


    private void Update()
    {
        if (wait) return;

        // Handle group activity
        groupMessagePanel.SetActive(isGroupActive);      
        if (isGroupActive)
        {
            if (isLocalPlayer)
            {
                CheckGroupActivity();
                TurnEverythingOff();
            }
        }

        // Early return if group activity, round over or game over
        if (groupMessagePanel.activeSelf) return;
        if (!isClient) return;
        if (gameOverPanel.activeSelf) return;
        if (roundCompletePanel.activeSelf)
        {
            TurnEverythingOff();
            return;
        }

        // Start instruction timer
        if (!timerStarted && gameController.isGameStarted)
        {
            StartInstTimer();
            timerStarted = true;
        }

        // During a round:
        else if (gameController.isGameStarted && gameController.roundTimeLeft > 0)
        {           
            UpdateInstTimeLeft();
            if (instTimeLeft < 0 && isLocalPlayer)
            {
                string tmp = GetBadNextNFC();
                validNfc = tmp;
                CmdFail(instructionText.text, tmp);
                PlayFailSound();
                StartInstTimer();
            }
            else
            {
                SetTimerText(instTimeLeft.ToString("F1"));
            }
            
            if (micPanel.activeSelf)
            {
                // Access MicListener
                if (micListener.MicLoudness > 0.15f)
                {
                    micPanel.SetActive(false);
                    micListener.enabled = false;
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            } 

            if (cameraPanel.activeSelf)
            {
                // Access CameraController
                bool cameraBool = false;
                if (cameraColour == 0) cameraBool = cameraController.red;
                else if (cameraColour == 1) cameraBool = cameraController.orange;
                else if (cameraColour == 2) cameraBool = cameraController.yellow;
                else if (cameraColour == 3) cameraBool = cameraController.green;
                else if (cameraColour == 4) cameraBool = cameraController.blue;

                if (cameraBool)
                {
                    cameraController.enabled = false;
                    cameraPanel.SetActive(false);
                    cameraController.red = false;
                    cameraController.blue = false;
                    cameraController.green = false;
                    cameraController.orange = false;
                    cameraController.yellow = false;
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

            if (shakePanel.activeSelf)
            {      
                // Access ShakeListener
                if (ShakeListener.shaking)
                {
                    shakePanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

            if (nfcPanel.activeSelf)
            {
                // Access NFCListener
                nfcValue = NfcCheck();
                if (validNfc.Equals(nfcValue))
                {
                    nfcPanel.SetActive(false);
                    CmdIncreaseScore();
                    StartInstTimer();
                }
            }

        }
        else
        {
            // Game Over
            SetTimerText("0");
            nfcPanel.SetActive(false);
            shakePanel.SetActive(false);
            micPanel.SetActive(false);
        }

    }


    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        SetTimerText("0");
    }


    public void SetPlayerId(int assignedId)
    {
        PlayerId = assignedId;
    }


    public int GetPlayerId()
    {
        return PlayerId;
    }


    private string NfcCheck()
    {
        string value = NFCListener.GetValue();
        
        //fridge
        if (value == "BEZoSotfgA==") return GoodStations[0].GetStationItem(0);
        if (value == "BH5nSotfgA==") return GoodStations[0].GetStationItem(1);

        //cupboard
        if (value == "BDNoSotfgA==") return GoodStations[1].GetStationItem(0);
        if (value == "BJJnSotfgA==") return GoodStations[1].GetStationItem(1);
        
        //prep
        if (value == "BG5oSotfgA==") return GoodStations[2].GetStationItem(0);
        if (value == "BFpoSotfgA==") return GoodStations[2].GetStationItem(1);
        
        //serve
        if (value == "BJloSotfgA==") return GoodStations[3].GetStationItem(0);
        if (value == "BFtoSotfgA==") return GoodStations[3].GetStationItem(1);

        //bin A
        if (value == "BG9oSotfgA==") return BadStations[0].GetStationItem(0);
        if (value == "BH1nSotfgA==") return BadStations[0].GetStationItem(1);
    
        //bin B
        if (value == "BINoSotfgA==") return BadStations[1].GetStationItem(0);
        if (value == "BIRoSotfgA==") return BadStations[1].GetStationItem(1);
        
        return value;
    }


    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }


    public void SetInstructionController(InstructionController instructionController)
    {
        InstructionController = instructionController;
    }


    public void SetActionButtons(string instruction, int i)
    {
        if (!isLocalPlayer) return;

        switch (i)
        {
            case 0:
                button1.GetComponentInChildren<Text>().text = instruction;
                break;
            case 1:
                button2.GetComponentInChildren<Text>().text = instruction;
                break;
            case 2:
                button3.GetComponentInChildren<Text>().text = instruction;
                break;
            case 3:
                button4.GetComponentInChildren<Text>().text = instruction;
                break;
            default:
                Console.WriteLine("ERROOOR");
                break;
        }
    }


    public void SetInstruction(String d)
    {
        if (!isLocalPlayer) return;
        instructionText.text = d;
        if (isGamePaused) return;
    }


    public string GetInstruction()
    {
        return instructionText.text;
    }
    

    [Command]
    public void CmdAction(string action)
    {
        InstructionController.CheckAction(action);
    }


    [Command]
    public void CmdFail(string action, string bin)
    {
        InstructionController.FailAction(action, bin);
        ResetScoreStreak();
    }


    [Command]
    public void CmdIncreaseScore()
    {
        gameController.IncreaseScore();
        PlayerScore++;
    }


    [Command]
    public void CmdIncreasePlayerScore()
    {
        PlayerScore++;
    }


    public void OnClickButton1()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button1.GetComponentInChildren<Text>().text, 0);
        }
    }


    public void OnClickButton2()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button2.GetComponentInChildren<Text>().text, 1);
        }
    }


    public void OnClickButton3()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button3.GetComponentInChildren<Text>().text, 2);
        }
    }


    public void OnClickButton4()
    {
        if (isLocalPlayer)
        {
            CheckInstruction(button4.GetComponentInChildren<Text>().text, 3);
        }
    }


    public void NfcClick(string nfcString)
    {
        if (isLocalPlayer)
        {
            CmdAction(nfcString);
        }
    }


    public void MicClick(string micString)
    {
        if (isLocalPlayer)
        {
            CmdAction(micString);
        }
    }


    public void ShakeClick(string shakeString)
    {
        if (isLocalPlayer)
        {
            CmdAction(shakeString);
        }
    }


    public void StartInstTimer()
    {
        instStartTime = gameController.CalculateInstructionTime();
        instTimeLeft = instStartTime;
    }


    private void UpdateInstTimeLeft()
    {
        if (isGamePaused)
        {
            //Reset timer
            instTimeLeft = instStartTime;
            instBar.GetComponent<RectTransform>().localScale = new Vector3(instTimeLeft / instStartTime, 1, 1);
        }
        else if (nfcPanel.activeSelf || micPanel.activeSelf || shakePanel.activeSelf || isGamePaused)
        {
            Debug.Log("Panel active, timer disabled.");
        }
        else
        {
            instTimeLeft -= Time.deltaTime;
            instBar.GetComponent<RectTransform>().localScale = new Vector3(instTimeLeft / instStartTime, 1, 1);
        }
    }


    private void SetTimerText(string text)
    {
        timerText.text = text;
    }


    public void SetNfcPanel(string text)
    {
        nfcPanel.SetActive(true);
        nfcText.text = text;
    }


    public void SetShakePanel(string text)
    {
        shakePanel.SetActive(true);
        shakeText.text = text;
    }


    public void SetMicPanel(string text)
    {
        micPanel.SetActive(true);
        micListener.enabled = true;
        micText.text = text;
    }


    public void SetCameraPanel(int colour, string text)
    {
        cameraController.enabled = true;
        cameraPanel.SetActive(true);
        cameraColour = colour;
        cameraText.text = text;
    }


    public void OnClickRefresh()
    {
        cameraController.enabled = false;
        cameraController.enabled = true;
    }


    public void OnClickCameraButton()
    {
        if (cameraPanel.activeInHierarchy)
        {
            cameraPanel.SetActive(false);
            cameraController.enabled = false;
        }
        else
        {
            cameraController.enabled = true;
            cameraPanel.SetActive(true);
        }
    }


    public void OnClickNfcButton()
    {
        if (isLocalPlayer)
        {
            nfcPanel.SetActive(false);
        }
    }


    public void OnClickMicButton()
    {
        if (isLocalPlayer)
        {
            micPanel.SetActive(false);
        }
    }


    public void IncreaseScoreStreak()
    {
        scoreStreak++;
    }


    public void ResetScoreStreak()
    {
        scoreStreak = 0;
    }


    public void OnClickShakeButton()
    {
        if (isLocalPlayer)
        {
            shakePanel.SetActive(false);
        }
    }


    public void ScoreStreakCheck()
    {
        if (scoreStreak >= scoreStreakMax)
        {
            isServe = true;
            String window = GetGoodNextNFC();
            validNfc = window;
            ResetScoreStreak();
            SetNfcPanel("Great Work!\nTap the " + window + "!\n\n(TAP ON " + window + " NFC)");
        }
    }


    public void PausePlayer()
    {
        isGamePaused = true;
    }


    public void UnpausePlayer()
    {
        isGamePaused = false;
    }


    private void CheckInstruction(string action, int buttonNumber)
    {
        CmdAction(action);
        if (InstructionController.ActiveInstructions.Contains(action))
        {
            CorrectButtonPress(buttonNumber);
        }

        else
        {
            WrongButtonPress(buttonNumber);
        }
    }


    private void CorrectButtonPress(int buttonNumber)
    {
        // Activate feedback on this button
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.green;
        CmdIncrementScore();
        
        PlayCorrectSound();
        CmdIncreasePlayerScore();
        
        StartCoroutine(ResetButtonColour(0.5f, buttonNumber));
    }


    private void WrongButtonPress(int buttonNumber)
    {
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.red;

        PlayIncorrectSound();

        StartCoroutine(ResetButtonColour(0.5f, buttonNumber));
    }


    private IEnumerator ResetButtonColour(float x, int buttonNumber)
    {
        yield return new WaitForSecondsRealtime(x);
        AllButtons[buttonNumber].GetComponent<Image>().color = Color.white;

    }


    public void PlayTenSecondCountdown()
    {
        Vibrate();
    }


    private void PlayFailSound()
    {
        source.PlayOneShot(badNoises[failSwitch], VolumeOfSoundEffects);
        failSwitch = (failSwitch + 1) % numberOfFails;
    }


    private void PlayCorrectSound()
    {
        source.PlayOneShot(correctActions[switcher], VolumeOfSoundEffects);
        switcher = (switcher + 1) % numberOfButtonSounds;
    }


    public void DisableOkayButtonsOnPanels()
    {
        nfcOkayButton.SetActive(false);
        micOkayButton.SetActive(false);
        shakeOkayButton.SetActive(false);
    }


    private void PlayIncorrectSound()
    {
        source.PlayOneShot(incorrectActions[switcher], VolumeOfSoundEffects);
        Vibrate();
        switcher = (switcher + 1) % numberOfButtonSounds;

    }


    [Command]
    public void CmdUpdateChefPrefab(int item)
    {
        // Allows Top Chef to purchase items to modify their chef prefab
        var chefs = GameObject.FindGameObjectsWithTag("ChefPrefab");
        foreach (GameObject chef in chefs)
        {
            if (chef.GetComponent<ChefController>().arrow.GetComponent<Image>().color == PlayerColour)
            {
                if (item == 1)
                {
                    GameObject roller = chef.GetComponent<ChefController>().roller;
                    if (roller.activeInHierarchy == false && PlayerScore >= 25)
                    {
                        roller.SetActive(true);
                        PlayerScore -= 25;
                        RpcCloseShop();
                    }
                    else RpcShopVibrate();
                }

                if (item == 2)
                {
                    GameObject ogreEars = chef.GetComponent<ChefController>().ogreEars;
                    Material ogreColour = chef.GetComponent<ChefController>().ogreColour;
                    List<GameObject> skin = chef.GetComponent<ChefController>().skin;
                    if (ogreEars.activeInHierarchy == false && PlayerScore >= 50)
                    {
                        ogreEars.SetActive(true);
                        foreach (GameObject s in skin)
                        {
                            s.GetComponent<MeshRenderer>().material = ogreColour;
                        }
                        PlayerScore -= 50;
                        RpcCloseShop();
                    }
                    else RpcShopVibrate();
                }

                if (item == 3)
                {
                    GameObject crown = chef.GetComponent<ChefController>().crown;
                    List<GameObject> hatParts = chef.GetComponent<ChefController>().hat;
                    if (crown.activeInHierarchy == false && PlayerScore >= 100)
                    {
                        crown.SetActive(true);
                        foreach (GameObject part in hatParts)
                        {
                            part.SetActive(false);
                        }
                        PlayerScore -= 100;
                        RpcCloseShop();
                    }
                    else RpcShopVibrate();
                }
            }
        }
    }


    [ClientRpc]
    void RpcCloseShop()
    {
        shopPanel.SetActive(false);
    }


    [ClientRpc]
    void RpcShopVibrate()
    {
        if (shopPanel.activeSelf) Vibrate();
    }


    [Command]
    public void CmdChangeHatColour()
    {
        var chefs = GameObject.FindGameObjectsWithTag("ChefPrefab");
        foreach (GameObject chef in chefs)
        {
            if (chef.GetComponent<ChefController>().arrow.GetComponent<Image>().color == PlayerColour)
            {
                //UPDATE HAT COLOUR
                List<GameObject> hatParts = chef.GetComponent<ChefController>().hat;
                foreach (GameObject part in hatParts)
                {
                    Material hatColour = new Material(part.GetComponent<MeshRenderer>().material);
                    hatColour.color = PlayerColour;
                    part.GetComponent<MeshRenderer>().material = hatColour;
                }
            }
        }
    }


    public void OnClickShopButton1()
    {
        CmdUpdateChefPrefab(1);
    }


    public void OnClickShopButton2()
    {
        CmdUpdateChefPrefab(2);
    }


    public void OnClickShopButton3()
    {
        CmdUpdateChefPrefab(3);
    }


    private void Vibrate()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }


    public void BackToMainMenu()
    {
        GameObject.FindGameObjectWithTag("lobbyBackground").SetActive(true);
        Application.Quit();
    }


    private void DisplayTopChef(string topChef)
    {
        topChefText.text = topChef;
    }


    [Command]
    public void CmdIncrementScore()
    {
        PlayerScore++;
    }
    

    [Command]
    public void CmdSetNameAndColour(string name, Color colour)
    {
        PlayerUserName = name;
        PlayerColour = colour;
    }
    

    [Command]
    public void CmdSetPlayerReady()
    {
        isSetupComplete = true;
    }


    [Command]
    public void CmdSetNetworkID(uint ID)
    {
        PlayerNetworkID = ID;
    }


    private void UpdateScore(int x)
    {
        scoreText.text = x.ToString();
        roundScoreText.text = x.ToString();
    }


    [Command]
    public void CmdSetShake(bool shake)
    {
        isShaking = shake;
    }


    [Command]
    public void CmdSetNFCRace(bool isNFCFinished)
    {
        IsNFCRaceCompleted = isNFCFinished;
        if (IsNFCRaceCompleted) validNfcRace = "";
    }
    

    private void CheckGroupActivity()
    {
        switch (activityNumber)
        {
            case 0:
                groupMessageText.text = "ALL HANDS ON DECK!\n\nLOOK AT THE MAIN SCREEN!";
                CmdSetShake(ShakeListener.shaking);
                break;
            case 1:
                nfcValue = NfcCheck();

                if (!isNFCRaceStarted)
                {
                    StartNFCRace();
                }
                else if (!IsNFCRaceCompleted && isNFCRaceStarted)
                {
                    CmdSetNFCRace(validNfcRace.Equals(nfcValue));
                }
                else
                {
                    groupMessageText.text = "DONE!";
                    wait = true;
                }
                break;            
            default:
                Debug.Log("Error with group activity number");
                break;
        }
    }


    public void GenerateGoodStation(List<List<string>> stations)
    {
        foreach (var station in stations)
        {
            GoodStations.Add(new Station(station));
        }
    }    
    

    public void GenerateBadStation(List<List<string>> stations)
    {
        foreach (var station in stations)
        {
            BadStations.Add(new Station(station));
        }
    }


    private string GetGoodNextNFC()
    {
        Random rand = new Random();
        int x = rand.Next(0, GoodStations.Count);
        return GoodStations[x].GetItem(nfcValue);
    }    
    

    private string GetBadNextNFC()
    {
        Random rand = new Random();
        int x = rand.Next(0, BadStations.Count);
        return BadStations[x].GetItem(nfcValue);
    }


    public void StartNFCRace()
    {
        nfcValue = NfcCheck();

        switch ( nfcStation )
        {
            case 0:
                validNfcRace = GoodStations[0].GetItem(nfcValue) ;
                break;
            case 1:
                validNfcRace = GoodStations[1].GetItem(nfcValue);
                break;
            case 2:
                validNfcRace = GoodStations[2].GetItem(nfcValue);
                break;
            case 3:
                validNfcRace = GoodStations[3].GetItem(nfcValue);
                break;
            case 4:
                validNfcRace = BadStations[0].GetItem(nfcValue);
                break;
            case 5:
                validNfcRace = BadStations[1].GetItem(nfcValue);
                break;
            default:
                validNfcRace = GoodStations[0].GetItem(nfcValue);
                break;
        }

        groupMessageText.text = "ALL HANDS ON DECK!\n\nLOOK AT THE MAIN SCREEN!";
        CmdSetValidNfcRace(validNfcRace);
        IsNFCRaceCompleted = false;
        isNFCRaceStarted = true;
    }


    [Command]
    public void CmdSetValidNfcRace(string tmp)
    {
        validNfcRace = tmp;
    }


    private void TurnEverythingOff()
    {
        nfcPanel.SetActive(false);
        shakePanel.SetActive(false);
        micPanel.SetActive(false);
        micListener.enabled = false;
        cameraPanel.SetActive(false);
        cameraController.enabled = false;
        cameraController.red = false;
        cameraController.blue = false;
        cameraController.green = false;
        cameraController.orange = false;
        cameraController.yellow = false;
    }

}

