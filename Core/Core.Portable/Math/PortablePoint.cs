using System;

namespace Core.Math
{
	public struct PortablePoint : IFormattable
	{
		internal static PortablePoint Zero = new PortablePoint (0, 0);

		public double X { get; set; }

		public double Y { get; set; }

		public static double EPSILON = double.Epsilon;

		public PortablePoint (double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public override bool Equals (object o)
		{
			if (!(o is PortablePoint))
				return false;

			return Equals ((PortablePoint)o);
		}

		public bool Equals (PortablePoint value)
		{
			return System.Math.Abs (value.X - X) < EPSILON && System.Math.Abs (value.Y - Y) < EPSILON;
		}

		public override int GetHashCode ()
		{
			return X.GetHashCode () ^ Y.GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ("{0},{1}", X, Y);
		}

		public string ToString (IFormatProvider provider)
		{
			return (this as IFormattable).ToString (null, provider);
		}

		public static bool operator == (PortablePoint point1, PortablePoint point2)
		{
			return (System.Math.Abs (point1.X - point2.X) < EPSILON) && (System.Math.Abs (point1.Y - point2.Y) < EPSILON);
		}

		public static bool operator != (PortablePoint point1, PortablePoint point2)
		{
			return System.Math.Abs (point1.X - point2.X) > EPSILON || System.Math.Abs (point1.Y - point2.Y) > EPSILON;
		}

		/*internal static PortablePoint FromString (string str)
		{
			if (String.IsNullOrEmpty (str))
				throw new ArgumentNullException ("str");

			IntPtr ptr = NativeMethods.point_from_str (str);

			if (ptr == IntPtr.Zero)
				return new Point ();

			Point p = (Point)Marshal.PtrToStructure (ptr, typeof(Point));

			return p;
		}*/

		string System.IFormattable.ToString (string format, IFormatProvider provider)
		{
			if (String.IsNullOrEmpty (format))
				format = null;

			if (provider != null) {
				ICustomFormatter cp = (ICustomFormatter)provider.GetFormat (typeof(ICustomFormatter));
				if (cp != null) {
					return String.Format ("{0}{1}{2}", cp.Format (format, X, provider), 
						cp.Format (null, ',', provider), cp.Format (format, Y, provider));
				}
			}

			return String.Format ("{0},{1}", X.ToString (format, provider), Y.ToString (format, provider));
		}

		public T To<T> () where T : new()
		{
			T result = new T ();
			(result as dynamic).X = X;
			(result as dynamic).Y = Y;
			return result;
		}

		public static PortablePoint From<T> (T origin)
		{
			PortablePoint result = new PortablePoint ();
			result.X = (origin as dynamic).X;
			result.Y = (origin as dynamic).Y;
			return result;
		}
	}
}

