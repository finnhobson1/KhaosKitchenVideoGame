using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Hold and handle gameplay data
 */
public class GameStateHandler
{
    public int RoundNumber { get; set; }
    public int OverallTeamScore
    {
        get
        {
            return overallTeamScore;
        }
        set
        {
            overallTeamScore = value;
        }
    }

    public int GroupActivityNumber { get; set; }
    private List<string> userNames = new List<string>();
    private Dictionary<string, int> userProfiles = new Dictionary<string, int>();
    private int overallTeamScore;

    public List<string> UserNames
    {
        get { return userNames; }
        set { userNames = value; }
    }
    public Dictionary<string, int> UserProfiles
    {
        get { return userProfiles; }
        set { userProfiles = value; }
    }


    public GameStateHandler(List<string> userNames)
    {
        RoundNumber = 0;
        OverallTeamScore = 0;
        UserNames = userNames;

        //Set each players initial score to 0
        foreach (var userName in userNames)
        {
            UserProfiles.Add(userName, 0);
        }
    }


    public void OnRoundComplete(int score)
    {
        RoundNumber++;
        OverallTeamScore += score;
    }


    public void UpdatePlayerScore(string userName, int score)
    {
        userProfiles[userName] += score;
    }


    public int GetPlayerScore(string name)
    {
        return userProfiles[name];
    }


    public void PrintGameData()
    {
        Debug.Log("Round Number: " + RoundNumber);
        Debug.Log("Team Score: " + OverallTeamScore);
        foreach (var userProfile in userProfiles)
        {
            Debug.Log("Player: " + userProfile.Key + ", Score: " + userProfile.Value);
        }
    }
     
}
