using System;
using System.Collections.Generic;

namespace CavemanTools.Model
{
	
    /// <summary>
	/// Holds limited result set and total number of items from a query.
	/// Used for pagination.
	/// </summary>
	/// <typeparam name="T">Type of item</typeparam>
    public class PagedResult<T>:IPagedResult<T>
    {
		public int Count
		{
			get; set;
		}

        private long? _count;

        public long LongCount
        {
	       get
	       {
	           if (_count == null)
	           {
	               return Count;
	           }
	           return _count.Value;
	       }

            set { _count = value; }
        }

	    public PagedResult()
	    {
	        Items = new T[0];
	    }

	    public IEnumerable<T> Items
		{
			get; set;
		}
	}

    //[Obsolete("Use PagedResult<T>",true)]
    //public class ResultSet<T>:PagedResult<T>
    //{
        
    //}
    
}