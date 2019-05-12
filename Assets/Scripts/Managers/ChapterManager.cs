using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    #region Singleton

    static ChapterManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static ChapterManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<ChapterManager>();
                if (!instance)
                    Debug.LogError("There is no 'Chapter Manager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    [SerializeField] ClueInfo[] chapterClues;

    public ClueInfo GetChapterClueInfo(int index)
    {
        ClueInfo clueInfo = null;

        if (index < chapterClues.Length && index >= 0)
            clueInfo = chapterClues[index];

        return clueInfo;
    }
}