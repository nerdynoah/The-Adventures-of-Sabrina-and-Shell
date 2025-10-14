using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    [SerializeField] Transform target;
    public float rotSpeed = 1.5f;
    public float rotV;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        rotV = transform.eulerAngles.y;
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float horInput = Input.GetAxis("Horizontal");
        if (!Mathf.Approximately(horInput, 0))
        {
            rotV += horInput * rotSpeed * Time.deltaTime;
        }
        else
        {
            rotV += Input.GetAxis("Mouse X") * rotSpeed * 3 * Time.deltaTime;
        }
        Quaternion rotation = Quaternion.Euler(0, rotV, 0);
        transform.position = target.position - (rotation * offset);
        transform.LookAt(target);
    }
}
