using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global
{
    public static int dayCount;

    public const int nAchievements = 6;
    public static bool[] achievements;

    public static bool initialised = false;

    public static void Initialise()
    {
        if (initialised) return;
        achievements = new bool[nAchievements];
        Reset();
    }

    public static void RestartGame()
    {
        dayCount++;
        Debug.Log(dayCount);
        StartGame();
    }

    public static void Reset()
    {
        dayCount = 1;
        for (int i = 0; i < achievements.Length; ++i)
        {
            achievements[i] = false;
        }
    }
    
    public static void GoToMenu()
    {
        Reset();
        SceneManager.LoadScene("mainmenu");
    }
    
    public static void StartGameFromMenu()
    {
        Reset();
        StartGame();
    }

    private static void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
