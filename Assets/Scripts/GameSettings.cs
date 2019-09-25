using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class GameSettings
{
    
    /*XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
     * HOW TO USE:
     * 
     * 1. Set up static var with get and set in GameSettings Class (here)
     * 2. Create syncvar in Game Controller to link to
     * 3. Update LoadSettings function in GameController
     * 4. Create new input field within Settings Panel within the LobbyManager 
     *     (Can copy and paste existing ones - set content type to whatever needed)
     * 5. State a text object within the LobbyManager 
     *     (i.e. public Text exampleFieldText; )
     * 6. Link the input field text child to the new text object in LobbyManager (drag and drop)
     * 7. Update SetSettings, SetDefaultSettings and ResetSettingsToLast functions within LobbyManager
     *     (Don't forget text will be a string so much use conversion to relevant var)
     * 
     * XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
     */

    private static int baseInstructionNumber = 20;
    private static int instructionNumberIncreasePerRound = 8;
    private static int baseInstructionTime = 15;
    private static int instructionTimeReductionPerRound = 5;
    private static int instructionTimeIncreasePerPlayer = 2;
    private static int minimumInstructionTime = 3;
    private static int roundTime = 90;
    private static bool easyPhoneInteraction = true;
    private static int phoneInteractionProbability = 12;
    
    private static int pCount;
    
    public static int PlayerCount 
    {
        get
        {
            return pCount;
        }
        set
        {
            pCount = value;
        }
    }

    public static int BaseInstructionNumber { get; set;}

    public static int InstructionNumberIncreasePerRound { get; set; }

    public static int BaseInstructionTime { get; set; }

    public static int InstructionTimeReductionPerRound { get; set; }

    public static int InstructionTimeIncreasePerPlayer { get; set; }

    public static int MinimumInstructionTime { get; set; }

    public static int RoundTime { get; set; }

    public static bool EasyPhoneInteractions { get; set; }

    public static int PhoneInteractionProbability { get; set; }

    public static void SetDefaultValues()
    {
        PlayerCount = pCount;
        BaseInstructionNumber = baseInstructionNumber;
        InstructionNumberIncreasePerRound = instructionNumberIncreasePerRound;
        BaseInstructionTime = baseInstructionTime;
        InstructionTimeReductionPerRound = instructionTimeReductionPerRound;
        InstructionTimeIncreasePerPlayer = instructionTimeIncreasePerPlayer;
        MinimumInstructionTime = minimumInstructionTime;
        RoundTime = roundTime;
        EasyPhoneInteractions = easyPhoneInteraction;
        PhoneInteractionProbability = phoneInteractionProbability;

    }

}
