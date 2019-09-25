public interface IPlayer
{
    float VolumeOfSoundEffects { get; set; }
    int PlayerId { get; set; }
    int PlayerScore { get; set; }
    bool IsGamePaused { get; }

    void BackToMainMenu();
    void CmdAction(string action);
    void CmdFail(string action, string bin);
    void CmdIncreaseScore();
    void DisableOkayButtonsOnPanels();
    void GameOver();
    string GetInstruction();
    int GetPlayerId();
    void IncreaseScoreStreak();
    void MicClick(string micString);
    void NfcClick(string nfcString);
    void OnClickButton1();
    void OnClickButton2();
    void OnClickButton3();
    void OnClickButton4();
    void OnClickCameraButton();
    void OnClickMicButton();
    void OnClickNfcButton();
    void OnClickShakeButton();
    void PausePlayer();
    void ResetScoreStreak();
    void ScoreStreakCheck();
    void SetActionButtons(string instruction, int i);
    void SetGameController(GameController controller);
    void SetInstruction(string d);
    void SetInstructionController(InstructionController instructionController);
    void SetMicPanel(string text);
    void SetNfcPanel(string text);
    void SetPlayerId(int assignedId);
    void SetShakePanel(string text);
    void ShakeClick(string shakeString);
    void StartInstTimer();
    void UnpausePlayer();
}