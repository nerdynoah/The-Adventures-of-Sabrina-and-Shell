using UnityEngine;

public class Spinnnn : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 360 * Time.deltaTime);
    }
}
