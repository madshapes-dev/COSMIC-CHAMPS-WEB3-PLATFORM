using System;
using System.Linq;
using CosmicChamps.Battle.Data;
using UnityEngine;

namespace CosmicChamps.Battle
{
    public class CameraService
    {
        private readonly Camera _camera;
        private readonly CameraPlaceholder[] _placeholders;

        public Camera Camera => _camera;

        public CameraService (Camera camera, CameraPlaceholder[] placeholders)
        {
            _camera = camera;
            _placeholders = placeholders;
        }

        public void SetActiveCamera (PlayerTeam playerTeam)
        {
            var placeholder = _placeholders.FirstOrDefault (x => x.PlayerTeam == playerTeam);
            if (placeholder == null)
                throw new ArgumentOutOfRangeException ($"Placeholder for '{playerTeam}' not found");

            var cameraTransform = _camera.transform;
            cameraTransform.position = placeholder.Placeholder.position;
            cameraTransform.rotation = placeholder.Placeholder.rotation;
        }

        public CameraRelativeSide GetSide (Transform transform)
        {
            var cameraTransform = _camera.transform;
            var cameraRelativePosition = cameraTransform.InverseTransformPoint (transform.position);

            return cameraRelativePosition.x > 0
                ? CameraRelativeSide.Right
                : CameraRelativeSide.Left;
        }
    }
}