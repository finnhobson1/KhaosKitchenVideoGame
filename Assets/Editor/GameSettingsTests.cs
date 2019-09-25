using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class GameSettingsTests
{
    [Test]
    public void BaseInstructionNumberNotNull_Test()
    {
        Assert.NotNull(GameSettings.BaseInstructionNumber);
    }

    [Test]
    public void BaseInstructionNumberSetting_Test()
    {
        GameSettings.BaseInstructionNumber = 10;
        Assert.AreEqual(10, GameSettings.BaseInstructionNumber);
    }

    [Test]
    public void InstructionNumberIncreasePerRoundNotNull_Test()
    {
        Assert.NotNull(GameSettings.InstructionNumberIncreasePerRound);
    }


    [Test]
    public void InstructionNumberIncreasePerRoundSetting_Test()
    {
        GameSettings.InstructionNumberIncreasePerRound = 10;
        Assert.AreEqual(10, GameSettings.InstructionNumberIncreasePerRound);
    }


    [Test]
    public void BaseInstructionTimeNotNull_Test()
    {
        Assert.NotNull(GameSettings.BaseInstructionTime);
    }


    [Test]
    public void BaseInstructionTimeSetting_Test()
    {
        GameSettings.BaseInstructionTime = 10;
        Assert.AreEqual(10, GameSettings.BaseInstructionTime);
    }

    [Test]
    public void InstructionTimeReductionPerRoundNotNull_Test()
    {
        Assert.NotNull(GameSettings.InstructionTimeReductionPerRound);
    }


    [Test]
    public void InstructionTimeReductionPerRoundSetting_Test()
    {
        GameSettings.InstructionTimeReductionPerRound = 10;
        Assert.AreEqual(10, GameSettings.InstructionTimeReductionPerRound);
    }

    [Test]
    public void InstructionTimeIncreasePerPlayerNotNull_Test()
    {
        Assert.NotNull(GameSettings.InstructionTimeIncreasePerPlayer);
    }


    [Test]
    public void InstructionTimeIncreasePerPlayerSetting_Test()
    {
        GameSettings.InstructionTimeIncreasePerPlayer = 10;
        Assert.AreEqual(10, GameSettings.InstructionTimeIncreasePerPlayer);
    }

    [Test]
    public void MinimumInstructionTimeNotNull_Test()
    {
        Assert.NotNull(GameSettings.MinimumInstructionTime);
    }


    [Test]
    public void MinimumInstructionTimeSetting_Test()
    {
        GameSettings.MinimumInstructionTime = 10;
        Assert.AreEqual(10, GameSettings.MinimumInstructionTime);
    }


    [Test]
    public void RoundTimeNotNull_Test()
    {
        Assert.NotNull(GameSettings.RoundTime);
    }


    [Test]
    public void RoundTimeSetting_Test()
    {
        GameSettings.RoundTime = 90;
        Assert.AreEqual(90, GameSettings.RoundTime);
    }

    [Test]
    public void EasyPhoneInteractionsNotNull_Test()
    {
        Assert.NotNull(GameSettings.EasyPhoneInteractions);
    }


    [Test]
    public void EasyPhoneInteractionsSetting_Test()
    {
        GameSettings.EasyPhoneInteractions = true;
        Assert.AreEqual(true, GameSettings.EasyPhoneInteractions);
    }


    [Test]
    public void PhoneInteractionProbabilityNotNull_Test()
    {
        Assert.NotNull(GameSettings.PhoneInteractionProbability);
    }


    [Test]
    public void PhoneInteractionProbabilitySetting_Test()
    {
        GameSettings.PhoneInteractionProbability = 90;
        Assert.AreEqual(90, GameSettings.PhoneInteractionProbability);
    }
}
