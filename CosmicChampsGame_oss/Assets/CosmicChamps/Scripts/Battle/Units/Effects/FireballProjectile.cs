using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Battle.Units.Effects
{
    public class FireballProjectile : MonoBehaviour
    {
        [SerializeField]
        private GameObject _collisionEffectPrefab;

        [SerializeField]
        private float _lifetime;

        [SerializeField]
        private ParticleSystem _particleSystem;

        private async void Start ()
        {
            await UniTask.Delay (TimeSpan.FromSeconds (_lifetime));

            var particles = new ParticleSystem.Particle[1];
            _particleSystem.GetParticles (particles, 1);

            var collisionEffect = Instantiate (_collisionEffectPrefab, transform);
            collisionEffect.transform.position = particles[0].position;
        }
    }
}