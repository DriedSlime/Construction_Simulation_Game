using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraneController : MonoBehaviour
{
    public Transform craneTop;
    public Transform trolley;
    public Transform hook;
    private LineRenderer wire;

    [Header("속도 설정")]
    float rotateSpeed = 30f;
    float trolleySpeed = 5f;
    float hookSpeed = 6f;
    bool isSlowMode = false;

    private float rotateInput;
    private float trolleyInput;
    private float hookVerticalInput;
    private float spinInput;

    void Start()
    {
        // 트롤리에 붙은 LineRenderer를 가져와 와이어로 사용
        wire = trolley.GetComponent<LineRenderer>();
    }

    void Update()
    {
        HandleMovement();
        UpdateWire();
    }

    private void HandleMovement()
    {
        float speedMultiplier = isSlowMode ? 0.2f : 1.0f;

        // 회전
        float rotationDelta = rotateInput * (rotateSpeed * speedMultiplier) * Time.deltaTime;
        craneTop.Rotate(Vector3.up * rotationDelta);

        // 회전 소리 적용
        bool isRotating = Math.Abs(rotationDelta) > 0.01f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetCraneRotationSound(isRotating);
        }

        // 트롤리 이동 (앞뒤)
        Vector3 tPos = trolley.localPosition;
        tPos.z += trolleyInput * (trolleySpeed * speedMultiplier) * Time.deltaTime;
        tPos.z = Mathf.Clamp(tPos.z, 7f, 28f);
        trolley.localPosition = tPos;
        Debug.Log(tPos.z);
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.SetCraneTrolleySound(Math.Abs(trolleyInput) > 0.1f && tPos.z > 7f && tPos.z < 28f);
        }

        // 훅 이동 (위아래)
        Vector3 hPos = hook.localPosition;
        hPos.y += hookVerticalInput * hookSpeed * Time.deltaTime;
        hPos.y = Mathf.Clamp(hPos.y, -18.5f, -5f);
        hook.localPosition = hPos;

        float spinDelta = spinInput * (rotateSpeed * speedMultiplier) * Time.deltaTime;
        hook.Rotate(Vector3.up * spinDelta);
        
    }

    // Input System 이벤트 연결
    public void OnRotateInput(InputAction.CallbackContext context) => rotateInput = context.ReadValue<float>();
    public void OnTrolleyInput(InputAction.CallbackContext context) => trolleyInput = context.ReadValue<float>();
    public void OnHookInput(InputAction.CallbackContext context) => hookVerticalInput = context.ReadValue<float>();
    public void OnSpinInput(InputAction.CallbackContext context) => spinInput = context.ReadValue<float>();
    public void OnSlowMode(InputAction.CallbackContext context)
    {
        // Shift 키를 누르고 있는 동안 true, 떼면 false
        if (context.performed) isSlowMode = true;
        if (context.canceled) isSlowMode = false;
    }

    private void UpdateWire()
    {
        if (wire != null)
        {
            // 와이어 시작점: 트롤리의 로컬 (0,0,0)
            wire.SetPosition(0, Vector3.zero); 
            // 와이어 끝점: 훅의 로컬 좌표
            wire.SetPosition(1, hook.localPosition);
        }
    }
}