using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTr;

    void Start()
    {
        cameraTr = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(cameraTr);
    }
}
