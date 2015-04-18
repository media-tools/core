using System;
using System.Reflection;

namespace Core.Common
{
	public static class ObjectExtensions
	{
		//public object this [string propertyName] {
		//	get { return this.GetType ().GetProperty (propertyName).GetValue (this, null); }
		//	set { this.GetType ().GetProperty (propertyName).SetValue (this, value, null); }
		//}

		public static V GetProperty<T, V> (this T obj, string name)
		{
			return (V)(obj.GetType ().GetProperty (name).GetValue (obj, null));
		}

		public static void SetProperty<T, V> (this T obj, string name, V value)
		{
			obj.GetType ().GetProperty (name).SetValue (obj, value, null);
		}
	}
}

