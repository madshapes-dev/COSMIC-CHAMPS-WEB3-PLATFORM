using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShowSequence : MonoBehaviour
{

    List<Transform> children = new List<Transform>();
    List<Transform> cameras = new List<Transform>();

    [SerializeField]
    float debugTimeScale = 1.0f;

    int currentChild = 0;
    [SerializeField]
    float holdTime = 3f;

    float countDown = 5f;

    [SerializeField]
    Transform camerasRoot;

    [SerializeField]
    int[] cameraTriggers;

    int nextCameraTrigger = 0;

    [SerializeField]
    float startDelay = 0.5f;

    [SerializeField]
    bool waitForClick = false; 

    bool hasStarted = false;

    [SerializeField]
    ParticleSystem particleIn;
    ParticleSystem.ShapeModule particleShapeIn;
    [SerializeField]
    ParticleSystem particleOut;
    ParticleSystem.ShapeModule particleShapeOut;
    
    [SerializeField]
    UnityEvent onCameraSwitch;
    
    // Start is called before the first frame update
    void Start()
    {
         for (int i = 0; i < transform.childCount; ++i)
         {
            children.Add(transform.GetChild(i));
            transform.GetChild(i).gameObject.SetActive(false);
         }
         for (int i = 0; i < camerasRoot.childCount; ++i)
         {             
             cameras.Add(camerasRoot.GetChild(i));
             camerasRoot.GetChild(i).gameObject.SetActive(false);
         }
         cameras[0].gameObject.SetActive(true);
         Debug.Log("Camera Sequence: Added " + cameras.Count);


        particleShapeIn = particleIn.shape;
        particleShapeOut = particleOut.shape;

        if (!waitForClick)
        {
            Invoke("StartDelayed", startDelay);
            hasStarted = true;
        }
        
        if (!Mathf.Approximately(debugTimeScale, 1.0f))
            Time.timeScale = debugTimeScale;

    }

    void StartDelayed()
    {   
        PlayAppearEffect();
        EnableCamera(0);
        countDown = holdTime;
        hasStarted = true;
    }

    void Update()
    {

        countDown -= Time.deltaTime;

        if (countDown <= 0f) {
            previousChild = currentChild;
            currentChild++;
            if (currentChild >= children.Count)
            {
                // final sequence 

                // children[currentChild-1].gameObject.SetActive(false);        
                EnableCamera(cameras.Count-1);
                DisablePreviousChild();
                enabled = false;
                return;
            }
            
            EnableChild(currentChild);

            countDown = holdTime;

            Debug.Log("checking camera trigger: " + nextCameraTrigger);


            if (cameraTriggers[nextCameraTrigger] == currentChild)
            {
                nextCameraTrigger++;
                EnableCamera(nextCameraTrigger);
            }

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!Mathf.Approximately(debugTimeScale, 1.0f))
            {
                Debug.Log("Resetting Time Scale to 1.0");
                Time.timeScale = 1.0f;

            }
        }

        if (!hasStarted && waitForClick) 
        {
            if (Input.GetMouseButton(0))
            {
                StartDelayed();
            }

        }
    }

    int previousChild = 0;

    void EnableChild(int childIndex)
    {
        // previousChild = currentChild;
        currentChild = childIndex;
        Invoke("DisablePreviousChild", 0.33f); // first delay allows the camera to start zooming out before...
    }

    void DisablePreviousChild()
    {
        if (children.Count == 0 || particleIn == null || particleOut == null)
            return;
        children[previousChild].gameObject.SetActive(false);        
        particleShapeOut.mesh = particleShapeIn.mesh;
        particleOut.transform.rotation = children[previousChild].rotation;
        particleOut.Play();

        Invoke("PlayAppearEffect", 0.250f); // this delay is the invisible time between sequence steps
    }

    void PlayAppearEffect() 
    {
        if (children.Count == 0 || particleIn == null || particleOut == null)
            return;        
        Mesh prmesh = children[currentChild].gameObject.GetComponent<MeshFilter>().sharedMesh;
        particleShapeIn.mesh = prmesh;
        Debug.Log("setting mesh to " + prmesh.name, prmesh);
        particleIn.transform.rotation = children[currentChild].rotation;
        particleIn.Play();

        Invoke("EnableCurrentChild", 0.142f); // this delay is how much before appearing to start effect

    }

    void EnableCurrentChild() {
        Debug.Log("Sequence EnableChild: " + currentChild);

        for (int i = 0; i < transform.childCount; ++i)
        {
            children[i].gameObject.SetActive(i == currentChild);        
        }

    }

    void EnableCamera(int cameraIndex)
    {
        Debug.Log("EnableCamera: "+ cameraIndex + " - " + cameras[cameraIndex].gameObject.name);
        onCameraSwitch.Invoke();
        for (int i = 0; i < camerasRoot.childCount; ++i)
        {        
            cameras[i].gameObject.SetActive(i == cameraIndex);     
        }
    }    
}
