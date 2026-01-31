using System.Collections.Generic;

namespace CheekyStork.Extensions
{
    public static class ListExtensions
    {
        private static readonly System.Random _random = new();
        
        public static List<T> Randomize<T>(this List<T> list)
        {
            List<T> randomizedList = new(list);

            int count = randomizedList.Count;
            for (int i = count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (randomizedList[i], randomizedList[j]) = (randomizedList[j], randomizedList[i]);
            }

            return randomizedList;
        }

        public static List<T> MoveItemAtIndexToFront<T>(this List<T> list, int index)
        {
            T item = list[index];
            for (int i = index; i > 0; i--)
                list[i] = list[i - 1];
            list[0] = item;

            return list;
        }

        public static bool TryAddUnique<T>(this List<T> list, T objectToAdd)
        {
            if (list.Contains(objectToAdd))
            {
                return false;
            }

            list.Add(objectToAdd);

            return true;
        }
        
        public static bool TryRemoveItem<T>(this List<T> list, T objectToRemove)
        {
            if (!list.Contains(objectToRemove))
            {
                return false;
            }

            list.Remove(objectToRemove);

            return true;
        }

        public static void AddUnique<T>(this List<T> list, T objectToAdd, System.Predicate<T> match = null)
        {
            if(match == null)
            {
                match = (checkObject) => checkObject.Equals(objectToAdd);
            }

            List<T> foundItems = list.FindAll(match);

            foreach(T item in foundItems)
            {
                list.Remove(item);
            }

            list.Add(objectToAdd);
        }
    }
}