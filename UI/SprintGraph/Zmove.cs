using UnityEngine;

public class ZmoveUI : MonoBehaviour
{
    // Start is called before the first frame update
    private float SpeedMaxThreshold;
    private float SpeedMinThreshold;

    public void SetSpeedThresh(float maxSpeed, float minSpeed)
    {
        SpeedMaxThreshold = maxSpeed;
        SpeedMinThreshold = minSpeed;
    }

    public void ApplyGraph(float speed)
    {
        if (speed < SpeedMinThreshold)
        {
            speed = 0;
        }
        else
        {
            speed = Mathf.Clamp(speed, SpeedMinThreshold, SpeedMaxThreshold);
        }


        transform.localScale = new Vector3(transform.localScale.x, Mathf.Max((speed - SpeedMinThreshold) / (SpeedMaxThreshold - SpeedMinThreshold), 0), transform.localScale.z);
    }


}
