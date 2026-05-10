using UnityEngine;

namespace CosmicChamps.Battle.Units.Effects
{
    public class ProjectileAttackEffect : MonoBehaviour
    {
        [SerializeField]
        private ProjectileMover _projectile;

        [SerializeField]
        private Transform _muzzle;

        public void Activate (Vector3 targetPosition)
        {
            var muzzlePosition = _muzzle.position;
            var projectileMover = Instantiate (_projectile, muzzlePosition, Quaternion.identity);
            projectileMover.transform.forward = targetPosition - muzzlePosition;
            projectileMover.SetTarget (targetPosition);
        }
    }
}