using UnityEngine;
using static Enums;
/// <summary>
/// Make the object disapper
/// </summary>
public class DisappearView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetCameraMode() == CameraMode.FirstPerson)
        {
            gameObject.SetActive(false);
        }

    }


}
