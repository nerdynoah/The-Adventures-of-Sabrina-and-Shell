using UnityEngine;

public class SpawnShadow : MonoBehaviour
{
    private float SpawnTime { get; set; }
    private Color shadowColor;
    public Material shadow;
    // Start is called before the first frame update
    void Start()
    {

    }
    /// <summary>
    /// Spawn the Shadow with attributes to adjust transperency
    /// </summary>
    /// <param name="distance">How far away is the shadow</param>
    /// <param name="SpawnTimeBase">How many frames does the shadow last. <code>spawnTime = SpawnTimeBase * Time.deltaTime + Time.time;</code></param>
    public void Setup(float distance, float SpawnTimeBase)
    {
        SpawnTime = SpawnTimeBase * Time.deltaTime + Time.time;
        transform.localScale = new Vector3(Mathf.Lerp(1.1f, 0.6f, distance / 200), Mathf.Lerp(0.3f, 10f, distance / 1000), Mathf.Lerp(1.1f, 0.6f, distance / 200));
        distance *= 0.7f;
        distance = Mathf.Max(0.01f, distance);
        distance = Mathf.Clamp((12 / distance) - 0.3f, 0.33f, 0.7f);
        shadowColor = new(0, 0, 0, distance);
        shadow.color = shadowColor;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > SpawnTime)
        {
            Destroy(gameObject);
        }

    }
}
