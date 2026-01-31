using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public sealed class UiSpriteSheetAnimator : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _frames = new List<Sprite>();

    [SerializeField]
    [Min(1)]
    private int _fps = 24;

    [SerializeField]
    private bool _loop = true;

    [SerializeField]
    private bool _playOnEnable = true;

    [SerializeField]
    private bool _setNativeSizeOnFirstFrame = false;

    private Image _image;
    private int _frameIndex;
    private float _secondsPerFrame;
    private float _accumulator;
    private bool _isPlaying;

    public IReadOnlyList<Sprite> Frames => _frames;
    public int Fps => _fps;
    public bool Loop => _loop;

    private void Awake()
    {
        _image = GetComponent<Image>();
        RecalculateTiming();
    }

    private void OnEnable()
    {
        if (_playOnEnable)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        Stop();
    }

    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }

        if (_frames == null || _frames.Count == 0)
        {
            return;
        }

        _accumulator += Time.unscaledDeltaTime;

        while (_accumulator >= _secondsPerFrame)
        {
            _accumulator -= _secondsPerFrame;
            AdvanceFrame();
        }
    }

    public void Play()
    {
        if (_frames == null || _frames.Count == 0)
        {
            _isPlaying = false;
            return;
        }

        _isPlaying = true;

        if (_frameIndex < 0 || _frameIndex >= _frames.Count)
        {
            _frameIndex = 0;
        }

        ApplyFrame(_frameIndex, applyNativeSizeIfEnabled: true);
    }

    public void Stop()
    {
        _isPlaying = false;
        _accumulator = 0.0f;
    }

    public void Restart()
    {
        _frameIndex = 0;
        _accumulator = 0.0f;
        Play();
    }

    public void SetFps(int fps)
    {
        _fps = Math.Max(1, fps);
        RecalculateTiming();
    }

    private void RecalculateTiming()
    {
        _secondsPerFrame = 1.0f / Mathf.Max(1, _fps);
    }

    private void AdvanceFrame()
    {
        int nextIndex = _frameIndex + 1;

        if (nextIndex >= _frames.Count)
        {
            if (_loop)
            {
                nextIndex = 0;
            }
            else
            {
                _isPlaying = false;
                return;
            }
        }

        _frameIndex = nextIndex;
        ApplyFrame(_frameIndex, applyNativeSizeIfEnabled: false);
    }

    private void ApplyFrame(int index, bool applyNativeSizeIfEnabled)
    {
        Sprite sprite = _frames[index];
        if (sprite == null)
        {
            return;
        }

        _image.sprite = sprite;

        if (applyNativeSizeIfEnabled && _setNativeSizeOnFirstFrame)
        {
            _image.SetNativeSize();
        }
    }
    
    public void SetFrames(List<Sprite> frames)
    {
        _frames = frames ?? new List<Sprite>();
        _frameIndex = 0;
        _accumulator = 0.0f;
    }
}