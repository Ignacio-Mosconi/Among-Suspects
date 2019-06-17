using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MenuScreen
{
    public AnimatedMenuScreen screen;
    public AnimatedMenuScreen previousScreen;
}

public class Menu : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] protected AnimatedMenuScreen mainScreen = default;
    [SerializeField] protected MenuScreen[] menuScreens = default;

    MenuScreen currentScreen;
    MenuScreen previousScreen;

    protected virtual void Start()
    {
        currentScreen = Array.Find(menuScreens, ms => ms.screen == mainScreen);
        GameManager.Instance.AddCursorPointerEventsToAllButtons(gameObject);

        foreach (MenuScreen menuScreen in menuScreens)
            menuScreen.screen.SetUp();
    }

    protected void ResetMenuState()
    {
        if (currentScreen.screen != mainScreen)
        {
            currentScreen.screen.Deactivate();
            currentScreen = Array.Find(menuScreens, ms => ms.screen == mainScreen);
            currentScreen.screen.Show();
            previousScreen.screen = null;
            previousScreen.previousScreen = null;
        }
    }

    public void MoveToNextScreen(AnimatedMenuScreen nextScreen)
    {
        currentScreen.screen.Hide();
        nextScreen.Show();

        previousScreen = currentScreen;
        currentScreen = Array.Find(menuScreens, ms => ms.screen == nextScreen);
    }

    public void ReturnToPreviousScreen()
    {
        currentScreen.screen.Hide();
        previousScreen.screen.Show();
        
        currentScreen = previousScreen;
        previousScreen = Array.Find(menuScreens, ms => ms.screen == previousScreen.previousScreen);
    }
}