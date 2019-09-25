using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;


public class InstructionController : NetworkBehaviour
{
    //--------------------------------------------------------------------------------------------------------------
    // GLOBAL VARIABLES --------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------
    
    // Linked Controller
    public GameController GameController;
    
    // Possible Instructions + Text
    private static List<String> verbList = new List<string>(new string[] { "Grab", "Fetch", "Grate", "Grill", "Melt", "Serve", "Stir", "Chop", "Cut", "Mash", "Season", "Flambé", "Bake", "Fry", "Taste", "Microwave", "Tendorise", "Roast", "Cry Into", "Mince", "Juice", "Freeze", "Purée", "Sneeze On", "Dice", "Cube", "Boil", "Brine", "Sous Vide", "Slice", "Poach",  "Deep Fry", "Lick", "Inhale", "Smell" });
    private static List<String> nounList = new List<string>(new string[] { "Minced Beef", "Steak", "Pork Loin", "Ice Cream", "Strawberry", "Bannana", "Toast", "Chocolate", "Pasta", "Bacon", "Tomato", "Sugar", "Salt", "Lettuce", "Sauce", "Mustard", "Sausage", "Apple", "Orange", "Chicken", "Ice Cubes", "Cheese", "Chicken Nuggets", "Brie", "Cheddar", "Camembert", "Wine", "Beer", "Whiskey", "Vodka", "Wasabi", "Salmon", "Tuna", "Mushroom", "Lard", "Burger" });

    private static List<string> fridge = new List<string>( new string[] { "MILK IN THE FRIDGE", "CHEESE IN THE FRIDGE" } );
    private static List<string> cupboard = new List<string>( new string[] { "PASTA", "MICROWAVE" } );
    private static List<string> prep = new List<string>( new string[] { "DELIVEROO BAG", "CHOPPING BOARD" } );
    private static List<string> serve = new List<string>( new string[] { "THE HOB", "PLATE" } );
    
    private static List<string> binA = new List<string>( new string[] { "GLASS BIN", "FOOD WASTE" } );
    private static List<string> binB = new List<string>( new string[] { "SINK", "PLASTIC RECYCLING" } );
    
    private static List<string> WinnersList = new List<string>( new string[] {"WINNER!!","2nd","3rd","4th", "5th", "6th"});
    
    private static List<List<string>> GoodStations = new List<List<string>>{ fridge, cupboard, prep, serve };
    private static List<List<string>> BadStations = new List<List<string>>{ binA, binB };
    
    private static List<String> micInstructions = new List<string>(new string[] { "Waiters won't take the food out fast enough!\n Shout at them to work harder!\n\n(SHOUT INTO THE MIC)", "Your team are being useless!\n Shout some sense into them!\n\n(SHOUT INTO THE MIC)",
                                                                                  "Rats have been spotted in the kitchen!\n Scream to scare them away!\n\n(SHOUT INTO THE MIC)", " Whoops! You just set your chopping board on fire!\n Try to blow it out!\n\n(BLOW INTO THE MIC)", 
                                                                                  "The person you have a crush on just walked in!\n Shout at them to confess your love!\n\n(SHOUT INTO THE MIC)", "Whoopsie! You've just set yourself on fire!\n Blow the fire out!\n\n(BLOW INTO THE MIC)",
                                                                                  "The human race is on the brink of extinction!\n Scream in despair!\n\n(SHOUT, SCREAM OR CRY INTO THE MIC"});

    private static List<String> shakeInstructions = new List<string>(new string[] { "Chef underseasoned the dish!\nShake to salt the food!\n\n(SHAKE YOUR PHONE)", "The Queen has decided to dine here!\nBetter give her a royal wave!\n\n(SHAKE YOUR PHONE)",
                                                                                    "Food runner dropped the dish!\nShake some sense into the boy!\n\n(SHAKE YOUR PHONE)", "It's Virgin Pornstar Martini time!\nBetter shake that cocktail!\n\n(SHAKE YOUR PHONE)",
                                                                                    "Pan set on fire!\nShake to put it out!\n\n(SHAKE YOUR PHONE)", "We are ten years away from irreversible climate damage!\n Shake to ignore\n\n(SHAKE YOUR PHONE)",
                                                                                    "Your arch nemisis just walked in!\nShake your fist at them angrily!\n\n(SHAKE YOUR PHONE)"});

    private static List<String> cameraInstructionText = new List<string>(new string[] { "FIND THE TOMATO!", "FIND THE ORANGE!", "FIND THE BANANAS!", "FIND THE APPLE!", "FIND THE EU FLAG TO REVOKE ARTICLE 50!" });

    // Active actions + instructions
    private SyncListString activeButtonActions = new SyncListString();
    private SyncListString activeInstructions = new SyncListString();

    // Gameplay Variables
    public List<Player> Players;
    public int NumberOfButtons { get; set; }
    public int PlayerCount { get; set; }
    [SyncVar] public bool isRoundPaused = false;
    [SyncVar] public bool isLastActionOfRound = false;
    [SyncVar] public bool SetupFinished = false;


    //--------------------------------------------------------------------------------------------------------------
    // GETTERS + SETTERS FOR UNIT TESTING --------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    public bool IsLastActionOfRound
    {
        get { return isLastActionOfRound; }
    }

    public SyncListString ActiveButtonActions
    {
        get { return activeButtonActions; }
        set { activeButtonActions = value; }
    }
    public SyncListString ActiveInstructions
    {
        get { return activeInstructions; }
        set { activeInstructions = value; }
    }


    //--------------------------------------------------------------------------------------------------------------
    // FUNCTIONS ---------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------


    // Initialise InstructionController, called from GameController
    public void ICStart(int playerCount, int numberOfButtons, List<Player> players, GameController gameController)
    {
        // Assign values from GC
        PlayerCount = playerCount;
        NumberOfButtons = numberOfButtons;
        Players = players;

        if (isServer)
        {
            // Create synced list of executables, one for each player action button.
            SelectButtonActions();
            // Select one instruction per player from ActionButton List.
            SetFirstInstructions(); 
            SetupFinished = true;
        }

        while (!SetupFinished)
        {
            Debug.Log("Waiting for GC to finish setup");
        }
        
        // Assign actions and instructions to each player.
        foreach (var player in Players)
        {
            for (int i = 0; i < NumberOfButtons; i++)
            {
                string action = activeButtonActions[player.PlayerId * numberOfButtons + i];
                player.SetActionButtons(action, i);

                if (!isServer) continue;
            }
            
            player.GenerateGoodStation(GoodStations);
            player.GenerateBadStation(BadStations);

            string instruction = ActiveInstructions[player.PlayerId];
            player.SetInstruction(instruction);
        }          
    }


    // Reset the active instructions and buttons.
    [Server]
    public void ResetIC()
    {
        SelectButtonActions();
        SetFirstInstructions();
    }


    public string GetPositionWord(int i)
    {
        if (i >= PlayerCount) return "error";
        return WinnersList[i];
    }


    [ClientRpc]
    public void RpcResetPlayers()
    {
        //Assign actions and instructions to each player.
        foreach (var player in Players)
        {
            for (int i = 0; i < NumberOfButtons; i++)
            {
                string action = activeButtonActions[player.PlayerId * NumberOfButtons + i];
                player.SetActionButtons(action, i);
            }
            string instruction = ActiveInstructions[player.PlayerId];
            player.SetInstruction(instruction);
        }
    }


    // Creates synced list of executable strings, one for each action button
    public void SelectButtonActions()
    {
        ActiveButtonActions.Clear();
        bool duplicate;
        int verbNo, nounNo;
        string text;
                
        for (int i = 0; i < PlayerCount*NumberOfButtons; i++)
        {
            duplicate = true;
            while (duplicate)
            {
                verbNo = UnityEngine.Random.Range(0, verbList.Count-1);
                nounNo = UnityEngine.Random.Range(0, nounList.Count-1); 
                text = verbList[verbNo] + " " + nounList[nounNo];

                if (ActiveButtonActions.Contains(text)) continue;
                ActiveButtonActions.Add(text);
                duplicate = false;
            }
        }
    }


    // Randomly select an instruction per player from ActiveButtonActions and add to ActiveInstruction.
    private void SetFirstInstructions()
    {
        ActiveInstructions.Clear();
        for (int i = 0; i < PlayerCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count); 
            while (ActiveInstructions.Contains(ActiveButtonActions[randomIndex]))
            {
                randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
            }
            ActiveInstructions.Add(ActiveButtonActions[randomIndex]);
        }
    }
    

    // Function called from player when action attempted.
    // If action is a current instruction then generate new instruction and order GC to handle score.
    [Server]
    public void CheckAction(string action)
    {
        if (isRoundPaused) return;
        bool match = false;

        // When an action button is pressed by a player-client, check if action matches an active instruction.
        for (int i = 0; i < ActiveInstructions.Count; i++)
        {
            // If match, increment score.
            if (action != ActiveInstructions[i]) continue;

            match = true;

            GameController.CheckAction(i);
            Players[i].PlayerScore++;

            PickNewInstruction(i, action);
            if (!isLastActionOfRound)
            {
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcStartInstTimer(i);

                int rand = UnityEngine.Random.Range(0, 5);
                if (rand == 0)
                {
                    rand = UnityEngine.Random.Range(0, micInstructions.Count);
                    RpcSetMicPanel(i, micInstructions[rand]);
                }
                if (rand == 1)
                {
                    rand = UnityEngine.Random.Range(0, shakeInstructions.Count);
                    RpcSetShakePanel(i, shakeInstructions[rand]);
                }
                else if (rand == 2)
                {
                    rand = UnityEngine.Random.Range(1, cameraInstructionText.Count);
                    RpcSetCameraPanel(i, rand, cameraInstructionText[rand]);
                }
            }
        }

        if (!match)
        {
            GameController.fireCount++;
            GameController.customerSatisfaction -= 5;
        }
    }
    

    // Checks if failed action is a current active instruction, and if so pick new instruction and orders NFC bin.
    public void FailAction(string action, string bin)
    {
        for (int i = 0; i < ActiveInstructions.Count; i++)
        {
            if (action == ActiveInstructions[i])
            {
                GameController.RpcResetScoreSteak(i);
                GameController.IncreaseFireCount();
                GameController.customerSatisfaction -= 5;

                PickNewInstruction(i, action);
                RpcUpdateInstruction(ActiveInstructions[i], i);
                RpcSetNfcPanel(i, "You missed an order! " + System.Environment.NewLine
                                                           + "Run to the "+ bin +"!" + System.Environment.NewLine + System.Environment.NewLine
                                                           + "(TAP ON "+ bin +" NFC)");
            }
        }
    }


    // New Instruction selected from set of ActiveButtonActions, with no duplicate ensured.
    public void PickNewInstruction(int index, string action)
    {
        int randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
        while (ActiveInstructions.Contains(ActiveButtonActions[randomIndex]))
        {
            randomIndex = UnityEngine.Random.Range(0, ActiveButtonActions.Count);
        }
        
        ActiveInstructions[index] = ActiveButtonActions[randomIndex];
    }
    

    [ClientRpc]
    public void RpcUpdateInstruction(String newInstruction, int playerID)
    {
        Players[playerID].SetInstruction(newInstruction);
    }


    [ClientRpc]
    public void RpcUpdateButtons(int playerID)
    {
        for (int i = 0; i < NumberOfButtons; i++)
        {
            Players[i].SetActionButtons(playerID.ToString(), i);
        }
    }


    [ClientRpc]
    public void RpcSetActionButton(int playerID, string action, int buttonNumber)
    {
        foreach (var player in Players)
        {
            if (player.PlayerId != playerID) return;
            player.SetActionButtons(action, buttonNumber);
        }
    }


    [ClientRpc]
    public void RpcStartInstTimer(int playerID)
    {
        Players[playerID].StartInstTimer();
    }
    

    [ClientRpc]
    public void RpcSetNfcPanel(int playerID, string text)
    {
        Players[playerID].SetNfcPanel(text);
    }


    [ClientRpc]
    public void RpcSetMicPanel(int playerID, string text)
    {
        Players[playerID].SetMicPanel(text);
    }


    [ClientRpc]
    public void RpcSetShakePanel(int playerID, string text)
    {
        Players[playerID].SetShakePanel(text);
    }


    [ClientRpc]
    public void RpcSetCameraPanel(int playerID, int colour, string text)
    {
        Players[playerID].SetCameraPanel(colour, text);
    }


    [ClientRpc]
    public void RpcShowPaused()
    {
        foreach (var player in Players)
        {
            player.SetInstruction(player.PlayerUserName + " is paused");
        }
    }


    public void PauseIC()
    {
        isRoundPaused = true;
    }
    

    public void UnPauseIC()
    {
        isRoundPaused = false;
    }


    public void PenultimateAction(bool fromGC)
    {
        isLastActionOfRound = fromGC;
    }
}



