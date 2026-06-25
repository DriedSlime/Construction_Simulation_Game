using UnityEngine;
using TMPro;
using System.Collections;

public class ExcavatorGameManager : MonoBehaviour
{
    public static ExcavatorGameManager Instance; // 어디서든 접근 가능하게 싱글톤 설정

    public GameObject clearPanel;    // UI 패널 연결
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI inGameTimerText;

    public int remainingHouses; // 남은 집 개수
    public int remainingTrashes; // 치워야 할 쓰레기 개수

    public float timer;
    private bool isGameOver = false;
    public bool isGameStart = false;

    void Awake()
    {
        Application.targetFrameRate = 144;

        Instance = this;
        clearPanel.SetActive(false);

    }

    void Start() {
        // 시작 시 개수 파악
        remainingHouses = GameObject.FindGameObjectsWithTag("Wall").Length + GameObject.FindGameObjectsWithTag("Pillar").Length + GameObject.FindGameObjectsWithTag("Props").Length - 3;
        remainingTrashes = 0; 
        timer = 0f;
    }
    void Update()
    {
        if (!isGameOver && isGameStart)
        {
            timer += Time.deltaTime;
            UpdateInGameTimerUI();
        }
    }
    void UpdateInGameTimerUI()
    {
        if (inGameTimerText != null)
        {
            // 보기 좋게 분:초 형식으로 표시
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            inGameTimerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
    }
    public void OnHouseDestroyed(bool spawnedTrash) {
        remainingHouses--;
        if (spawnedTrash)
        {
            remainingTrashes++;
        }

        CheckClearCondition();
    }
    public void OnTrashCollected()
    {
        remainingTrashes--;
        CheckClearCondition();
    }

    // 쓰레기가 다시 밖으로 나갔을 경우
    public void OnTrashEscaped()
    {
        remainingTrashes++;
    }
    private void CheckClearCondition()
    {
        if (remainingHouses <= 0 && remainingTrashes <= 0)
        {
            ShowClearUI();
        }
    }
    void ShowClearUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (clearPanel != null)
        {
            isGameOver = true;

            string rank = CalculateRank(timer);

            if (rankText != null) rankText.text = $"Rank : {rank}";

            clearPanel.SetActive(true);
            // 커서 잠금 해제
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            switch (rank)
            {
                case "S": rankText.color = Color.yellow; break; // 금색
                case "A": rankText.color = Color.green; break;  // 초록색
                case "B": rankText.color = Color.cyan; break;   // 하늘색
                default: rankText.color = Color.gray; break;    // 회색
            }
            PlayerPrefs.SetString("ExcavatorRank", rank);
        }
        Time.timeScale = 0f;
    }
    string CalculateRank(float finishTime)
    {
        if (finishTime <= 150f) return "S";
        if (finishTime <= 180f) return "A";
        if (finishTime <= 240f) return "B";
        if (finishTime <= 300f) return "C";
        return "D";
    }

    public void StartInGameControl()
    {
        // 컷신이 끝났으니 이제 커서를 감추고 조작을 잠글 것
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}