using UnityEngine;

namespace CosmicChamps.Utils
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        private void Awake ()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        private void Update ()
        {
            if (_camera == null)
                return;

            transform.rotation = Quaternion.LookRotation (_camera.transform.forward * -1);
        }
    }
}