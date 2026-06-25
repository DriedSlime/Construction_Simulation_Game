using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class CraneGameManager : MonoBehaviour
{
    public static CraneGameManager Instance;

    float gameOverYThreshold = 15f; // 종료 기준이 되는 Y 높이
    float delayBeforeGameOver = 8f; // 조건 달성 후 대기 시간
    private bool isGameOver = false;

    //private bool isGameOverTriggered = false; // 타이머가 이미 시작되었는지 확인용 플래그
    //private bool isGameOverFinished = false;  // 최종 종료 여부

    public GameObject clearPanel;      // 결과 패널 부모 오브젝트
    public TextMeshProUGUI rankText;    // 등급을 표시할 텍스트

    void Awake()
    {
        Application.targetFrameRate = 144;

        Instance = this;
        clearPanel.SetActive(false);
    }

    void Update()
    {
        // 이미 게임이 종료되었다면 더 이상 체크하지 않음
        if (isGameOver) return;

        CheckGameOverCondition();
    }

    private void CheckGameOverCondition()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        if (buildings.Length == 0) return;

        bool allBuildingsBelowThreshold = true;

        foreach (GameObject building in buildings)
        {
            // 단 하나라도 50 위에 있다면 아직 조건을 만족하지 못함
            if (building.transform.position.y >= gameOverYThreshold)
            {
                allBuildingsBelowThreshold = false;
                break;
            }
        }

        // 모든 건물이 Y축 50 아래로 내려갔다면
        if (allBuildingsBelowThreshold)
        {
            // 10초 대기 코루틴 시작
            StartCoroutine(GameOverSequence());
        }
    }

    private IEnumerator GameOverSequence()
    {
        //isGameOverTriggered = true;

        // 10초 동안 대기
        yield return new WaitForSeconds(delayBeforeGameOver);

        // 최종 게임 종료 처리
        //isGameOverFinished = true;

        // ScoreManager를 통해 결과 패널 활성화
        if (CraneScoreManager.instance != null)
        {
            CraneScoreManager.instance.GameOver();
        }
        Time.timeScale = 0;
    }

    public void ShowResult(int finalScore)
    {
        if (clearPanel == null) return;

        // 결과 패널 활성화
        clearPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 점수별 등급 판정 로직
        string rank = "C";

        if (finalScore >= 500)
        {
            rank = "S";
        }
        else if (finalScore >= 400)
        {
            rank = "A";
        }
        else if (finalScore >= 300)
        {
            rank = "B";
        }
        else
        {
            rank = "C";
        }

        // 등급 텍스트 반영
        if (rankText != null)
        {
            rankText.text = $"Rank : {rank}";
            
            // 보너스: 등급에 따른 색상 변경
            switch (rank)
            {
                case "S": rankText.color = Color.yellow; break; // 금색
                case "A": rankText.color = Color.green; break;  // 초록색
                case "B": rankText.color = Color.cyan; break;   // 하늘색
                default: rankText.color = Color.gray; break;    // 회색
            }
        }
        PlayerPrefs.SetString("CraneRank", rank);
    }
    public void StartInGameControl()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnResultButtonClick()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("EndScene");
    }
}
