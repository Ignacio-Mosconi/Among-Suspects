using UnityEngine;

public class PauseMenu : Menu
{
    [SerializeField] GameObject menuArea;

    bool isPaused;

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        menuArea.SetActive(true);

        GameManager.Instance.SetCursorAvailability(enable: true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        menuArea.SetActive(false);

        ResetMenuState();
        GameManager.Instance.SetCursorAvailability(enable: false);
    }
}