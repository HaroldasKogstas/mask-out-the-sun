using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace CheekyStork
{
    public class WorldspacePopup : MonoBehaviour
    {
        [SerializeField]
        private Image _sprite;

        [SerializeField]
        private TextMeshProUGUI _text;

        public void Initialize(WorldspacePopupData popupData)
        {
            _text.text = popupData.Text;
            _sprite.sprite = popupData.Icon;
            _sprite.color = popupData.IconColor;

            DoAnimation(popupData.Duration);
        }

        private void DoAnimation(float duration)
        {
            Sequence popupSequence = DOTween.Sequence();

            popupSequence.Append(transform.DOMoveY(transform.position.y + 0.1f, duration).SetEase(Ease.OutQuad));
            popupSequence.Join(_text.DOFade(0f, duration - 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f));
            popupSequence.Join(_sprite.DOFade(0f, duration - 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f));
            popupSequence.OnComplete(() => Destroy(gameObject));
            popupSequence.Play();
        }
    }
}