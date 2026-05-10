using UnityEngine;
using UnityEngine.UI;
// using HorizonBasedAmbientOcclusion.Universal;
using UnityEngine.Rendering;


public class ToggleHBAOButton : MonoBehaviour
{
    
    [SerializeField]
    VolumeProfile postProcessProfile;
    
    Button button;
    Text text;

    // HBAO hbao;

    private bool isOn = true;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPressed);
        Transform t = transform.Find("Text");
        if (t)
            text = t.GetComponent<Text>();
        
        
        /* postProcessProfile.TryGet(out hbao);

        if (hbao != null)
        {
            isOn = hbao.active;
        } 
        else
            Debug.LogError("Could not find text", gameObject);
        */
 
    
        if (text == null) 
            Debug.LogError("Could not find text", gameObject);
        else
            UpdateText();
    }

    // Update is called once per frame
    void OnButtonPressed()
    {
        isOn = !isOn;
        // hbao.EnableHBAO(isOn);
        // hbao.active = isOn;
        
        // mainLight.shadows = (isOn ? LightShadows.None : LightShadows.Soft);

        UpdateText();
    }

    void UpdateText()
    {
        text.text = "HBAO " + (isOn ? "\n[ ON ]" : "\n[ -- ]");
        text.fontStyle = (isOn ? FontStyle.Bold : FontStyle.Normal);
    }



}
