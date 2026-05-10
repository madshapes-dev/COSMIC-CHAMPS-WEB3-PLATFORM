using System.Collections;
using UnityEngine;

public class TweenMotion : MonoBehaviour
{

    private Vector3 startPosition;
    private Vector3 endPosition;

    [SerializeField]
    Vector3 targetPosition;
    
    [SerializeField]
    bool relative = false;

    [SerializeField]
    bool endAtCurrentPosition = false;

    [SerializeField]
    AnimationCurve animCurve = new AnimationCurve();

    [SerializeField]
    float duration = 2f;

    [SerializeField]
    bool autoStart = false;

    [SerializeField]
    bool loop = false;

    private bool isFirstEnable = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (endAtCurrentPosition)
        {
            startPosition = targetPosition + (relative ? transform.position : Vector3.zero);
            endPosition = transform.position;
            transform.position = startPosition;
        }
        else
        {
            startPosition = transform.position;
            endPosition = targetPosition + (relative ? transform.position : Vector3.zero);
        }
    
        // Debug.Log("PlayTweenMotion: " + gameObject.name + " start: " + startPosition + " end: " + endPosition, gameObject);
    }
    
    void Start() {
        if (autoStart)
            StartCoroutine(PlayTweenMotion(duration));
    }

    void OnEnable() {
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return; // Skip first enable, because Start works better for that.
        }  
        if (autoStart)
            StartCoroutine(PlayTweenMotion(duration));          
    }

    public void StartTweenMotion() 
    {
        StartCoroutine(PlayTweenMotion(duration));
    }
    
    public void StartTweenMotion(float overrideDuration) 
    {
        StartCoroutine(PlayTweenMotion(overrideDuration));
    }

    IEnumerator PlayTweenMotion(float duration)
    {
        // Debug.Log("PlayTweenMotion: " + gameObject.name + " start: " + startPosition + " end: " + endPosition, gameObject);

        float t = 0;

        while (t < duration) {           
            t += Time.deltaTime;
            float val = t/duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, animCurve.Evaluate(val));
            // Debug.Log("T=" + t + " val:" + val + " animCurve: " + animCurve.Evaluate(val));
            yield return null;
        }

        // yield return null;
        
        if (loop)
            ReverseAndPlay();

    }

    public void ReverseAndPlay()
    {
            Vector3 prevStartPosition = startPosition;
            startPosition = endPosition;
            endPosition = prevStartPosition;
            transform.position = startPosition;
            StartCoroutine(PlayTweenMotion(duration));

    }

}
