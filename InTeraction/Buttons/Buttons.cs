using UnityEngine;

public class Buttons : MonoBehaviour
{
    public int type = 0;
    private bool triggered = false;
    private bool canTrigger = false;
    private Color color;
    public Material mat;
    [SerializeField] ButtonResponces push;
    private void Start()
    {
        color = new Color(0.3f, 0.8f, 0.8f);
        mat.color = color;
    }
    public void ReactToHit()
    {
        if (triggered == false && canTrigger == true)
        {
            triggered = true;
            push.SendButtonCommand(type);
            Debug.Log("Send Command");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        canTrigger = true;
        color = new Color(1f, 1f, 1f);
        mat.color = color;
    }
    private void OnTriggerExit(Collider other)
    {
        canTrigger = false;
        color = new Color(0.3f, 0.8f, 0.8f);
        mat.color = color;
    }
}