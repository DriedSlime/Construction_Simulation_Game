using UnityEngine;
using TMPro;

public class TransportGameManager : MonoBehaviour
{
    public static TransportGameManager Instance;

    [Header("Score Settings")]
    public int currentScore = 1000;

    [Header("Result UI")]
    public GameObject clearPanel;    // 클리어 결과 패널 오브젝트
    public GameObject gameOverPanel; // 게임 오버 패널 오브젝트
    public TextMeshProUGUI rankText;  // 결과 패널의 등급 텍스트 (S, A, B, C)

    public Transform truckTransform;  // 트럭
    private bool isGameOver = false;   // 게임오버 중복 실행 방지 플래그

    void Awake()
    {
        Application.targetFrameRate = 144;
        Instance = this;
        clearPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        // 트럭이 존재하고, 아직 게임오버 상태가 아닐 때만 Y축 체크
        if (truckTransform != null && !isGameOver)
        {
            if (truckTransform.position.y <= -6f)
            {
                GameOver();
            }
        }
    }

    public void DecreaseScore(int amount)
    {
        currentScore -= amount;
        if (currentScore < 0) currentScore = 0;
    }

    // 목표 지점 도달 시 호출할 함수
    public void FinishGame()
    {
        if (isGameOver) return; // 이미 게임오버라면 클리어 처리 안 함

        clearPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        string rank = CalculateRank(currentScore);

        if (rankText != null) rankText.text = $"Rank : {rank}";

        switch (rank) 
        {
            case "S": rankText.color = Color.yellow; break; // 금색
            case "A": rankText.color = Color.green; break;  // 초록색
            case "B": rankText.color = Color.cyan; break;   // 하늘색 
            default: rankText.color = Color.gray; break;    // 회색
        }
        PlayerPrefs.SetString("TransportRank", rank);
        Time.timeScale = 0; // 게임 일시 정지
    }

    // Y축 -10 이하로 추락 시 호출할 게임 오버 함수
    public void GameOver()
    {
        isGameOver = true; // 2번 실행되지 않도록 방지
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true); // 게임 오버 창 켜기

        // 마우스 커서 해제
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0; // 게임 일시 정지
    }

    private string CalculateRank(int score)
    {
        if (score >= 1000) return "S";
        if (score >= 800)  return "A";
        if (score >= 600)  return "B";
        return "C";
    }

    public void StartInGameControl()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}