using System.Collections.Generic;
using CheekyStork.Extensions;
using UnityEditor;
using UnityEngine;

namespace CheekyStork.ScriptableVariables
{
    /// <summary>
    /// Class which holds a list of objects of type T in a ScriptableObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListSO<T> : ScriptableObject
    {
        [SerializeField]
        private List<T> _list;

        public IReadOnlyCollection<T> List => _list;

        public bool TryAddUnique(T objectToAdd)
        {
            bool objectAdded = _list.TryAddUnique(objectToAdd);

#if UNITY_EDITOR
            if(objectAdded)
            {
                EditorUtility.SetDirty(this);
            }
#endif
            return objectAdded;
        }

        public void AddUnique(T objectToAdd, System.Predicate<T> match)
        {
            _list.AddUnique(objectToAdd, match);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void Remove(System.Predicate<T> match)
        {
            Remove(_list.Find(match));
        }

        public void Remove(T objectToRemove)
        {
            _list.Remove(objectToRemove);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public int FindIndex(System.Predicate<T> match)
        {
            int index = _list.FindIndex(match);

            return index;
        }

        public T Find(System.Predicate<T> match)
        {
            T foundObject = _list.Find(match);

            return foundObject;
        }
    }
}
