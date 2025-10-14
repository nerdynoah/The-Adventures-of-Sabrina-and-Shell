using UnityEngine;

public class PlayerShadow : MonoBehaviour
{

    Vector3 direction = Vector3.down;
    [SerializeField] SpawnShadow spawnShadow;
    private float Speed;

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction);
        float closestDist = int.MaxValue;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("IgnoreRaycast") || hit.collider.CompareTag("JumpCoin") || hit.collider.CompareTag("FootCoin") || hit.collider.CompareTag("GroundPound") || hit.collider.CompareTag("Player") || hit.collider.CompareTag("IgnorePlayerRaycast"))
            {
                continue;
            }
            else
            {
                float temp = hit.distance;
                if (temp < closestDist)
                {
                    closestDist = temp;
                    SpawnShadow shadow = Instantiate(spawnShadow, hit.point, Quaternion.identity);
                    shadow.Setup(temp, 0.12f);
                }
            }

        }

    }
}
