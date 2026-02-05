using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using CheekyStork.Pooling;

namespace CheekyStork
{
    public class WorldspacePopup : MonoBehaviour, IPoolable<WorldspacePopup>
    {
        [SerializeField]
        private Image _sprite;

        [SerializeField]
        private TextMeshProUGUI _text;

        private Sequence _popupSequence;

        public event UnityAction<WorldspacePopup> ReturnToPool;

        public void Initialize(WorldspacePopupData popupData)
        {
            _text.text = popupData.Text;
            _sprite.sprite = popupData.Icon;
            _sprite.color = popupData.IconColor;

            DoAnimation(popupData.Duration);
        }

        public void ResetObject()
        {
            _text.text = string.Empty;
            _sprite.sprite = null;
            _sprite.color = Color.white;
            _text.alpha = 1f;

            _popupSequence?.Kill();
        }

        private void DoAnimation(float duration)
        {
            _popupSequence = DOTween.Sequence();

            _popupSequence.Append(transform.DOMoveY(transform.position.y + 0.1f, duration).SetEase(Ease.OutQuad));
            _popupSequence.Join(_text.DOFade(0f, duration - 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f));
            _popupSequence.Join(_sprite.DOFade(0f, duration - 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f));
            _popupSequence.OnComplete(() => ReturnToPool?.Invoke(this));
            _popupSequence.Play();
        }
    }
}