using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CavemanTools.Extensions;


namespace System
{
	public static class ObjectExtend
	{
	    private static ConcurrentDictionary<Type, TypeInfo> _typeDicts;
		/// <summary>
		/// Creates dictionary from object properties.
		/// </summary>
		/// <param name="value">Object</param>
		/// <returns></returns>
		public static IDictionary<string,object> ToDictionary(this object value)
		{
			if (_typeDicts==null)
			{
			    _typeDicts= new ConcurrentDictionary<Type, TypeInfo>();
			}

		    TypeInfo info;
		    var tp = value.GetType();
            if (tp==typeof(ExpandoObject))
            {
                return (IDictionary<string, object>) value;
            }
            
            if(!_typeDicts.TryGetValue(tp,out info))
            {
                var allp = tp.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.GetProperty);
                
                //lambda
                var dict = Expression.Parameter(typeof (IDictionary<string, object>),"dict");
                var inst = Expression.Parameter(typeof(object),"obj");
              
                var lblock=new List<Expression>(allp.Length);
                
                
                for(int i=0;i<allp.Length;i++)
                {
                    var prop = allp[i];
                    var indexer = Expression.Property(dict, "Item",Expression.Constant(prop.Name));
                    lblock.Add(
                        Expression.Assign(indexer,
                            Expression.ConvertChecked(
                                Expression.Property(
                                   Expression.ConvertChecked(inst, tp), prop.Name), typeof(object))
                            ));
                }
                var body = Expression.Block(lblock);
                var lambda = Expression.Lambda(Expression.GetActionType(typeof(IDictionary<string,object>),typeof(object)),body, dict, inst);
                
                info=new TypeInfo(allp.Length,lambda.Compile());
                _typeDicts.TryAdd(tp, info);
            }

            return info.Update(value.ConvertTo(tp));		    
		}
		
        class TypeInfo
        {
            private readonly int _size;
            private readonly Delegate _del;
         
            public TypeInfo(int size,Delegate del)
            {
                _size = size;
                _del = del;
            }

            public Dictionary<string,object> Update(object o)
            {
                var d = new Dictionary<string, object>(_size);
                (_del as Action<IDictionary<string,object>,object>)(d, o);
                return d;
            }
        }

	
        ///// <summary>
        /////  Shallow copies source object into destination, only public properties are copied. Use CopyOptionsAttribute to mark the properties you want ignored.
        ///// Use Automapper for heavy duty mapping
        ///// </summary>
        ///// <seealso cref="CopyOptionsAttribute"/>
        ///// <typeparam name="T">Destination type must have parameterless constructor</typeparam>
        ///// <param name="source">Object to copy</param>
        //public static T CopyTo<T>(this object source) where T :class, new() 
        //{
        //    var destination = new T();
        //    source.CopyTo(destination);
        //    return destination;
        //}


        ///// <summary>
        ///// Shallow copies source object into destination, only public properties are copied. For ocasional use.
        ///// Use Automapper for heavy duty mapping
        ///// </summary>
        ///// <exception cref="ArgumentNullException">If source or destination are null</exception>
        ///// <typeparam name="T">Destination Type</typeparam>
        ///// <param name="source">Object to copy from</param>
        ///// <param name="destination">Object to copy to. Unmatching or read-only properties are ignored</param>
        //public static void CopyTo<T>(this object source, T destination) where T : class
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (destination == null) throw new ArgumentNullException("destination");
        //    var srcType = source.GetType();
        //    var attr = destination.GetType().GetSingleAttribute<CopyOptionsAttribute>();
        //    if (attr != null)
        //    {
        //        if (attr.IgnoreProperty) ;
        //    }

        //    foreach (var destProperty in destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.FlattenHierarchy))
        //    {
        //        if (!destProperty.CanWrite) continue;

        //        var pSource = srcType.GetProperty(destProperty.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
        //        if (pSource == null) continue;
        //        var o = pSource.GetValue(source, null);
        //        if (!pSource.PropertyType.Equals(destProperty.PropertyType))
        //        {
        //            o = ConvertTo(o, destProperty.PropertyType);
        //        }
        //        destProperty.SetValue(destination, o, null);
        //    }
        //}

		
		/// <summary>
		/// Converts object to type.
		/// Supports conversion to Enum, DateTime,TimeSpan and CultureInfo
		/// </summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <param name="data">Object to be converted</param>
		/// <param name="tp">Type to convert to</param>
		/// <returns></returns>
		public static object ConvertTo(this object data, Type tp)
		{
		   if (data==null)
		   {
		       if (tp.IsValueType && !tp.IsNullable())
		       {
		           throw new InvalidCastException("Cant convert null to a value type");
		       }
		       return null;
		   }

           var otp = data.GetType();
           if (otp.Equals(tp)) return data;
           if (tp.IsEnum)
           {
               if (data is string)
               {
                   return Enum.Parse(tp, data.ToString());
               }
               var o = Enum.ToObject(tp, data);
               return o;
           }

           if (tp.IsValueType)
           {
               if (tp == typeof(TimeSpan))
               {
                   return TimeSpan.Parse(data.ToString());
               }

               if (tp == typeof(DateTime))
               {
                   if (data is DateTimeOffset)
                   {
                       return data.Cast<DateTimeOffset>().DateTime;
                   }
                   return DateTime.Parse(data.ToString());
               }

               if (tp == typeof(DateTimeOffset))
               {
                   if (data is DateTime)
                   {
                       var dt = (DateTime)data;
                       return new DateTimeOffset(dt);
                   }
                   return DateTimeOffset.Parse(data.ToString());
               }

               if (tp.IsNullable())
               {
                   var under = Nullable.GetUnderlyingType(tp);
                   return data.ConvertTo(under);
               }
           }
           else if (tp == typeof(CultureInfo)) return new CultureInfo(data.ToString());

           return System.Convert.ChangeType(data, tp);
		}

		/// <summary>
		///	Tries to convert the object to type.
		/// </summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="FormatException"></exception>
		/// <typeparam name="T">Type to convert to</typeparam>
		/// <param name="data">Object</param>
		/// <returns></returns>
		public static T ConvertTo<T>(this object data)
		{
			var tp = typeof(T);			
			var temp = (T)ConvertTo(data, tp);
			return temp;			
		}

		

		/// <summary>
		///	Tries to convert the object to type.
		/// If it fails it returns the specified default value.
		/// </summary>
		/// <typeparam name="T">Type to convert to</typeparam>
		/// <param name="data">Object</param>
		/// <param name="defaultValue">IF not set , the default for T is used</param>
		/// <returns></returns>
		public static T SilentConvertTo<T>(this object data,T defaultValue=default(T))
		{
			var tp = typeof (T);  
			try
			{
				var temp = (T) ConvertTo(data, tp);
				return temp;
		    }
			catch (InvalidCastException)
			{
			    return defaultValue;
			}			
		}

        
        /// <summary>
        /// Shorthand for lazy people to cast an object to a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
       public static T As<T>(this object o) where T:class 
        {
            return o as T ;
		}

        /// <summary>
        /// Shorthand for lazy people to cast an object to a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T Cast<T>(this object o)
        {
            return (T)o;
        }

        /// <summary>
        /// Shortcut for 'object is type'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool Is<T>(this object o)
        {
            if (o is Type)
            {
                return (Type) o == typeof (T);
            }
            return o is T;
        }
	}

    
}

namespace CavemanTools.Testing
{
public static class ObjectExtensions
{
    /// <summary>
    /// Generates a string containing the properties and values of the object
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string ToDebugString(this object val, StringBuilder sb = null)
    {
        if (val == null) return string.Empty;
        if (sb == null)
        {
            sb = new StringBuilder();
        }
        var dict = val.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetIndexParameters().Length == 0)
            .ToDictionary(p => string.Format("({1}) {0}", p.Name, p.PropertyType.Name), p => p.GetValue(val, null));
        sb.AppendFormat("({0})\n", val.GetType());
        foreach (var kv in dict)
        {
            sb.AppendFormat("{0} =", kv.Key);
            if (kv.Value != null)
            {
                var tp = kv.Value.GetType();
                if (tp.Implements<IEnumerable>())
                {
                    var en = kv.Value as IEnumerable;
                    var i = 0;
                    sb.AppendFormat("\n\t");
                    foreach (var item in en)
                    {
                        sb.AppendFormat("[{0}]={1}\n", i, item.ToDebugString());
                        i++;
                    }

                }
                else
                {
                    if (!tp.IsPrimitive)
                    {
                        sb.Append("\n\t");
                        sb.Append(kv.Value.ToDebugString());
                    }
                    else
                    {
                        sb.Append(kv.Value);
                    }
                }
            }
            else
            {
                sb.Append("null");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

}

}							 