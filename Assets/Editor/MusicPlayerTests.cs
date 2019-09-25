using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class MusicPlayerTests
{

    [Test] 
    public void IsRoundPausedNotNull_Test()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.NotNull(music.IsRoundPaused);
    }

    [Test]
    public void IsRoundPausedStartUp_Test()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.IsFalse(music.IsRoundPaused);
    }

    [Test]
    public void IsMusicPausedNotNull_Test()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.NotNull(music.IsMusicPaused);
    }

    [Test]
    public void IsMusicPausedStartUp_Test()
    {
        MusicPlayer music = new MusicPlayer();
        Assert.IsFalse(music.IsMusicPaused);
    }
}
