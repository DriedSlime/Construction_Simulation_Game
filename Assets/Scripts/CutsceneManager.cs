using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI 연결")]
    public Image cutsceneImage;       // 화면에 보여줄 UI Image 컴포넌트
    public GameObject cutscenePanel;  // 컷신 UI 전체를 담고 있는 패널

    public Sprite[] cutsceneSprites;  // 인스펙터에서 이미지 5개를 넣을 배열

    private int currentIndex = 0;     // 현재 몇 번째 장을 보고 있는지 나타내는 인덱스
    private bool isClickAllowed = true; // 클릭 가능한 상태인지 체크하는 플래그
    private float clickCooldown = 0.5f; // 최소 딜레이 시간 (0.5초)

    void Start()
    {
        if (cutsceneSprites.Length > 0)
        {
            AudioListener.volume = 0f;

            cutscenePanel.SetActive(true);
            cutsceneImage.sprite = cutsceneSprites[currentIndex];
            
            StartCoroutine(ClickCooldownCoroutine());
        }
        else
        {
            StartInGame();
        }
    }

    // 마우스 클릭 시 호출될 함수 (전체 화면 버튼에 연결)
    public void OnClickNextCutscene()
    {
        // 1초 쿨타임 중이라면 클릭 무시
        if (!isClickAllowed) return;

        currentIndex++; // 다음 장으로 번호 증가

        // 아직 보여줄 컷신이 남아있다면 이미지 교체
        if (currentIndex < cutsceneSprites.Length)
        {
            cutsceneImage.sprite = cutsceneSprites[currentIndex];
            
            // 이미지가 바뀐 후 다시 1초 쿨타임 가동
            StartCoroutine(ClickCooldownCoroutine());
        }
        else
        {
            StartInGame();
        }
    }

    // 0.5초 동안 클릭을 막아주는 타이머 기능 (코루틴)
    private IEnumerator ClickCooldownCoroutine()
    {
        isClickAllowed = false; // 클릭 차단
        
        // clickCooldown만큼 대기
        yield return new WaitForSeconds(clickCooldown); 
        
        isClickAllowed = true;
    }

    void StartInGame()
    {
        AudioListener.volume = 1f;
        cutscenePanel.SetActive(false);
        if (ExcavatorGameManager.Instance != null)
        {
            ExcavatorGameManager.Instance.isGameStart = true;
            ExcavatorGameManager.Instance.StartInGameControl();
        }

        if(TransportGameManager.Instance != null)
        {
            TransportGameManager.Instance.StartInGameControl();
        }
        
        if(CraneGameManager.Instance != null)
        {
            CraneGameManager.Instance.StartInGameControl();
        }
    }
}