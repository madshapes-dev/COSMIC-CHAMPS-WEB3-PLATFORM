using UnityEngine;
using UnityEngine.UI;

public class TogglePartsButton : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    
    Button button;
    Text text;

    private bool isOn { get { return target.gameObject.activeSelf; } }

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
        Transform t = transform.Find("Text");
        if (t)
            text = t.GetComponent<Text>();
        
        if (text == null) 
            Debug.LogError("Could not find text", gameObject);
        else
            UpdateText();
    }

    // Update is called once per frame
    void OnButtonPressed()
    {

        target.gameObject.SetActive(!target.gameObject.activeSelf);
        UpdateText();
    }

    void UpdateText()
    {
        text.text = target.name + (isOn ? "\n[ ON ]" : "\n[ -- ]");
        text.fontStyle = (target.gameObject.activeSelf ? FontStyle.Bold : FontStyle.Normal);
    }
}
