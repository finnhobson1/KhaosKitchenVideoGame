public interface IGameController
{
    int CalculateInstructionTime();
    void CheckAction(string action, int i);
    void IncreaseFireCount();
    void IncreaseScore();
    void OnClickBack();
    void PlayersInitialisedFromStart();
    void PrintOut(int buttonNumber);
    void RpcIncreaseScoreSteak(int playerID);
    void RpcPausePlayers();
    void RpcQuitGame();
    void RpcResetScoreSteak(int playerID);
    void RpcScoreSteakCheck(int playerID);
    void RpcUnpausePlayers();
    void StartRoundTimer();
}