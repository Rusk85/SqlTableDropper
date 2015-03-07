using System;

namespace CavemanTools.Extensions
{
    public static class GuidExtensions
    {
         public static string ToBase64(this Guid id)
         {
             return Convert.ToBase64String(id.ToByteArray());
         }
    }
}