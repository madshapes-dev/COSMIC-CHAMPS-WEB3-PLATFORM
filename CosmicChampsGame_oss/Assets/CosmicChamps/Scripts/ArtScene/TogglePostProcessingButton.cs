using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class TogglePostProcessingButton : MonoBehaviour
{
    
    [SerializeField]
    Camera mainCamera;
    
    Button button;
    Text text;

    private bool isOn { get { return mainCamera.GetUniversalAdditionalCameraData().renderPostProcessing; } }

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
        mainCamera.GetUniversalAdditionalCameraData().renderPostProcessing = !isOn;
        UpdateText();
    }

    void UpdateText()
    {
        text.text = "Post Processing " + (isOn ? "\n[ ON ]" : "\n[ -- ]");
        text.fontStyle = (isOn ? FontStyle.Bold : FontStyle.Normal);
    }
}
