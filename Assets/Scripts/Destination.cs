using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 트럭 태그를 가진 물체가 들어오면 게임 종료
        if (other.CompareTag("Player"))
        {
            TransportGameManager.Instance.FinishGame();
        }
    }
}
