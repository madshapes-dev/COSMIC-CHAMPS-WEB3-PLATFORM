using UnityEngine;


public class ToggleSSAOButton : MonoBehaviour
{
    
    /*[SerializeField]
    Camera mainCamera;
    
    private UniversalAdditionalCameraData camData;

    Button button;
    Text text;

    private bool isOn = true;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
        Transform t = transform.Find("Text");
        if (t)
            text = t.GetComponent<Text>();
    }

    void Start()
    {
        isOn = AOManager.defaultRenderer == 0;
        camData = mainCamera.GetUniversalAdditionalCameraData();
        SetAO(isOn);

        if (text == null) 
            Debug.LogError("Could not find text", gameObject);
        else
            UpdateText();
    }

    void OnEnable() {
        UpdateText();
    }

    // Update is called once per frame
    void OnButtonPressed()
    {
        SetAO(!isOn);
        // hbao.EnableHBAO(isOn);
        // hbao.active = isOn;
        
        // mainLight.shadows = (isOn ? LightShadows.None : LightShadows.Soft);

        UpdateText();
    }

    void SetAO(bool state) {
        isOn = state;
        Debug.Log("Set AO: " + state);
        camData = mainCamera.GetUniversalAdditionalCameraData();        
        camData.SetRenderer(isOn ? 0 : 1); // switch to renderer without/with render feature.
    }

    void UpdateText()
    {
        text.text = "SSAO " + (isOn ? "\n[ ON ]" : "\n[ -- ]");
        text.fontStyle = (isOn ? FontStyle.Bold : FontStyle.Normal);
    }*/



}
