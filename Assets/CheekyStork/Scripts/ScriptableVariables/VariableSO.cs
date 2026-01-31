using CheekyStork.Logging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CheekyStork.ScriptableVariables
{
    /// <summary>
    /// This class implements a reference to a variable through a scriptable object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableSO<T> : ResettableSO
    {
        [field: SerializeField]
        protected bool DoDebug { get; private set; }

        [field: Title("Values")]
        [SerializeField]
        private T _value;

        [FormerlySerializedAs("DefaultValue")]
        [ShowIf("IsResetting")]
        [SerializeField] 
        private T _defaultValue;

        private T _lastValue;

        public event UnityAction OnValueChanged;
        public event UnityAction<T> OnValueChangedWithNewValue; 

        private void OnValidate()
        {
            CheckIfValueChanged();
        }

        public T Value
        {
            get
            {
                if (DoDebug)
                {
                    this.LogDebug("Getter got called, check trace for more info.", LogTag.Core);
                }

                return _value;
            }
            set
            {
                if (DoDebug)
                {
                    this.LogDebug("Setter got called, check trace for more info.", LogTag.Core);
                }

                _value = value;

                CheckIfValueChanged();
            }
        }

        public override void ResetResettable()
        {
            Value = _defaultValue;
        }

        private void CheckIfValueChanged()
        {
            if (_lastValue!= null && _lastValue.Equals(_value))
            {
                return;
            }

            _lastValue = _value;

            OnValueChanged?.Invoke();
        }
    }
}