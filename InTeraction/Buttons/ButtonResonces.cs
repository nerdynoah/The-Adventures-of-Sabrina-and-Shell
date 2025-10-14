using UnityEngine;

public class ButtonResponces : MonoBehaviour
{
    public int type = 0;
    public bool triggered = false;
    private float futureTime = 2.8f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SendButtonCommand(float command)
    {
        if (command == 0 && triggered == false)
        {
            triggered = true;
            futureTime += Time.time;
        }
        if (command == 1 && triggered == false)
        {
            triggered = true;
            futureTime += Time.time;
        }
    }
    void Update()
    {
        if (triggered == true)
        {
            if (Time.time <= futureTime)
            {
                Vector3 tempMove = new(0, 0, 7f);
                tempMove = tempMove * Time.deltaTime;
                transform.Translate(tempMove);
            }
        }
    }
}
