using UnityEngine;

public class RayShooters : MonoBehaviour
{
    [SerializeField] Camera cam;
    public float CrossSize = 100;
    private readonly float DELAY = 0.01f;
    private float timedelay = 0;
    [SerializeField] PewPewAnimation pewPewAnimation;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > timedelay)
        {

            Vector3 point = new Vector3((cam.pixelWidth) / 2, (cam.pixelHeight) / 2, 0.5f);
            Ray ray = cam.ScreenPointToRay(point);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
            bool DidhitObject = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("IgnoreRaycast") || hit.collider.CompareTag("IgnorePlayerRaycast") || hit.collider.CompareTag("player"))
                {
                    //Debug.Log("Ran into IgnorableObject");
                    continue;
                }
                else
                {
                    //Debug.Log("Object ran into");
                    timedelay = Time.time + DELAY;
                    GameObject hitObject = hit.transform.gameObject;
                    Buttons target = hitObject.GetComponent<Buttons>();

                    //PewPewAnimation pewpew = Instantiate(pewPewAnimation, transform.position, transform.rotation);
                    //pewpew.Setup(1, 90f, 0.5f, 4f, hitObject.transform.position);
                    if (target == true && DidhitObject == false)
                    {
                        DidhitObject = true;
                        //Debug.Log("Hit Button");
                        target.ReactToHit();
                    }
                }

            }


        }
    }



    private Color rectColor = new(0.88f, 0.07f, 0.33f);
    private void OnGUI()
    {
        // Save the current GUI color
        Color oldColor = GUI.color;
        // Set the new GUI color
        GUI.color = rectColor;


        float posX = cam.pixelWidth / 2;
        float posY = cam.pixelHeight / 2;
        Rect rect = new(posX, posY, CrossSize, CrossSize);
        GUI.Label(rect, "x");


        GUI.color = oldColor;
    }
}
