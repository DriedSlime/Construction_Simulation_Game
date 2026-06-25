using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; 

public class CraneCameraController : MonoBehaviour
{
    [Header("카메라 설정")]
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    private bool isFirstPerson = true;

    void Start()
    {
        UpdateCameraStates();
    }

    // Input Action에서 'V' 키 액션에 이 함수를 연결
    public void OnToggleView(InputAction.CallbackContext context)
    {
        // 키를 눌렀을 때(Performed) 한 번만 실행
        if (context.performed)
        {
            isFirstPerson = !isFirstPerson;
            UpdateCameraStates();
        }
    }

    private void UpdateCameraStates()
    {
        if (isFirstPerson)
        {
            // 1인칭 활성화
            firstPersonCam.Priority = 100;
            thirdPersonCam.Priority = 0;
        }
        else
        {
            // 3인칭 활성화
            firstPersonCam.Priority = 0;
            thirdPersonCam.Priority = 100;
        }
    }
}