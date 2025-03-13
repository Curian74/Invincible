using UnityEngine;

public class CameraDemo : MonoBehaviour
{
    [SerializeField] private Transform target;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
