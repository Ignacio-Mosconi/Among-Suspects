using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MenuScreen
{
    public AnimatedMenuScreen screen;
    public AnimatedMenuScreen previousScreen;
}

public abstract class Menu : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] protected AnimatedMenuScreen mainScreen = default;
    [SerializeField] protected MenuScreen[] menuScreens = default;
    
    [Header("Menu Languages")]
    [SerializeField] ScriptableObject[] menuTranslatedTexts = new ScriptableObject[(int)Language.Count];

    protected Dictionary<Language, ScriptableObject> menuTextsByLanguage = new Dictionary<Language, ScriptableObject>();
    
    MenuScreen currentScreen;
    MenuScreen previousScreen;

    void OnValidate()
    {
        Array.Resize(ref menuTranslatedTexts, (int)Language.Count);
    }

    protected virtual void Awake()
    {
        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            menuTextsByLanguage.Add(language, menuTranslatedTexts[i]);
        }
    }

    protected virtual void Start()
    {
        currentScreen = Array.Find(menuScreens, ms => ms.screen == mainScreen);

        SetUpTexts();

        GameManager.Instance.AddCursorPointerEventsToAllButtons(gameObject);
        GameManager.Instance.OnLanguageChanged.AddListener(SetUpTexts);

        foreach (MenuScreen menuScreen in menuScreens)
            menuScreen.screen.SetUp();
    }

    protected abstract void SetUpTexts();

    public void ResetMenuState(bool showImmediately = true)
    {
        if (currentScreen.screen != mainScreen)
        {
            currentScreen.screen.Deactivate();
            currentScreen = Array.Find(menuScreens, ms => ms.screen == mainScreen);
            if (showImmediately)
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