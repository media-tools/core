using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common
{
	public abstract class ValueObject<T> : IEquatable<T>
		where T : ValueObject<T>
	{
		protected abstract IEnumerable<object> Reflect ();

		public override bool Equals (object obj)
		{
			return ValueObject<T>.Equals (myself: (T)this, obj: obj);
		}

		public bool Equals (T other)
		{
			return ValueObject<T>.Equals (myself: (T)this, other: other);
		}

		public static bool Equals (T myself, object obj)
		{
			if (ReferenceEquals (null, obj))
				return false;
			if (obj.GetType () != myself.GetType ())
				return false;
			return Equals (myself: myself, other: obj as T);
		}

		public static bool Equals (T myself, T other)
		{
			if (ReferenceEquals (null, other))
				return false;
			if (ReferenceEquals (myself, other))
				return true;
			return myself.Reflect ().SequenceEqual (other.Reflect ());
		}

		public override int GetHashCode ()
		{
			return Reflect ().Aggregate (36, (hashCode, value) => value == null ?
				hashCode : hashCode ^ value.GetHashCode ());
		}

		public override string ToString ()
		{
			return "{ " + Reflect ().Aggregate ((l, r) => l + ", " + r) + " }";
		}

		public static bool Equality (ValueObject<T> a, ValueObject<T> b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals (a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match:
			return a.Reflect ().SequenceEqual (b.Reflect ());
		}

		public static bool Inequality (ValueObject<T> a, ValueObject<T> b)
		{
			return !(Equality (a, b));
		}

		public static bool operator == (ValueObject<T> a, ValueObject<T> b)
		{
			return Equality (a, b);
		}

		public static bool operator != (ValueObject<T> a, ValueObject<T> b)
		{
			return Inequality (a, b);
		}
	}
}

