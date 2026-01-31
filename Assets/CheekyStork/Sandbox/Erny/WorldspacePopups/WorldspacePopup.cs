using UnityEngine;
using TMPro;
using DG.Tweening;

namespace CheekyStork
{
    public class WorldspacePopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        public void Initialize(WorldspacePopupData popupData)
        {
            _text.text = popupData.Text;
            //_text.color = popupData.VisualProfile.TextColor;
            //_text.fontSize *= popupData.VisualProfile.FontSizeMultiplier;

            DoAnimation(popupData.Duration);
        }

        private void DoAnimation(float duration)
        {
            Sequence popupSequence = DOTween.Sequence();

            popupSequence.Append(transform.DOMoveY(transform.position.y + 2f, duration).SetEase(Ease.OutQuad));
            popupSequence.Join(_text.DOFade(0f, duration - 0.5f).SetEase(Ease.OutQuad).SetDelay(0.5f));
            popupSequence.OnComplete(() => Destroy(gameObject));
            popupSequence.Play();
        }
    }
}