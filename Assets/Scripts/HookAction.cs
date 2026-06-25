using UnityEngine;
using UnityEngine.InputSystem;

public class HookAction : MonoBehaviour
{
    [Header("Settings")]
    private const float GRAB_COOLDOWN = 1f;

    private GameObject grabbedObject = null;
    private float lastReleaseTime = 0f;

    private Vector3 lastPosition;
    private Vector3 currentVelocity;

    void Update()
    {
        currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbedObject != null || Time.time - lastReleaseTime < GRAB_COOLDOWN) return;

        if (other.CompareTag("Building"))
        {
            GrabObject(other.gameObject);
        }
    }

    private void GrabObject(GameObject obj)
    {
        grabbedObject = obj;

        // 물리 연산 중지
        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // 부모를 '훅' 자체가 아닌, 훅의 자식인 'targetTransform'으로 설정
        obj.transform.SetParent(this.transform);

        // 위치와 회전을 0으로 초기화
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (obj.TryGetComponent<Collider>(out Collider col))
        {
            // 블럭의 중심에서 상단까지의 거리만큼 아래로 내림
            float yOffset = col.bounds.extents.y;
            obj.transform.localPosition = new Vector3(0, -yOffset, 0);
        }
    }

    public void OnRelease(InputAction.CallbackContext context)
    {
        if (context.started && grabbedObject != null)
        {
            ReleaseObject();
        }
        Invoke("ResetRotation", 1.1f);
    }
    private void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void ReleaseObject()
    {
        lastReleaseTime = Time.time;

        if (grabbedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }

        grabbedObject.transform.SetParent(null);
        grabbedObject = null;
    }
}