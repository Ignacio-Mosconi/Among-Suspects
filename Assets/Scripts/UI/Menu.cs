using System;
using UnityEngine;

[System.Serializable]
public struct MenuScreen
{
    public GameObject screen;
    public GameObject previousScreen;
}

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject mainScreen;
    [SerializeField] MenuScreen[] menuScreens;

    MenuScreen currentScreen;
    MenuScreen previousScreen;

    void Start()
    {
        ResetMenuState();
    }

    protected void ResetMenuState()
    {
        currentScreen = Array.Find(menuScreens, ms => ms.screen == mainScreen);
        previousScreen.screen = null;
        previousScreen.previousScreen = null;
    }

    public void MoveToNextScreen(GameObject nextScreen)
    {
        currentScreen.screen.SetActive(false);
        nextScreen.SetActive(true);

        previousScreen = currentScreen;
        currentScreen = Array.Find(menuScreens, ms => ms.screen == nextScreen);
    }

    public void ReturnToPreviousScreen()
    {
        currentScreen.screen.SetActive(false);
        previousScreen.screen.SetActive(true);
        
        currentScreen = previousScreen;
        previousScreen = Array.Find(menuScreens, ms => ms.screen == previousScreen.previousScreen);
    }
}