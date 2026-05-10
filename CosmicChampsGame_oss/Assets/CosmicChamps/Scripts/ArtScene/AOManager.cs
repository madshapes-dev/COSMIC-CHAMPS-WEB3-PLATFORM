using UnityEngine;
// using HorizonBasedAmbientOcclusion.Universal;


public class AOManager : MonoBehaviour
{

    /*public static int defaultRenderer { get { return m_defaultRenderer; } }
    private static int m_defaultRenderer = 0;

    [SerializeField]
    VolumeProfile postProcessProfile;
   
    [SerializeField]
    Camera mainCamera;
    
    private UniversalAdditionalCameraData camData;

    // private HBAO hbao;
    private enum AOMethod
    {
        SSAO,
        HBAO
    }
    [SerializeField]
    AOMethod method = AOMethod.SSAO;

    public bool onMobile = false;
    public bool onOther = true;

    [SerializeField]
    RenderPipelineAsset mobilePipelineAsset;
    [SerializeField]
    RenderPipelineAsset ultraPipelineAsset;

    void Awake() {
        if (method == AOMethod.SSAO)
            camData = mainCamera.GetUniversalAdditionalCameraData();
        else
            // postProcessProfile.TryGet(out hbao);
            Debug.LogWarning("HBAO disabled, uncomment HBAO code to re-activate it", gameObject);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR          
            // disable by default on mobile
            m_defaultRenderer = onMobile ? 0 : 1;
            if (hbao != null)
            {
                hbao.EnableHBAO(onMobile);
                hbao.active = onMobile;
            } 
            else if (usingSSAO)
            {
                Debug.Log("Setting AO: " + (m_defaultRenderer == 0 ? "on" : "off"));
                camData.SetRenderer(m_defaultRenderer); // switch to renderer without/with ao.
            }
#else
            // enable by default on editor / pc
            m_defaultRenderer = onOther ? 0 : 1;
            if (method == AOMethod.HBAO)
            {            
                Debug.LogWarning("HBAO disabled, uncomment HBAO code to re-activate it", gameObject);               
                // hbao.EnableHBAO(onOther);
                // hbao.active = onOther;
            }
            else if (method == AOMethod.SSAO)
            {   
                Debug.Log("Setting SSAO: " + (m_defaultRenderer == 0 ? "on" : "off"));
                camData.SetRenderer(m_defaultRenderer); // switch to renderer without/with ao.                              
            }

#endif

            // Debug.LogError("Couldnt find HBAO");
    }*/

}
