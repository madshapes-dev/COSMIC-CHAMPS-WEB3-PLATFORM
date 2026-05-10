using System;
using System.Collections;
using System.Linq;
using CosmicChamps.Battle;
using Cysharp.Threading.Tasks;
using Exploder;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (TweenMotion))]
public class ShipFlyController : MonoBehaviour
{
    [Serializable]
    private class Exploder
    {
        [SerializeField]
        private BaseUnitType _type;

        [SerializeField]
        private ExploderObject _exploder;

        [SerializeField]
        private GameObject _effect;

        [SerializeField]
        private float _effectDuration;

        public BaseUnitType Type => _type;

        public void Explode ()
        {
            async UniTaskVoid PlayEffect ()
            {
                if (_effect == null)
                    return;

                _effect.SetActive (true);
                await UniTask.Delay (TimeSpan.FromSeconds (_effectDuration));
                _effect.SetActive (false);
            }

            PlayEffect ().Forget ();
            _exploder.ExplodeRadius ();
        }
    }

    /*[SerializeField]
    bool isPlayer = false;*/

    [SerializeField]
    Animator animator;

    private Vector3 basePosition = new Vector3 ();

    private TweenMotion tweenMotion;

    [SerializeField]
    float startDelay = 0.0f;

    [SerializeField]
    private Exploder[] _exploders;

    [SerializeField]
    BaseUpgradesManager baseUpgradesManager;

    /*[SerializeField]
    CameraSwitchingManager cameraSwitchingManager;*/

    /*[SerializeField]
    CinemachineVirtualCamera launchCam;

    [SerializeField]
    Button launchButton;*/

    [SerializeField]
    GameObject exhaustEffect;

    // Start is called before the first frame update
    void Awake ()
    {
        tweenMotion = GetComponent<TweenMotion> ();
        basePosition = transform.position;
        /*if (launchButton)
            launchButton.gameObject.SetActive(false);*/
        if (exhaustEffect)
            exhaustEffect.gameObject.SetActive (false);
    }

    public void LandShip ()
    {
        baseUpgradesManager.HideUpgrades ();

        if (startDelay > 0.001f)
        {
            // Debug.Log("Delayed Start ", gameObject);
            Invoke ("LandShipInternal", startDelay);
            animator.speed = 1f;
            animator.gameObject.SetActive (false);
        } else
        {
            LandShipInternal ();
        }
    }

    public void LandImmediate ()
    {
        baseUpgradesManager.ShowUpgrades ();
        tweenMotion.StartTweenMotion (0.01f);
        animator.gameObject.SetActive (true);
        animator.speed = float.MaxValue;
    }

    void LandShipInternal ()
    {
        animator.gameObject.SetActive (true);
        animator.speed = 1f;
        animator.SetBool ("Fly", false);
        tweenMotion.StartTweenMotion ();
        StartCoroutine (LandingSequence ());
    }

    IEnumerator LandingSequence ()
    {
        yield return new WaitForSeconds (1.0f);

        if (exhaustEffect)
            exhaustEffect.gameObject.SetActive (true);

        yield return new WaitForSeconds (3.0f);
        baseUpgradesManager.ShowUpgrades ();

        yield return new WaitForSeconds (2.2f);

        /*if (isPlayer)
        {
            if (cameraSwitchingManager.GetCurrentCameraIndex() == 1)
                cameraSwitchingManager.SwitchToNext();
        }*/

        if (exhaustEffect)
            exhaustEffect.gameObject.SetActive (false);
    }


    public void LaunchShip ()
    {
        /*cameraSwitchingManager.SwitchToCustomView(launchCam);
        cameraSwitchingManager.DisableCameraButton();
        cameraSwitchingManager.SetBGActive(true);*/

        StartCoroutine (LaunchSequence ());
    }

    IEnumerator LaunchSequence ()
    {
        yield return new WaitForSeconds (1.1f);
        animator.speed = 1f;
        animator.SetBool ("Fly", true);
        baseUpgradesManager.HideUpgrades ();

        yield return new WaitForSeconds (1.5f);

        if (exhaustEffect)
            exhaustEffect.gameObject.SetActive (true);

        yield return new WaitForSeconds (1.5f);

        tweenMotion.ReverseAndPlay ();
        yield return new WaitForSeconds (2.0f);
        SceneManager.LoadScene (0, LoadSceneMode.Single);
    }

    public void Explode (BaseUnitType type)
    {
        _exploders
            .FirstOrDefault (x => x.Type == type)
            ?.Explode ();
    }
}