using System;
using UniRx;
using UnityEngine;

namespace CosmicChamps.Battle.Units.Effects
{
    public class CybiAttackEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject _particles;

        [SerializeField]
        private float _duration = 1.5f;

        private IDisposable _deactivateDisposable;

        private void OnDestroy ()
        {
            _deactivateDisposable?.Dispose ();
        }

        private void Deactivate ()
        {
            _particles.SetActive (false);
        }

        public void Activate ()
        {
            _particles.SetActive (true);
            _deactivateDisposable?.Dispose ();
            _deactivateDisposable = Observable
                .Timer (TimeSpan.FromSeconds (_duration))
                .Subscribe (_ => Deactivate ())
                .AddTo (this);
        }
    }
}