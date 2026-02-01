using UnityEngine;

public class DysonPart : MonoBehaviour
{
    [SerializeField]
    private float _deployRate = 1f;
    [SerializeField]
    private AnimationCurve _deployCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private float _targetScale = 1f;

    [SerializeField]
    private bool _scaleOnEnable = true;

    private Vector3 _cachedScale;
    private float _deployProgress = 0f;
    private bool _isDeploying = false;

    void OnEnable()
    {
        if(_scaleOnEnable)
            StartDeploy();
    }

    private void Update()
    {
        if (_isDeploying)
        {
            _deployProgress += Time.deltaTime * _deployRate;

            if (_deployProgress >= 1f)
            {
                _deployProgress = 1f;
                _isDeploying = false;
            }

            float curveValue = _deployCurve.Evaluate(_deployProgress);
            transform.localScale = _cachedScale * curveValue;
        }
    }

    void OnDisable()
    {
        ResetDeploy();
    }

    public void Initialize(float size)
    {
        _cachedScale = new Vector3(size, size, size);
        transform.localScale = _cachedScale;
    }

    public void StartDeploy()
    {
        _deployProgress = 0f;
        _isDeploying = true;
        transform.localScale = Vector3.zero;
    }

    public void ResetDeploy()
    {
        _deployProgress = 0f;
        _isDeploying = false;
        transform.localScale = Vector3.zero;
    }
}