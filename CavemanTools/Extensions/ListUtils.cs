using System.Linq;

namespace System.Collections.Generic
{
	public static class ListUtils
	{
		/// <summary>
		/// Checks if 2 enumerables have the same elements in the same order
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
        public static bool HasTheSameElementsAs<T>(this IEnumerable<T> first,IEnumerable<T> second)
		{
		    first.MustNotBeNull();
            second.MustNotBeNull();

            var cnt1 = first.Count();
		    if (cnt1 != second.Count()) return false;
		    T item1 = default(T);
		    T item2 = default(T);
            for(int i=0;i<cnt1;i++)
            {
                item1 = first.Skip(i).Take(1).First();
                item2 = second.Skip(i).Take(1).First();
                if (!item1.Equals(item2)) return false;
            }
		    return true;
		}
        /// <summary>
		/// Compares two sequences and returns the added or removed items.
		/// </summary>
		/// <typeparam name="T">Implements IEquatable</typeparam>
		/// <param name="fresh">Recent sequence</param>
		/// <param name="old">Older sequence used as base of comparison</param>
		/// <returns></returns>
		public static IModifiedSet<T> Compare<T>(this IEnumerable<T> fresh, IEnumerable<T> old)	where T:IEquatable<T>
		{
			if (fresh == null) throw new ArgumentNullException("fresh");
			if (old == null) throw new ArgumentNullException("old");
			var mods = new ModifiedSet<T>();
			
			foreach (var item in old)
			{
				if (!fresh.Contains(item)) mods.RemovedItem(item);
			}
			
			foreach (var item in fresh)
			{
				if (!old.Contains(item)) mods.AddedItem(item);
			}
			return mods;
		}

	    /// <summary>
	    /// Compares two sequences and returns the added or removed items.
	    /// Use this when T doesn't implement IEquatable
	    /// </summary>
	    /// <typeparam name="T">Type</typeparam>
	    /// <param name="fresh">Recent sequence</param>
	    /// <param name="old">Older sequence used as base of comparison</param>
	    /// <param name="match">function to check equality</param>
	    /// <returns></returns>
	    public static IModifiedSet<T> Compare<T>(this IEnumerable<T> fresh, IEnumerable<T> old,Func<T,T,bool> match)
        {
            if (fresh == null) throw new ArgumentNullException("fresh");
            if (old == null) throw new ArgumentNullException("old");
            if (match == null) throw new ArgumentNullException("match");
            var mods = new ModifiedSet<T>();

            foreach (var item in old)
            {
                if (!fresh.Any(d=>match(d,item))) mods.RemovedItem(item);
            }

            foreach (var item in fresh)
            {
                if (!old.Any(d=>match(d,item))) mods.AddedItem(item);
            }
            return mods;
        }

		/// <summary>
		/// Compares two sequences and returns the result.
		/// This special case method is best used when you have identifiable objects that can change their content/value but not their id.
		/// </summary>
		/// <typeparam name="T">Implements IEquatable</typeparam>
		/// <param name="fresh">Recent sequence</param>
		/// <param name="old">Older sequence used as base of comparison</param>
		/// <param name="detectChange">Delegate to determine if the items are identical.
		/// First parameter is new item, second is the item used as base for comparison</param>
		/// <returns></returns>
		public static IModifiedSet<T> WhatChanged<T>(this IEnumerable<T> fresh, IEnumerable<T> old,Func<T,T,bool> detectChange) where T : IEquatable<T>
		{
			if (fresh == null) throw new ArgumentNullException("fresh");
			if (old == null) throw new ArgumentNullException("old");
			if (detectChange == null) throw new ArgumentNullException("detectChange");
			var mods = new ModifiedSet<T>();

			foreach (var item in old)
			{
				if (!fresh.Any(d=> d.Equals(item))) mods.RemovedItem(item);
			}

			foreach (var item in fresh)
			{
				if (!old.Any(d=>d.Equals(item))) mods.AddedItem(item);
				else
				{
					var oldItem = old.First(d => d.Equals(item));	
					if (detectChange(item,oldItem))
					{						
						mods.ModifiedItem(oldItem,item);
					}
				}
			}
			return mods;
		}

        /// <summary>
        /// Updates the old collection with new items, while removing the inexistent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="old"></param>
        /// <param name="fresh"></param>
        /// <returns></returns>
        public static void Update<T>(this IList<T> old,IEnumerable<T> fresh) where T:IEquatable<T>
        {
            if (old == null) throw new ArgumentNullException("old");
            if (fresh == null) throw new ArgumentNullException("fresh");
            var diff = fresh.Compare(old);
            foreach (var item in diff.Removed)
            {
                old.Remove(item);
            }
            foreach (var item in diff.Added)
            {
                old.Add(item);
            }          
        }

        /// <summary>
        /// Updates the old collection with new items, while removing the inexistent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="old"></param>
        /// <param name="fresh"></param>
        /// <returns></returns>
        public static void Update<T>(this IList<T> old, IEnumerable<T> fresh,Func<T,T,bool> isEqual)
        {
            if (old == null) throw new ArgumentNullException("old");
            if (fresh == null) throw new ArgumentNullException("fresh");
            var diff = fresh.Compare(old,isEqual);
            
            foreach (var item in diff.Removed)
            {
                var i = old.Where(d => isEqual(d, item)).Select((d,idx)=>idx).First();
                old.RemoveAt(i);
            }
            foreach (var item in diff.Added)
            {
                old.Add(item);
            }
        }

		/// <summary>
		/// Checks if a collection is null or empty duh!
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="items">collection</param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
		{
			return items == null || !items.Any();
		}

        /// <summary>
        /// Gets typed value from dictionary or a default value if key is missing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="defValue">Value to return if dictionary doesn't contain the key</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string,object> dic,string key,T defValue=default(T))
        {
            if (dic.ContainsKey(key)) return dic[key].Cast<T>();
            return defValue;
        }

        public static bool AddIfNotPresent<T>(this IList<T> list, T item)
        {
            list.MustNotBeNull();
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns number of items removed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int RemoveAll<T>(this IList<T> items, Func<T, bool> predicate)
        {
            items.MustNotBeEmpty();
            predicate.MustNotBeNull();
            var removed = 0;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (predicate(items[i]))
                {
                    items.RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }
	}
}