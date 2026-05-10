using UnityEngine;

namespace CosmicChamps.Utils
{
    public class ClipRectToTarget : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _target;

        public RectTransform Target
        {
            get => _target;
            set
            {
                _target = value;
                if (enabled)
                {
                    SetTargetClippingRect ();
                    currentPos = lastPos = _target.position;
                }
            }
        }

        private CanvasRenderer canvasRenderer;
        private Vector2 lastPos;
        private Vector2 currentPos;

        private void Awake ()
        {
            canvasRenderer = GetComponent<CanvasRenderer> ();
            if (_target != null)
            {
                currentPos = lastPos = _target.position;
            }
        }

        private void Update ()
        {
            if (_target == null) return;

            currentPos = _target.position;
            if (currentPos != lastPos)
            {
                SetTargetClippingRect ();
            }

            lastPos = currentPos;
        }

        private void OnEnable ()
        {
            if (_target != null)
            {
                SetTargetClippingRect ();
            }
        }

        private void OnDisable ()
        {
            canvasRenderer.DisableRectClipping ();
        }

        private void SetTargetClippingRect ()
        {
            var rect = _target.rect;
            Vector2 offset = _target.localPosition;
            var parent = _target.parent;
            while (parent.GetComponent<Canvas> () == null || !parent.GetComponent<Canvas> ().isRootCanvas)
            {
                offset += (Vector2)parent.localPosition;
                parent = parent.parent;
            }

            rect.x += offset.x;
            rect.y += offset.y;
            canvasRenderer.EnableRectClipping (rect);
        }
    }
}