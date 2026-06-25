using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject cutScenePannel;

    private bool isPaused = false;

    void Update()
    {
        // ESC 키 입력 확인
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        menuPanel.SetActive(true);      // 패널 켜기
        Time.timeScale = 0f;            // 시간 멈춤
        
        // 마우스 커서가 보이게 설정
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        menuPanel.SetActive(false);     // 패널 끄기
        Time.timeScale = 1f;            // 시간 다시 흐름
        
        // 마우스 커서 숨기기
        if(cutScenePannel.activeSelf == false){
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void QuitGame()
    {
        // 실제 빌드된 게임 프로그램 종료
        Application.Quit();

        // 유니티 에디터에서 테스트 중일 때 재생 모드를 멈춤
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // 리스타트 버튼
    public void RestartScene()
    {
        // 현재 활성화된 씬의 이름을 가져와서 다시 로드합니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        
        Time.timeScale = 1f;
    }

    // 다른 씬으로 이동
    // 인스펙터 창에서 이동할 씬 이름을 직접 입력할 수 있게 매개변수 할당
    public void LoadTargetScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
}