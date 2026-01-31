using UnityEngine;

namespace CheekyStork
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera _cachedCamera;

        private void Awake()
        {
            CacheCamera();
        }

        private void OnEnable()
        {
            CacheCamera();
            Apply();
        }

        private void LateUpdate()
        {
            Apply();
        }

        private void CacheCamera()
        {
            if (_cachedCamera == null)
            {
                _cachedCamera = Camera.main;
            }
        }

        private void Apply()
        {
            Vector3 camForward = _cachedCamera.transform.forward;
            Vector3 camUp = _cachedCamera.transform.up;

            Quaternion desiredRotation = Quaternion.LookRotation(camForward, camUp);

            transform.rotation = desiredRotation;
        }
    }
}