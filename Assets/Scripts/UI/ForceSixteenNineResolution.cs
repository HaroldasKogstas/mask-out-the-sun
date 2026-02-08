using System;
using UnityEngine;

public sealed class ForceHighestSixteenNineResolution : MonoBehaviour
{
    [SerializeField]
    private FullScreenMode _fullScreenMode = FullScreenMode.FullScreenWindow;

    [SerializeField]
    private int _refreshRateHz = 0; // 0 = best available / default

    [SerializeField]
    private bool _reapplyIfChanged = false;

    [SerializeField]
    private float _checkIntervalSeconds = 0.5f;

    private int _appliedWidth;
    private int _appliedHeight;
    private FullScreenMode _appliedMode;
    private float _nextCheckTime;

    private void Awake()
    {
        ApplyHighestSixteenNine();
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
            ApplyHighestSixteenNine();
        }
    }

    private void ApplyHighestSixteenNine()
    {
        Resolution best = FindHighestSupportedSixteenNine();

        Screen.SetResolution(best.width, best.height, _fullScreenMode, ResolveRefreshRate(best));

        _appliedWidth = best.width;
        _appliedHeight = best.height;
        _appliedMode = _fullScreenMode;
    }

    private static Resolution FindHighestSupportedSixteenNine()
    {
        Resolution[] resolutions = Screen.resolutions;

        if (resolutions == null || resolutions.Length == 0)
        {
            return new Resolution
            {
                width = 1920,
                height = 1080,
#if UNITY_2022_2_OR_NEWER
                refreshRateRatio = new RefreshRate { numerator = 60, denominator = 1 }
#endif
            };
        }

        Resolution best = default;
        bool found = false;

        int bestPixels = -1;
        int bestRefresh = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];

            if (!IsSixteenNine(r.width, r.height))
            {
                continue;
            }

            int pixels = r.width * r.height;
            int refresh = GetRefreshRateHz(r);

            if (!found ||
                pixels > bestPixels ||
                (pixels == bestPixels && refresh > bestRefresh))
            {
                found = true;
                best = r;
                bestPixels = pixels;
                bestRefresh = refresh;
            }
        }

        if (found)
        {
            return best;
        }

        // If literally no 16:9 exists, fall back to the highest pixel count overall.
        Resolution fallback = resolutions[0];
        int fallbackPixels = fallback.width * fallback.height;
        int fallbackRefresh = GetRefreshRateHz(fallback);

        for (int i = 1; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];
            int pixels = r.width * r.height;
            int refresh = GetRefreshRateHz(r);

            if (pixels > fallbackPixels || (pixels == fallbackPixels && refresh > fallbackRefresh))
            {
                fallback = r;
                fallbackPixels = pixels;
                fallbackRefresh = refresh;
            }
        }

        return fallback;
    }

    private static bool IsSixteenNine(int width, int height)
    {
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
