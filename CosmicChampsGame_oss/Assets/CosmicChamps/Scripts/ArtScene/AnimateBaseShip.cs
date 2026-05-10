using UnityEngine;

public class AnimateBaseShip : MonoBehaviour
{

    Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();    
        anim.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
