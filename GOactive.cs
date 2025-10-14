using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOActive : MonoBehaviour
{
    [SerializeField] private GameObject[] ObjectToDisable;
    private float Timer = 0;
    private bool EndState;
    /// <summary>
    /// Set active to deactivate or active.
    /// </summary>
    /// <param name="state"></param>
    public void SetState(bool state)
    {
        foreach (var obj in ObjectToDisable)
        {
            obj.SetActive(state);
            EndState = state;
        }
    }
    public void DestroyObject()
    {
        foreach (var obj in ObjectToDisable)
        {
            Destroy(obj);
        }
    }
    public void SwitchStateAfterTime(float time)
    {
        Timer = Time.time + time;

    }
    private void Update()
    {
        if (!Mathf.Approximately(Timer,0))
        {
            if (Time.time > Timer)
            {
                SetState(!EndState);
                Timer = 0;
            }
        }
    }

}
