using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class ExcavatorVehicleController : MonoBehaviour
{
    [System.Serializable]
    public struct Wheel {
        public WheelCollider collider;
        public Transform visualMesh;
        public bool isSteerable; // 앞바퀴(조향) 여부
        public bool isMotorized;  // 구동바퀴(엔진) 여부
    }
    public List<Wheel> wheels;
    public Transform bodyLogic;
    public Transform knee1Logic;    
    public Transform knee2Logic;   
    public Transform bucketLogic;  

    float motorForce = 1500f;
    float brakeForce = 3000f;
    float maxSteerAngle = 40f;  // 최대 조향 각도
    float maxSpeed = 10f;

    float armSpeed = 50f;
    float knee1Min = -70f;
    float knee1Max = -10f;
    float knee2Min = -100f;
    float knee2Max = 0f;
    float bucketMin = -100f;
    float bucketMax = 0f;

    private Rigidbody rb;
    private float knee1Angle, knee2Angle, bucketAngle;
    private Vector2 moveInput;
    private float bodyInput, arm1Input, arm2Input, bucketInput;
    private ExcavatorInput inputActions;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new ExcavatorInput();

        // 입력 바인딩 (W/S: y, A/D: x)
        inputActions.Player.MoveBase.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.MoveBase.canceled += ctx => moveInput = Vector2.zero;
        
        inputActions.Player.RotateBody.performed += ctx => bodyInput = ctx.ReadValue<float>();
        inputActions.Player.RotateBody.canceled += ctx => bodyInput = 0f;
        inputActions.Player.Arm1.performed += ctx => arm1Input = ctx.ReadValue<Vector2>().y;
        inputActions.Player.Arm1.canceled += ctx => arm1Input = 0f;
        inputActions.Player.Arm2.performed += ctx => arm2Input = ctx.ReadValue<float>();
        inputActions.Player.Arm2.canceled += ctx => arm2Input = 0f;
        inputActions.Player.Bucket.performed += ctx => bucketInput = ctx.ReadValue<float>();
        inputActions.Player.Bucket.canceled += ctx => bucketInput = 0f;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Start()
    {
        knee1Angle = NormalizeAngle(knee1Logic.localEulerAngles.x);
        knee2Angle = NormalizeAngle(knee2Logic.localEulerAngles.x);
        bucketAngle = NormalizeAngle(bucketLogic.localEulerAngles.x);
        
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.centerOfMass += new Vector3(0, -0.5f, 0); // 전복 방지
    }

    void Update()
    {
        float scrollDelta = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollDelta) > 0.1f)
        {
            float wheelSensitivity = 0.01f; 
            knee1Angle += scrollDelta * wheelSensitivity * armSpeed;
        }
        HandleBodyAndArms();
    }

    void FixedUpdate()
    {
        HandleVehicleMovement();
    }

    private void HandleVehicleMovement()
    {
        // 입력값 정규화
        float forwardInput = -moveInput.y; // 앞뒤가 반대면 -moveInput.y
        float steerInput = moveInput.x;

        float currentSpeed = rb.linearVelocity.magnitude;

        // 엔진 피치 조절
        if (AudioManager.Instance != null)
        {
            float forwardIntensity = Mathf.Max(0f, -forwardInput);
            AudioManager.Instance.SetEnginePitch(forwardIntensity);
        }

        foreach (var w in wheels)
        {
            // 조향 적용 (앞바퀴만)
            if (w.isSteerable)
            {
                w.collider.steerAngle = steerInput * maxSteerAngle;
            }

            // 구동 및 제동 로직
            if (Mathf.Abs(forwardInput) < 0.05f)
            {
                // 입력 없을 때 약한 제동 (자연스러운 정지)
                w.collider.brakeTorque = brakeForce * 0.2f;
                w.collider.motorTorque = 0;
            }
            else
            {
                w.collider.brakeTorque = 0;
                
                if (w.isMotorized && currentSpeed < maxSpeed)
                {
                    w.collider.motorTorque = forwardInput * motorForce;
                }
                else
                {
                    w.collider.motorTorque = 0;
                }
            }
        }
    }

    private void HandleBodyAndArms()
    {
        bodyLogic.Rotate(Vector3.up * bodyInput * armSpeed * Time.deltaTime);

        knee1Angle = Mathf.Clamp(knee1Angle + arm1Input * 0.1f * armSpeed * Time.deltaTime, knee1Min, knee1Max);
        knee1Logic.localRotation = Quaternion.Euler(knee1Angle, 0f, 0f);

        knee2Angle = Mathf.Clamp(knee2Angle + arm2Input * armSpeed * Time.deltaTime, knee2Min, knee2Max);
        knee2Logic.localRotation = Quaternion.Euler(knee2Angle, 0f, 0f);

        bucketAngle = Mathf.Clamp(bucketAngle + bucketInput * armSpeed * Time.deltaTime, bucketMin, bucketMax);
        bucketLogic.localRotation = Quaternion.Euler(bucketAngle, 0f, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}