using UnityEngine;

public class RotateObjectJAM : MonoBehaviour
{

    [SerializeField]
    float speed = 15.0f;
    
        
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * speed, Space.World);
    }
}

