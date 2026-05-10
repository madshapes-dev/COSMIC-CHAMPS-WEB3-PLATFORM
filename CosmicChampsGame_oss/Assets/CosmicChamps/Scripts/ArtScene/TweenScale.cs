using System.Collections;
using UnityEngine;

public class TweenScale : MonoBehaviour
{

    private Vector3 startScale;
    private Vector3 endScale;

    [SerializeField]
    Vector3 targetScale;
    
    [SerializeField]
    bool relative = false;

    [SerializeField]
    bool endAtCurrentPosition = false;

    [SerializeField]
    AnimationCurve animCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

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
            startScale = targetScale + (relative ? transform.localScale : Vector3.zero);
            endScale = transform.localScale;
            transform.localScale = startScale;
        }
        else
        {
            startScale = transform.localScale;
            endScale = targetScale + (relative ? transform.localScale : Vector3.zero);
        }
    
        // Debug.Log("PlayTweenScale: " + gameObject.name + " start: " + startScale + " end: " + endScale, gameObject);
    }
    
    void Start() {
        if (autoStart)
            StartCoroutine(PlayTweenMotion());
    }

    void OnEnable() {
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return; // Skip first enable, because Start works better for that.
        }  
        if (autoStart)
            StartCoroutine(PlayTweenMotion());          
    }



    public void StartTweenMotion() 
    {
        StartCoroutine(PlayTweenMotion());
    }

    IEnumerator PlayTweenMotion()
    {
        // Debug.Log("PlayTweenMotion: " + gameObject.name + " start: " + startScale + " end: " + endScale, gameObject);

        float t = 0;

        while (t < duration) {           
            t += Time.deltaTime;
            float val = t/duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, animCurve.Evaluate(val));
            // Debug.Log("T=" + t + " val:" + val + " animCurve: " + animCurve.Evaluate(val));
            yield return null;
        }

        // yield return null;
        
        if (loop)
            ReverseAndPlay();

    }

    void ReverseAndPlay()
    {
            Vector3 prevStartScale = startScale;
            startScale = endScale;
            endScale = prevStartScale;
            transform.localScale = startScale;
            StartCoroutine(PlayTweenMotion());

    }

}
