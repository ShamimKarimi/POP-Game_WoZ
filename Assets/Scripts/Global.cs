using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{

    public static float TimerInterval = 0.02f;

    public static string[] targetPositions = { "DR", "DL", "UR", "UL", "SR", "SL" };
    public static string[] columnPositions = { "LL", "ML", "MR", "RR" };

    public static string[] colors = { "green", "red", "yellow", "blue", "pink", "cyan" };

    public static string[] mainColors = { "green", "red", "blue" };

    public static float targetColorPercentage = 0.5f;

    public static string generateType = "generate";
    public static string hitType = "hit";
    public static string missType = "miss";

    public static int maxNumberOfBalloonsOnScreen = 5;
    public static int maxNumberOfBalloonsInTotal = 15;

    public static float balloonAnimationSpeed = 2.0f; //how fast it moves
    public static float balloonAnimationDelta = 0.005f; //how much it moves

    public static float balloonVerticalTranslationDelta = 0.03f;

    public static int timeToNextBalloonMin = 2;
    public static int timeToNextBalloonMax = 3;

    public static int colorX = -1220;
    public static int colorY = 600;

    public static int intervalBetweenErrorSounds = 3;

    public static float popAnimationDuration = 0.3f;

}
