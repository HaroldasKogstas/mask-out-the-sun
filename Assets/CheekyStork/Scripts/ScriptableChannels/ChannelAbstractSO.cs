using CheekyStork.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace CheekyStork.ScriptableChannels
{
    // This class handles passing of events in a detached way, where anyone can hook into these channels and receive callbacks.
    public abstract class ChannelAbstractSO : ScriptableObject
    {
        [TextArea]
        [SerializeField]
        private string _description;

        [field: SerializeField]
        protected bool NotifyEventRaiseFailed { get; private set; }

        [field: SerializeField]
        protected bool DebugSenders { get; private set; }

        [field: SerializeField]
        protected bool DebugListeners { get; private set; }

        protected virtual void OnEventRaiseFailed()
        {
            this.LogError("Delegate is null.", LogTag.Flow);
        }

        protected void ListenerDebugging(UnityAction actionToDebug)
        {
            if (DebugListeners)
            {
                if (actionToDebug == null)
                {
                    this.LogDebug("No listeners.", LogTag.Flow);
                    return;
                }

                System.Delegate[] delegateList = actionToDebug.GetInvocationList();

                PrintListenerDelegateList(delegateList);
            }
        }

        protected void ListenerDebugging<T>(UnityAction<T> actionToDebug)
        {
            if (DebugListeners)
            {
                if (actionToDebug == null)
                {
                    this.LogDebug("No listeners.", LogTag.Flow);
                    return;
                }

                System.Delegate[] delegateList = actionToDebug.GetInvocationList();

                PrintListenerDelegateList(delegateList);
            }
        }

        protected void ListenerDebugging<T1, T2>(UnityAction<T1, T2> actionToDebug)
        {
            if (DebugListeners)
            {
                if (actionToDebug == null)
                {
                    this.LogDebug("No listeners.", LogTag.Flow);
                    return;
                }

                System.Delegate[] delegateList = actionToDebug.GetInvocationList();

                PrintListenerDelegateList(delegateList);
            }
        }

        protected void ListenerDebugging<T1, T2, T3>(UnityAction<T1, T2, T3> actionToDebug)
        {
            if (DebugListeners)
            {
                if (actionToDebug == null)
                {
                    this.LogDebug("No listeners.", LogTag.Flow);
                    return;
                }

                System.Delegate[] delegateList = actionToDebug.GetInvocationList();

                PrintListenerDelegateList(delegateList);
            }
        }

        private void PrintListenerDelegateList(System.Delegate[] delegateList)
        {
            foreach (System.Delegate delegateItem in delegateList)
            {
                string delegateName = delegateItem.Method.Name;
                string delegateTypeName = delegateItem.Method.DeclaringType.Name;

                this.LogDebug($"Listener method name: {delegateName} in class: {delegateTypeName}.", LogTag.Flow);
            }
        }

        protected void SenderDebugging()
        {
            if (DebugSenders)
            {
                string callingMethodName = new System.Diagnostics.StackFrame(1).GetMethod().Name;
                string methodTypeName = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType.Name;

                this.LogDebug($"Sender method: {callingMethodName} in class: {methodTypeName}.", LogTag.Flow);
            }
        }
    }
}