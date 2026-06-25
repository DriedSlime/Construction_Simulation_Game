using UnityEngine;

public class CraneScoreManager : MonoBehaviour
{
    public static CraneScoreManager instance;
    private int currentScore = 0;

    void Awake()
    {
        instance = this;
    }

    public void AddScore(int points)
    {
        currentScore += points;
    }
    public void GameOver()
    {
        if (CraneGameManager.Instance != null)
        {
            CraneGameManager.Instance.ShowResult(currentScore);
        }
    }
}
