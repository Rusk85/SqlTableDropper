namespace System.Reflection
{
	public static class AttributeUtils
	{
		/// <summary>
		/// Returns all custom attributes of specified type
		/// </summary>
		/// <typeparam name="T">Attribute</typeparam>
		/// <param name="provider">Custom attributes provider</param>
		/// <returns></returns>
		public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			return GetCustomAttributes<T>(provider, true);
		}

		/// <summary>
		/// Returns all custom attributes of specified type
		/// </summary>
		/// <typeparam name="T">Attribute</typeparam>
		/// <param name="provider">Custom attributes provider</param>
		/// <param name="inherit">When true, look up the hierarchy chain for custom attribute </param>
		/// <returns></returns>
		public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit) where T : Attribute
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			T[] attributes = provider.GetCustomAttributes(typeof(T), inherit) as T[];
			if (attributes == null)
			{
				return new T[0];
			}
			return attributes;
		}

		/// <summary>
		/// Gets a single or the first custom attribute of specified type
		/// </summary>
		/// <typeparam name="T">Attribute</typeparam>
		/// <param name="memberInfo">Custom Attribute provider</param>
		/// <returns></returns>
		public static T GetSingleAttribute<T>(this ICustomAttributeProvider memberInfo) where T : Attribute
		{
            if (memberInfo == null) throw new ArgumentNullException("memberInfo");
			var list = memberInfo.GetCustomAttributes(typeof(T), false);
			if (list.Length > 0) return (T)list[0];
			return null;
		}	
	
		public static bool HasCustomAttribute<T>(this ICustomAttributeProvider mi) where T:Attribute
		{
			return mi.GetSingleAttribute<T>() != null;
		}
	}
}