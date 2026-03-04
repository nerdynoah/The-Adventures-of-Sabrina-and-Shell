using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private Vector3 direction = Vector3.down;
    [SerializeField] private SpawnShadow spawnShadow;
    [SerializeField][Range(0f,0.25f)] private float ShadowStaysFor = 0.12f;
    [SerializeField] private float size = 1f;

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction,float.MaxValue, (1 << 0) | (1 << 3));
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
                    shadow.Setup(temp, 0.12f, size);
                }
            }

        }

    }
}
