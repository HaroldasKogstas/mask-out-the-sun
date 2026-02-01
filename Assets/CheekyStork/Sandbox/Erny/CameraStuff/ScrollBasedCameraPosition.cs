using CheekyStork.ScriptableVariables;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class ScrollBasedCameraPosition : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraTransform;

    [SerializeField]
    private FloatSO _scrollLevel;

    [SerializeField]
    private float _minY = 0f;

    [SerializeField]
    private float _maxY = 10f;

    private float _scrollSpeed = 0.25f;

    private Vector3 _targetPosition;

    private Coroutine _scrollCoroutine;

    private TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _tweener;

    private void Awake()
    {
        _scrollLevel.OnValueChanged += OnScrollLevelChanged;
    }

    private void OnDestroy()
    {
        _scrollLevel.OnValueChanged -= OnScrollLevelChanged;
    }

    private void OnScrollLevelChanged()
    {
        // when scroll level is between 0 and 0.8, move camera between minY and maxY using do tween library

        if (_scrollLevel.Value >= 0f && _scrollLevel.Value <= 0.9f)
        {
            float targetY = Mathf.Lerp(_minY, _maxY, Mathf.InverseLerp(0f, 0.8f, _scrollLevel.Value));
            Vector3 targetPosition = new Vector3(_cameraTransform.position.x, targetY, _cameraTransform.position.z);

            if (_tweener != null && _tweener.IsActive())
            {
                _tweener.Kill();
            }

            _tweener = _cameraTransform.DOMove(targetPosition, _scrollSpeed);
        }
    }

}