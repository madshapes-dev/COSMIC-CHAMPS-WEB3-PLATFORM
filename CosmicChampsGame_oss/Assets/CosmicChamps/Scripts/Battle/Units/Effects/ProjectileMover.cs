using UnityEngine;


namespace CosmicChamps.Battle.Units.Effects
{
    public class ProjectileMover : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 15f;

        [SerializeField]
        private float _hitOffset;

        [SerializeField]
        private bool _useFirePointRotation;

        [SerializeField]
        private Vector3 _rotationOffset = new(0, 0, 0);

        [SerializeField]
        private GameObject _hit;

        [SerializeField]
        private GameObject _flash;

        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private GameObject[] _detached;

        private Vector3 origin;
        private Vector3? _target;

        private void Start ()
        {
            origin = transform.position;

            if (_flash != null)
            {
                //Instantiate flash effect on projectile position
                var flashInstance = Instantiate (_flash, transform.position, Quaternion.identity);
                flashInstance.transform.forward = gameObject.transform.forward;

                //Destroy flash effect depending on particle Duration time
                var flashPs = flashInstance.GetComponent<ParticleSystem> ();
                if (flashPs != null)
                {
                    Destroy (flashInstance, flashPs.main.duration);
                } else
                {
                    var flashPsParts = flashInstance.transform.GetChild (0).GetComponent<ParticleSystem> ();
                    Destroy (flashInstance, flashPsParts.main.duration);
                }
            }

            Destroy (gameObject, 5);
        }

        private void FixedUpdate ()
        {
            if (_speed != 0)
            {
                _rb.linearVelocity = transform.forward * _speed;
                //transform.position += transform.forward * (speed * Time.deltaTime);         
            }

            if (!_target.HasValue)
                return;

            if ((_target.Value - origin).magnitude > (transform.position - origin).magnitude)
                return;

            HandleCollision (transform.position, Vector3.up);
        }

        private void HandleCollision (Vector3 point, Vector3 normal)
        {
            //Lock all axes movement and rotation
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            _speed = 0;

            // ContactPoint contact = collision.contacts[0];
            var rot = Quaternion.FromToRotation (Vector3.up, normal);
            var pos = point + normal * _hitOffset;

            //Spawn hit effect on collision
            if (_hit != null)
            {
                var hitInstance = Instantiate (_hit, pos, rot);
                if (_useFirePointRotation)
                {
                    hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler (0, 180f, 0);
                } else if (_rotationOffset != Vector3.zero)
                {
                    hitInstance.transform.rotation = Quaternion.Euler (_rotationOffset);
                } else
                {
                    hitInstance.transform.LookAt (point + normal);
                }

                //Destroy hit effects depending on particle Duration time
                var hitPs = hitInstance.GetComponent<ParticleSystem> ();
                if (hitPs != null)
                {
                    Destroy (hitInstance, hitPs.main.duration);
                } else
                {
                    var hitPsParts = hitInstance.transform.GetChild (0).GetComponent<ParticleSystem> ();
                    Destroy (hitInstance, hitPsParts.main.duration);
                }
            }

            //Removing trail from the projectile on cillision enter or smooth removing. Detached elements must have "AutoDestroying script"
            foreach (var detachedPrefab in _detached)
            {
                if (detachedPrefab != null)
                {
                    detachedPrefab.transform.parent = null;
                }
            }

            //Destroy projectile on collision
            Destroy (gameObject);
        }

        public void SetTarget (Vector3 target)
        {
            _target = target;
        }
    }
}