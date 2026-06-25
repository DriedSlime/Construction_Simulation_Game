using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TruckController : MonoBehaviour
{
    [System.Serializable]
    public struct Wheel {
        public WheelCollider collider;
        public Transform mesh;
    }

    [Header("Wheels")]
    public Wheel[] frontWheels; // 조향 + 구동
    public Wheel[] rearWheels;  // 구동

    [Header("Engine Settings")]
    float motorForce = 700f;
    float brakeForce = 7000f; 
    float maxVelocity = 50f;
    float currentSteerAngle = 0f;
    float maxSteerAngle = 20f;
    float steerSpeed = 30f;

    private Rigidbody rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 무게 중심을 낮게 배치해 안정성 확보
        rb.centerOfMass = new Vector3(0, -0.8f, 0);
    }

    public void OnMove(InputValue value) => input = value.Get<Vector2>();

    void FixedUpdate()
    {
        ApplySteering();
        ApplyDrive();
        UpdateVisuals();
        
        // 속도 제한
        if (rb.linearVelocity.magnitude > maxVelocity)
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
    }

    private void ApplySteering()
    {
        float targetSteerAngle = input.x * maxSteerAngle;

        // 현재 각도에서 목표 각도까지 부드럽게 이동
        currentSteerAngle = Mathf.MoveTowards(
            currentSteerAngle, 
            targetSteerAngle, 
            steerSpeed * Time.fixedDeltaTime
        );

        foreach (var wheel in frontWheels) 
        {
            wheel.collider.steerAngle = currentSteerAngle;
        }
    }

    private void ApplyDrive()
    {
        // 현재 로컬 Z 속도
        float localVelocityZ = transform.InverseTransformDirection(rb.linearVelocity).z;
        
        // 브레이크 판별 - 진행 방향과 입력 방향이 반대일 때
        bool isBraking = (localVelocityZ > 0.1f && input.y < 0) || (localVelocityZ < -0.1f && input.y > 0);

        if (AudioManager.Instance != null)
        {
            float forwardIntensity = Mathf.Max(0f, input.y);
            AudioManager.Instance.SetEnginePitch(forwardIntensity);
        }

        foreach (var wheel in GetAllWheels())
        {
            if (isBraking)
            {
                wheel.collider.motorTorque = 0;
                wheel.collider.brakeTorque = brakeForce;
            }
            else if (input.y == 0)
            {
                wheel.collider.motorTorque = 0;
                wheel.collider.brakeTorque = brakeForce * 0.05f; // 중립 시 자동 감속
            }
            else
            {
                wheel.collider.brakeTorque = 0;
                wheel.collider.motorTorque = input.y * motorForce;
            }
        }
    }

    private void UpdateVisuals()
    {
        foreach (var wheel in GetAllWheels())
        {
            Vector3 pos; Quaternion rot;
            wheel.collider.GetWorldPose(out pos, out rot);
            wheel.mesh.position = pos;
            wheel.mesh.rotation = rot;
        }
    }

    // 가독성과 성능을 위해 휠 그룹 통합 순회
    private IEnumerable<Wheel> GetAllWheels()
    {
        for (int i = 0; i < frontWheels.Length; i++) yield return frontWheels[i];
        for (int i = 0; i < rearWheels.Length; i++) yield return rearWheels[i];
    }
}