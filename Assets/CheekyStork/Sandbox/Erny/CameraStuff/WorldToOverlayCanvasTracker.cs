using UnityEngine;

namespace UI.WorldspaceTracking
{
    [DisallowMultipleComponent]
    public sealed class WorldToOverlayCanvasTracker : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Transform _target;

        [SerializeField]
        private RectTransform _uiDot;

        [SerializeField]
        private bool _hideWhenBehindCamera = true;

        [SerializeField]
        private bool _clampToScreen = false;

        [SerializeField]
        private float _screenPaddingPixels = 12.0f;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            if (_camera == null || _target == null || _uiDot == null)
            {
                return;
            }

            Vector3 worldPosition = _target.position;
            Vector3 screenPoint = _camera.WorldToScreenPoint(worldPosition);

            bool isBehind = screenPoint.z <= 0.0f;

            if (_hideWhenBehindCamera && isBehind)
            {
                if (_uiDot.gameObject.activeSelf)
                {
                    _uiDot.gameObject.SetActive(false);
                }

                return;
            }

            if (!_uiDot.gameObject.activeSelf)
            {
                _uiDot.gameObject.SetActive(true);
            }

            if (_clampToScreen)
            {
                float minX = _screenPaddingPixels;
                float maxX = Screen.width - _screenPaddingPixels;
                float minY = _screenPaddingPixels;
                float maxY = Screen.height - _screenPaddingPixels;

                screenPoint.x = Mathf.Clamp(screenPoint.x, minX, maxX);
                screenPoint.y = Mathf.Clamp(screenPoint.y, minY, maxY);
            }

            // For Screen Space - Overlay canvas, RectTransform.position uses screen coordinates.
            _uiDot.position = new Vector3(screenPoint.x, screenPoint.y, 0.0f);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetCamera(Camera camera)
        {
            _camera = camera;
        }

        public void SetUiDot(RectTransform uiDot)
        {
            _uiDot = uiDot;
        }
    }
}
