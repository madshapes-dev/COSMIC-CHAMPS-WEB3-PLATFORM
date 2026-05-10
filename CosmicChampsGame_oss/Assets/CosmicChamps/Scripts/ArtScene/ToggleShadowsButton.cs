using UnityEngine;
using UnityEngine.UI;

public class ToggleShadowsButton : MonoBehaviour
{
    
    [SerializeField]
    Light mainLight;
    
    Button button;
    Text text;

    private bool isOn { get { return mainLight.shadows != LightShadows.None; } }

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

        
        mainLight.shadows = (isOn ? LightShadows.None : LightShadows.Soft);

        UpdateText();
    }

    void UpdateText()
    {
        text.text = "Shadows " + (isOn ? "\n[ ON ]" : "\n[ -- ]");
        text.fontStyle = (isOn ? FontStyle.Bold : FontStyle.Normal);
    }
}
