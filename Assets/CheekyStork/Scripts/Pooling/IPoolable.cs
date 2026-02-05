using UnityEngine.Events;

namespace CheekyStork.Pooling
{
    public interface IPoolable<T>
    {
        public event UnityAction<T> ReturnToPool;

        public void ResetObject();
    }
}