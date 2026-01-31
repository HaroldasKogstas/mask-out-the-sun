using UnityEngine;
using UnityEngine.Events;

namespace CheekyStork.ScriptableChannels
{


    public abstract class ChannelSO : ChannelAbstractSO
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            SenderDebugging();
            ListenerDebugging(OnEventRaised);

            if (OnEventRaised == null)
            {
                if (NotifyEventRaiseFailed)
                {
                    OnEventRaiseFailed();
                }

                return;
            }

            OnEventRaised.Invoke();
        }
    }

    public abstract class ChannelSO<T> : ChannelAbstractSO
    {
        public event UnityAction<T> OnEventRaised;

        public void RaiseEvent(T value)
        {
            SenderDebugging();
            ListenerDebugging(OnEventRaised);

            if (OnEventRaised == null)
            {
                if (NotifyEventRaiseFailed)
                {
                    OnEventRaiseFailed();
                }

                return;
            }

            OnEventRaised.Invoke(value);
        }
    }

    public abstract class ChannelSO<T1, T2> : ChannelAbstractSO
    {
        public event UnityAction<T1, T2> OnEventRaised;

        public virtual void RaiseEvent(T1 value1, T2 value2)
        {
            SenderDebugging();
            ListenerDebugging(OnEventRaised);

            if (OnEventRaised == null)
            {
                if (NotifyEventRaiseFailed)
                {
                    OnEventRaiseFailed();
                }

                return;
            }

            OnEventRaised.Invoke(value1, value2);
        }
    }

    public abstract class ChannelSO<T1, T2, T3> : ChannelAbstractSO
    {
        public event UnityAction<T1, T2, T3> OnEventRaised;

        public virtual void RaiseEvent(T1 value1, T2 value2, T3 value3)
        {
            SenderDebugging();
            ListenerDebugging(OnEventRaised);

            if (OnEventRaised == null)
            {
                if (NotifyEventRaiseFailed)
                {
                    OnEventRaiseFailed();
                }

                return;
            }

            OnEventRaised.Invoke(value1, value2, value3);
        }
    }
}