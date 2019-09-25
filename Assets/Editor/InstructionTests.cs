using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class InstructionTests
{
    [Test]
    public void ActiveButtonActionsNotNull_Test()
    {
        InstructionController instructionController = new InstructionController();
        Assert.NotNull(instructionController.ActiveButtonActions);
    }

    [Test]
    public void ActiveButtonActionsInitialSize_Test()
    {
        InstructionController instructionController = new InstructionController();
        Assert.AreEqual(0, instructionController.ActiveButtonActions.Count);
    }

    [Test]
    public void ActiveInstructionsNotNull_Test()
    {
        InstructionController instructionController = new InstructionController();
        Assert.NotNull(instructionController.ActiveInstructions);
    }

    [Test]
    public void ActiveInstructionsInitialSize_Test()
    {
        InstructionController instructionController = new InstructionController();
        Assert.AreEqual(0, instructionController.ActiveInstructions.Count);
    }

}
