using ThingsLibrary.Interfaces;

namespace ThingsLibrary.DataType.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Add or replace the item in the list
        /// </summary>
        /// <typeparam name="T">Item Type</typeparam>
        /// <param name="items">List of Items</param>
        /// <param name="newItem">New Item</param>
        public static void AddOrReplace<T>(this List<T> items, T newItem) where T : class, IKey
        {
            items.AddOrReplace(x => x.Key == newItem.Key, newItem);
        }

        /// <summary>
        /// Add or replace the item in the list
        /// </summary>
        /// <typeparam name="T">Item Data Type</typeparam>
        /// <param name="items">List of items</param>
        /// <param name="oldItemSelector">How to find the old item</param>
        /// <param name="newItem">New Item</param>
        public static void AddOrReplace<T>(this List<T> items, Predicate<T> oldItemSelector, T newItem)
        {            
            var index = items.FindIndex(oldItemSelector);
            if (index < 0)
            {
                items.Add(newItem);
            }
            else
            {
                items[index] = newItem;
            }            
        }
    }
}
