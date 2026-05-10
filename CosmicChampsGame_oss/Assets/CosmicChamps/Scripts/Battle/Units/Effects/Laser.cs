using UnityEngine;

namespace CosmicChamps.Battle.Units.Effects
{
    public class Laser : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hitEffect;

        [SerializeField]
        private float _hitOffset;

        [SerializeField]
        private bool _useLaserRotation;

        [SerializeField]
        private float _mainTextureLength = 1f;

        [SerializeField]
        private float _noiseTextureLength = 1f;

        private LineRenderer _laser;
        private Vector4 _length = new(1, 1, 1, 1);
        private bool _laserSaver;
        private bool _updateSaver;
        private ParticleSystem[] _effects;
        private Vector3? _target;
        private bool _stopped;

        private void Start ()
        {
            _laser = GetComponent<LineRenderer> ();
            _effects = GetComponentsInChildren<ParticleSystem> ();
        }

        private void OnEnable ()
        {
            _target = null;
        }

        public void SetTarget (Vector3 target)
        {
            _target = target;
        }

        private void Update ()
        {
            if (!_target.HasValue || _stopped)
                return;

            _laser.material.SetTextureScale ("_MainTex", new Vector2 (_length[0], _length[1]));
            _laser.material.SetTextureScale ("_Noise", new Vector2 (_length[2], _length[3]));

            _laser.SetPosition (0, transform.position);
            _laser.SetPosition (1, _target.Value);

            _hitEffect.transform.position = _target.Value + Vector3.up * _hitOffset;
            if (_useLaserRotation)
                _hitEffect.transform.rotation = transform.rotation;
            else
                _hitEffect.transform.LookAt (_target.Value + Vector3.up);

            foreach (var AllPs in _effects)
            {
                if (!AllPs.isPlaying) AllPs.Play ();
            }

            _length[0] = _mainTextureLength * Vector3.Distance (transform.position, _target.Value);
            _length[2] = _noiseTextureLength * Vector3.Distance (transform.position, _target.Value);
        }

        public void Stop ()
        {
            _stopped = true;
            _laser.enabled = false;
            foreach (var AllPs in _effects)
            {
                if (AllPs.isPlaying) AllPs.Stop ();
            }
        }
    }
}