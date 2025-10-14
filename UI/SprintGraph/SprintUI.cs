using UnityEngine;

public class SprintUI : MonoBehaviour
{
    private float SprintMax = 1;
    public float PresetScale = 100;
    // Start is called before the first frame update

    public void SetSprintUI(float sprint)
    {
        if (sprint <= 1)
        {
            transform.localScale = new(transform.localScale.x, 0, transform.localScale.z);
        }
        else
        {
            transform.localScale = new(transform.localScale.x, sprint / SprintMax, transform.localScale.z);
        }

    }
    public void SetupSprint(float sprint, float maxSprint)
    {
        SprintMax = maxSprint;
        float temp = ((PresetScale / maxSprint) * 2.5f) + 0.5f;
        transform.localScale = new(transform.localScale.x, sprint / SprintMax, transform.localScale.z);
        transform.parent.localScale = new Vector3(transform.parent.localScale.x, temp, transform.parent.localScale.z);
    }

}
