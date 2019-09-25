using NUnit.Framework;

public class GameStateTests
{
    [Test]
    public void IsGameStartedNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.isGameStarted);
    }

    [Test]
    public void IsGameStartedInitialStartUp_Test()
    {
        GameController gameController = new GameController();
        Assert.IsFalse(gameController.isGameStarted);
    }

    [Test]
    public void PlayersInitialisedNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.playersInitialised);
    }

    [Test]
    public void PlayersInitialisedInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.IsFalse(gameController.playersInitialised);
    }

    [Test]
    public void PlayersInitialised_Test()
    {
        GameController gameController = new GameController();
        gameController.PlayersInitialisedFromStart();
        Assert.AreEqual(true, gameController.playersInitialised);
    }

    [Test]
    public void RoundTimerInitialisation_Test()
    {
        GameSettings.RoundTime = 90;
        GameController gameController = new GameController();
        gameController.StartRoundTimer();
        Assert.AreEqual(0, gameController.roundStartTime);
    }

    [Test]
    public void IsLastActionOfRoundNotNull_Test()
    {
        InstructionController instructionController = new InstructionController();
        Assert.NotNull(instructionController.isLastActionOfRound);
    }

    [Test]
    public void IsLastActionOfRoundInitialSetUp_Test()
    {
        InstructionController instructionController = new InstructionController();
        Assert.IsFalse(instructionController.isLastActionOfRound);
    }

    public void IsGamePausedNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.isRoundPaused);
    }

    [Test]
    public void IsGamePausedInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.IsFalse(gameController.isRoundPaused);
    }

    [Test]
    public void IsRoundPaused_Test()
    {
        GameController gameController = new GameController();
        gameController.RoundPaused();
        Assert.AreEqual(true, gameController.isRoundPaused);
    }


    [Test]
    public void PlayerPauseNotNull_Test()
    {
        Player player = new Player();
        Assert.NotNull(player.IsGamePaused);
    }

    [Test]
    public void PlayerPausedInitialStartUp_Test()
    {
        Player player = new Player();
        Assert.IsFalse(player.IsGamePaused);
    }

    [Test]
    public void PlayerPauseGameState_Test()
    {
        Player player = new Player();
        player.PausePlayer();
        Assert.AreEqual(true, player.IsGamePaused);
    }

    [Test]
    public void PlayerUnpauseGameState_Test()
    {
        Player player = new Player();
        player.UnpausePlayer();
        Assert.AreEqual(false, player.isGamePaused);
    }

    [Test]
    public void IsGameOverNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.IsGameOver);
    }

    [Test]
    public void IsGameOverInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.IsFalse(gameController.IsGameOver);
    }

    [Test]
    public void IsGameOver_Test()
    {
        GameController gameController = new GameController();
        gameController.GameOver();
        Assert.AreEqual(true, gameController.IsGameOver);
    }

    [Test]
    public void RoundNumberNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.roundNumber);
    }

    [Test]
    public void RoundNumberInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.AreEqual(1, gameController.roundNumber);
    }

    [Test]
    public void RoundNumberIncrease_Test()
    {
        GameController gameController = new GameController();
        gameController.roundNumber++;
        Assert.AreEqual(2, gameController.roundNumber);
    }


    [Test]
    public void FireCountNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.fireCount);
    }

    [Test]
    public void FireCountInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.AreEqual(0, gameController.fireCount);
    }

    [Test]
    public void FireCountIncrease_Test()
    {
        GameController gameController = new GameController();
        gameController.fireCount++;
        Assert.AreEqual(1, gameController.fireCount);
    }

}
