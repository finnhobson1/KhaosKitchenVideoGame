using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class PlayerCountTests
{
    [Test]
    public void PlayerCountNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.playerCount);
    }

    [Test]
    public void PlayerCountInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.AreEqual(0, gameController.playerCount);
    }

    [Test]
    public void PlayerCountIncrease_Test()
    {
        GameController gameController = new GameController();
        gameController.playerCount++;
        Assert.AreEqual(1, gameController.playerCount);
    }



    [Test]
    public void PlayerListNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.playerList);
    }

    [Test]
    public void PlayerListInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.AreEqual(0, gameController.playerList.Count);
    }

    [Test]
    public void PlayerListIncrease_Test()
    {
        Player player = new Player();
        GameController gameController = new GameController();
        gameController.playerList.Add(player);
        Assert.AreEqual(1, gameController.playerList.Count);
    }


    [Test]
    public void PlayerNamesListNotNull_Test()
    {
        GameController gameController = new GameController();
        Assert.NotNull(gameController.playerNames);
    }

    [Test]
    public void PlayerNamesListInitialSetUp_Test()
    {
        GameController gameController = new GameController();
        Assert.AreEqual(0, gameController.playerNames.Count);
    }

    [Test]
    public void PlayerNamesListIncrease_Test()
    {
        Player player = new Player();
        GameController gameController = new GameController();
        gameController.playerList.Add(player);
        Assert.AreEqual(1, gameController.playerList.Count);
    }
}
