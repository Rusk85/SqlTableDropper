using System;

namespace CavemanTools.Model
{
	
    ///// <summary>
    ///// Used for paginating stuff
    ///// </summary>
    //[Obsolete("Use Pagination")]
    //public class PagerHelper
    //{
    //    /// <summary>
    //    /// How many items should be skipped to reach the result page
    //    /// </summary>
    //    /// <param name="page">Page number</param>
    //    /// <param name="itemsOnPage">Number of items on a page</param>
    //    /// <returns></returns>
    //    [Obsolete("Use Pagination")]
    //    public static int GetSkips(int page,int itemsOnPage)
    //    {
    //        if (page < 1) page = 1;
    //        return (page - 1)*itemsOnPage;
    //    }

    //    /// <summary>
    //    /// Ensures we have a valid page number
    //    /// </summary>
    //    /// <param name="page">Page number</param>
    //    /// <returns></returns>
    //    public static int MakeValidPage(int? page)
    //    {
    //        if (page == null || page < 1)
    //        {
    //            page = 1;
    //        }
    //        return page.Value;
    //    }
    //}
}