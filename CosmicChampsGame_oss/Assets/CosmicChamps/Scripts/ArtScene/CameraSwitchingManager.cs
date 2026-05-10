using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.UI;


public class CameraSwitchingManager : MonoBehaviour
{

    private List<Transform> cameras = new List<Transform>();

    [SerializeField]
    float debugTimeScale = 1.0f;

    int currentCamera = 0;
    int previousCamera = 0;

    float pressHoldTime = 0f;

    [SerializeField]
    Transform camerasRoot;

    [SerializeField]
    float startDelay = 0.5f;

    bool hasStarted = false;
   
    [SerializeField]
    UnityEvent onCameraSwitch;

    [SerializeField]
    CinemachineBrain mainBrain;
    
    bool isInGameView = false;

    /*[SerializeField]
    bool disableBGInGameView = true;
    */
    [SerializeField]
    GameObject[] disableInGameView;

    CinemachineVirtualCamera currentCam;

    [SerializeField]
    bool disableMovementNoise = false;

    [SerializeField]
    Button cameraButton;

    [SerializeField]
    GameObject worldRoot;
    [SerializeField]
    Vector3 worldOtrhoScale = new Vector3(1f, 1.241f, 1.2f);
    [SerializeField]
    Vector3 worldOtrhoPos = new Vector3(0f, -1.35f, 0f);

    bool isOrtho = false;

    // Start is called before the first frame update
    void Awake()
    {
         for (int i = 0; i < camerasRoot.childCount; ++i)
         {             
             cameras.Add(camerasRoot.GetChild(i));
             camerasRoot.GetChild(i).gameObject.SetActive(false);
         }
         cameras[0].gameObject.SetActive(true);
         currentCam = cameras[0].GetComponent<CinemachineVirtualCamera>();
         // Debug.Log("Camera Switcher: Added " + cameras.Count + " cameras");

        if (!Mathf.Approximately(debugTimeScale, 1.0f))
            Time.timeScale = debugTimeScale;

        Invoke("StartDelayed", startDelay);
    }

    void Start()
    {
        // DisableCameraButton();
        SetBGActive(true, false);
        FadeFogTo(closeFogNear, closeFogFar, 6f, 2.2f);
    }

    void StartDelayed()
    {   
        EnableCamera(1);
        hasStarted = true;
    }


    private bool isFastFwd = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            Time.timeScale = 6.0f;
            isFastFwd = true;
        } 
        else if (isFastFwd && (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt)))
        {
            Time.timeScale = 1.0f;
            isFastFwd = false;
        }

        // if (EventSystem.current.IsPointerOverGameObject())
            // return;

    }
    /*
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!Mathf.Approximately(debugTimeScale, 1.0f))
            {
                Debug.Log("Resetting Time Scale to 1.0");
                Time.timeScale = 1.0f;

            }
        }
    */
    // sent from a button...

    public void OnPressed() 
    {
        if (Input.GetMouseButton(0))
            pressHoldTime += Time.deltaTime;
        else if (!Input.GetMouseButtonUp(0))
            pressHoldTime = 0f;

        // countDown -= Time.deltaTime;
        if (Input.GetMouseButtonUp(0) && hasStarted && pressHoldTime < 0.3f)       
        {
            SwitchToNext();
        }

    }
    void OnDoubleTap()
    {

        SwitchToNext();
 
    }


    public void SwitchToNext() {

        previousCamera = currentCamera;
        
        currentCamera++;
        if (currentCamera > cameras.Count - 1)
        {
            currentCamera = 2; // skip 'start' camera 0 & 'landing' camera 1

        }

        EnableCamera(currentCamera);

    }


    void EnableCamera(int cameraIndex)
    {
        currentCamera = cameraIndex;

        Debug.Log("Camera: "+ cameraIndex + " - " + (cameraIndex > 0 ? cameras[cameraIndex].gameObject.name : "CUSTOM"));
        onCameraSwitch.Invoke();
        for (int i = 0; i < camerasRoot.childCount; ++i)
        {        
            cameras[i].gameObject.SetActive(i == cameraIndex);     
        }

        if (cameraIndex > -1)
            currentCam = cameras[cameraIndex].GetComponent<CinemachineVirtualCamera>();
        
        if (currentCam.m_Lens.Orthographic)
        {
            // ortho view
            worldRoot.transform.localPosition = worldOtrhoPos;
            worldRoot.transform.localScale = worldOtrhoScale;
        } 
        else if (isOrtho)
        {           
            // (was ortho) exiting ortho
            worldRoot.transform.localPosition = new Vector3(0f,0f,0f);
            worldRoot.transform.localScale = new Vector3(1f,1f,1f);
        }
        isOrtho = currentCam.m_Lens.Orthographic;

        if (disableMovementNoise)
        {
            CinemachineBasicMultiChannelPerlin noise = currentCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); 
            if (noise) {
                noise.m_NoiseProfile = null;
                noise.m_AmplitudeGain = 0;
                noise.m_FrequencyGain = 0;
            }
        }
        // Special case crap
        if (cameraIndex > 1) 
        {
            mainBrain.m_DefaultBlend.m_Time = 0.95f;
            mainBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        }
        if (cameraIndex == 1 && !isInGameView)
            Invoke("SetGameView", 7f);
        else if (cameraIndex == 6 || cameraIndex == 11) // CM vcam MANUAL (BASE A VIEW)
            SetBGActive(true);
        else
            SetBGActive(false);

    }    

    void SetGameView() 
    {
        return;
        /*Debug.Log("Setting game view, (disabling background & sky stuff)", gameObject);
        if (disableBGInGameView)
        {
            SetBGActive(false);
        }
        isInGameView = true;
        */
    }

    // [SerializeField]
    float closeFogNear = 250;
    // [SerializeField]
    float closeFogFar = 600;
    // [SerializeField]
    float wideFogNear = 160;
    // [SerializeField]
    float wideFogFar = 500;

    public void SetBGActive(bool state, bool fadeFog = true, float delay = 0f) {
        // Debug.Log("Set BG Active:" + state);
        foreach(GameObject go in disableInGameView) 
        {
            go.SetActive(state);
        }
        float nearDistance = state ? wideFogNear : closeFogNear;
        float farDistance =  state ? wideFogFar : closeFogFar;
        if (fadeFog)
        {
            FadeFogTo(nearDistance, farDistance, 3f, delay);
        }
        else
        {
            RenderSettings.fogStartDistance = nearDistance;
            RenderSettings.fogEndDistance = farDistance;
        }
    }

    public int GetCurrentCameraIndex() 
    {
        return currentCamera;
    }

    public void SwitchToCustomView(CinemachineVirtualCamera vcam)
    {
        EnableCamera(-1);
        currentCam = vcam;
        vcam.gameObject.SetActive(true);

    }

    public void DisableCameraButton() 
    {
        cameraButton.gameObject.SetActive(false);
    }

    Coroutine fadeCoroutine = null;

	void FadeFogTo(float near, float far, float lerpTime, float delay)
    {
		if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
		fadeCoroutine = StartCoroutine(FadingFog(near, far, lerpTime, delay));
	}

	private IEnumerator FadingFog(float near, float far, float time, float delay)
    {

		float elapsedTime = 0;

        if (delay > 0.01f)
        {
            yield return new WaitForSeconds(delay);
        }

		while (elapsedTime < time) {
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / time;
            // RenderSettings.fogColor = Color.Lerp(c1, c2, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, near, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, far, t);
            // Debug.Log("Fading fog " + t + " " + near + " " + far);
            yield return null;
		}

		RenderSettings.fogStartDistance = near;
		RenderSettings.fogEndDistance = far;
		
	}

}
