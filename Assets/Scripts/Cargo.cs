using UnityEngine;

public class Cargo : MonoBehaviour
{
    private bool hasDropped = false;
    public ParticleSystem destroyParticle;

    private void OnCollisionEnter(Collision collision)
    {
        // 지면(Ground) 태그를 가진 오브젝트에 닿으면 감점
        if (!hasDropped && collision.gameObject.CompareTag("Ground"))
        {
            hasDropped = true;
            TransportGameManager.Instance.DecreaseScore(100);
            
            // 2초 뒤에 짐 제거 (최적화)
            Destroy(gameObject);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBreakSound(transform.position);
            }
            if (destroyParticle != null)
            {
                ParticleSystem effect = Instantiate(destroyParticle, transform.position, Quaternion.Euler(-90, 0, 0));
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
    }
}
