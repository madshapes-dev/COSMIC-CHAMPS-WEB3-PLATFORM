using System;
using UniRx;
using UnityEngine;

namespace CosmicChamps.Battle.Units.Effects
{
    public class TurretAttackEffect : MonoBehaviour
    {
        [SerializeField]
        private Laser _laserPrefab;

        [SerializeField]
        private float _duration = 0.2f;

        private Laser _laser;
        private IDisposable _deactivateDisposable;

        private void OnDestroy ()
        {
            Cleanup (0f);
        }

        private void Cleanup (float destroyDelay)
        {
            _deactivateDisposable?.Dispose ();
            if (_laser == null)
                return;

            _laser.Stop ();
            Destroy (_laser.gameObject, destroyDelay);
        }

        private void Deactivate ()
        {
            Cleanup (1f);
        }

        public void Activate (Transform muzzle, Vector3 targetPosition)
        {
            Cleanup (0f);

            _laser = Instantiate (_laserPrefab);
            _laser.transform.position = muzzle.position;
            _laser.SetTarget (targetPosition);

            _deactivateDisposable = Observable
                .Timer (TimeSpan.FromSeconds (_duration))
                .Subscribe (_ => Deactivate ())
                .AddTo (this);
        }
    }
}