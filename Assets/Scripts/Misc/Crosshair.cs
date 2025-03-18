using UnityEngine;

public class Crosshair : MonoBehaviour
{
    void Start()
    {
        
    }

    void LateUpdate()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; 

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.position = worldPosition;

        transform.rotation = Quaternion.identity;
    }
}
