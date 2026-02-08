using System;
using UnityEngine;

public sealed class ForceSixteenNineResolution : MonoBehaviour
{
    [Header("Preference")]
    [SerializeField]
    private int _preferredHeight = 1080;

    [SerializeField]
    private FullScreenMode _fullScreenMode = FullScreenMode.FullScreenWindow;

    [SerializeField]
    private int _refreshRateHz = 0; // 0 = keep current / default

    [Header("Behavior")]
    [SerializeField]
    private bool _reapplyIfChanged = true;

    [SerializeField]
    private float _checkIntervalSeconds = 0.5f;

    private const float TargetAspect = 16.0f / 9.0f;

    private int _appliedWidth;
    private int _appliedHeight;
    private FullScreenMode _appliedMode;

    private float _nextCheckTime;

    private void Awake()
    {
        ApplyBestSixteenNine();
    }

    private void Update()
    {
        if (!_reapplyIfChanged)
        {
            return;
        }

        if (Time.unscaledTime < _nextCheckTime)
        {
            return;
        }

        _nextCheckTime = Time.unscaledTime + Mathf.Max(0.1f, _checkIntervalSeconds);

        if (Screen.width != _appliedWidth ||
            Screen.height != _appliedHeight ||
            Screen.fullScreenMode != _appliedMode)
        {
            ApplyBestSixteenNine();
        }
    }

    private void ApplyBestSixteenNine()
    {
        Resolution best = FindBestSupportedSixteenNine(_preferredHeight);

        int width = best.width;
        int height = best.height;

        Screen.SetResolution(width, height, _fullScreenMode, ResolveRefreshRate(best));

        _appliedWidth = width;
        _appliedHeight = height;
        _appliedMode = _fullScreenMode;
    }

    private static Resolution FindBestSupportedSixteenNine(int preferredHeight)
    {
        Resolution[] resolutions = Screen.resolutions;

        if (resolutions == null || resolutions.Length == 0)
        {
            // Fallback if Unity can't enumerate (rare, but possible).
            return new Resolution
            {
                width = 1920,
                height = 1080,
                refreshRateRatio = new RefreshRate { numerator = 60, denominator = 1 }
            };
        }

        Resolution best = default;
        bool found = false;

        int preferredWidth = Mathf.RoundToInt(preferredHeight * TargetAspect);

        // Choose:
        // 1) Closest height to preferred
        // 2) Then closest width to preferred
        // 3) Then highest refresh rate
        int bestScore = int.MaxValue;
        int bestRefresh = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];

            if (!IsSixteenNine(r.width, r.height))
            {
                continue;
            }

            int dh = Mathf.Abs(r.height - preferredHeight);
            int dw = Mathf.Abs(r.width - preferredWidth);

            int score = (dh * 100000) + dw; // heavily weight matching height

            int refresh = GetRefreshRateHz(r);

            if (!found ||
                score < bestScore ||
                (score == bestScore && refresh > bestRefresh))
            {
                found = true;
                best = r;
                bestScore = score;
                bestRefresh = refresh;
            }
        }

        if (found)
        {
            return best;
        }

        // If no exact 16:9 mode exists, pick closest aspect mode to 16:9.
        float bestAspectDiff = float.MaxValue;
        int bestPixels = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];
            float aspect = (float)r.width / (float)r.height;
            float diff = Mathf.Abs(aspect - TargetAspect);

            int pixels = r.width * r.height;

            if (diff < bestAspectDiff || (Mathf.Approximately(diff, bestAspectDiff) && pixels > bestPixels))
            {
                best = r;
                bestAspectDiff = diff;
                bestPixels = pixels;
            }
        }

        return best;
    }

    private static bool IsSixteenNine(int width, int height)
    {
        // Exact integer ratio check: width/height == 16/9
        // i.e., width * 9 == height * 16
        return (width * 9) == (height * 16);
    }

    private int ResolveRefreshRate(Resolution chosen)
    {
#if UNITY_2022_2_OR_NEWER
        if (_refreshRateHz > 0)
        {
            return _refreshRateHz;
        }

        int hz = GetRefreshRateHz(chosen);
        return hz > 0 ? hz : 60;
#else
        return _refreshRateHz > 0 ? _refreshRateHz : chosen.refreshRate;
#endif
    }

    private static int GetRefreshRateHz(Resolution r)
    {
#if UNITY_2022_2_OR_NEWER
        if (r.refreshRateRatio.denominator == 0)
        {
            return 0;
        }

        double hz = (double)r.refreshRateRatio.numerator / (double)r.refreshRateRatio.denominator;
        return (int)Math.Round(hz);
#else
        return r.refreshRate;
#endif
    }
}
