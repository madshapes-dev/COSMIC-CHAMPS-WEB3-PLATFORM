using UnityEngine;

public class TogglePassage : MonoBehaviour
{

    [SerializeField]
    GameObject beforeParent;
    [SerializeField]
    GameObject afterParent;

    Transform[] before;
    Transform[] after;

    [SerializeField]
    bool toggleOnTimer = false;

    bool isToggled = false;

    // Start is called before the first frame update
    void Start()
    {
        before = beforeParent.GetComponentsInChildren<Transform>();
        after = afterParent.GetComponentsInChildren<Transform>();
        
        if (toggleOnTimer)
            InvokeRepeating("Toggle", 10f, 10f);
    
        SetState(isToggled);
    }

    public void Toggle()
    {
        SetState(!isToggled);
    }

    public void SetState(bool state)
    {
        isToggled = state;
 
        foreach (Transform t in before)
            t.gameObject.SetActive(!isToggled);
        foreach (Transform t in after)
            t.gameObject.SetActive(isToggled);
    }
}
