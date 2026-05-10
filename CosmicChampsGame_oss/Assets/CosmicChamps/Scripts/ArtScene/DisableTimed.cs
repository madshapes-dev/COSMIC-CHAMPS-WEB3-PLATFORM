using UnityEngine;

public class DisableTimed : MonoBehaviour
{
    [SerializeField]
    GameObject[] disableGameObjects;

    [SerializeField]
    float duration = 3f;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in disableGameObjects)
        {
            go.SetActive(false);
        }
        Invoke("EnableDelayed", duration);
    }

    void EnableDelayed()
    {
        foreach(GameObject go in disableGameObjects)
        {
            go.SetActive(true);
        }        
    }
}
