using UnityEngine;

public class PewPewAnimation : MonoBehaviour
{

    protected int Type { get; set; } = 0;
    protected float Distance { get; set; } = 3.0f;
    protected float Speed { get; set; } = 1.0f;
    protected Vector3 Wantedarea { get; set; }
    protected Vector3 StartingArea { get; set; }
    [SerializeField] Material materal;

    /// <summary>
    /// Setup the visual projectile
    /// </summary>
    /// <param name="which">What mode, 1 = far attack, 2 = close attack</param>
    /// <param name="distance">How far does it travel</param>
    /// <param name="hitLocation">Where was the ray hit</param>
    /// <param name="size">How big is the projectile</param>
    public void Setup(int which, float distance, float size, float speed, Vector3 hitLocation)
    {
        if (which == 1)
        {
            materal.color = new(0.1f, 1, 0.8f);
            Type = 1;
        }
        else if (which == 2)
        {
            materal.color = new(1f, 0.5f, 1f);
            Type = 2;
        }

        transform.localScale = new(size, size * 1.5f, size);
        Distance = distance;
        StartingArea = transform.position;
        Wantedarea = hitLocation;
        Speed = speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Type > 0)
        {
            transform.Translate(0, 0, Speed * Time.deltaTime);
            Vector3 difference = transform.localPosition - StartingArea;
            difference = new Vector3(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
            float mag = difference.magnitude;
            if (mag > Distance)
            {
                Destroy(gameObject);
            }

            float temp = Mathf.Abs(transform.localPosition.magnitude) - Wantedarea.magnitude;
            if (temp < 0.1f)
            {
                Destroy(gameObject);
            }
        }

    }
}
