using UnityEngine;

public class OutOfBoundsDestroyer : MonoBehaviour
{
    public float yThreshold = 5f; // 이 높이보다 낮아지면 삭제
    
    public ParticleSystem destroyParticle; // 삭제될 때 터뜨릴 이펙트

    void Update()
    {

    }

        private void OnCollisionEnter(Collision collision)
    {
        // 지면(Ground) 태그를 가진 오브젝트에 닿으면 감점
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
            if (destroyParticle != null)
            {
                ParticleSystem effect = Instantiate(destroyParticle, transform.position, Quaternion.Euler(-90, 0, 0));
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }
    }
}