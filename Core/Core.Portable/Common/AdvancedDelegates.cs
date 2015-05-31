using System;
using System.Threading.Tasks;

namespace Core.Common
{
	// async Func
	public delegate Task<TR> AsyncFunc<TR> ();
	public delegate Task<TR> AsyncFunc<T1, TR> (T1 a);
	public delegate Task<TR> AsyncFunc<T1, T2, TR> (T1 a, T2 b);
	public delegate Task<TR> AsyncFunc<T1, T2, T3, TR> (T1 a, T2 b, T3 c);
	public delegate Task<TR> AsyncFunc<T1, T2, T3, T4, TR> (T1 a, T2 b, T3 c, T4 d);

	// async Action
	public delegate Task AsyncAction ();
	public delegate Task AsyncAction<T1> (T1 a);
	public delegate Task AsyncAction<T1, T2> (T1 a, T2 b);
	public delegate Task AsyncAction<T1, T2, T3> (T1 a, T2 b, T3 c);
	public delegate Task AsyncAction<T1, T2, T3, T4> (T1 a, T2 b, T3 c, T4 d);

	// params Func
	public delegate TR ParamsFunc<TP, TR> (params TP[] array);
	public delegate TR ParamsFunc<T1, TP, TR> (T1 a, params TP[] array);
	public delegate TR ParamsFunc<T1, T2, TP, TR> (T1 a, T2 b, params TP[] array);
	public delegate TR ParamsFunc<T1, T2, T3, TP, TR> (T1 a, T2 b, T3 c, params TP[] array);
	public delegate TR ParamsFunc<T1, T2, T3, T4, TP, TR> (T1 a, T2 b, T3 c, T4 d, params TP[] array);

	// params Action
	public delegate void ParamsAction<TP> (params TP[] array);
	public delegate void ParamsAction<T1, TP> (T1 a, params TP[] array);
	public delegate void ParamsAction<T1, T2, TP> (T1 a, T2 b, params TP[] array);
	public delegate void ParamsAction<T1, T2, T3, TP> (T1 a, T2 b, T3 c, params TP[] array);
	public delegate void ParamsAction<T1, T2, T3, T4, TP> (T1 a, T2 b, T3 c, T4 d, params TP[] array);

	public static class Actions
	{
		public static void Empty ()
		{
		}

		public static void Empty<T> (T value)
		{
		}

		public static void Empty<T1, T2> (T1 value1, T2 value2)
		{
		}

		public static void Empty<T1, T2, T3> (T1 value1, T2 value2, T3 value3)
		{
		}

		public static void Empty<T1, T2, T3, T4> (T1 value1, T2 value2, T3 value3, T4 value4)
		{
		}

		public static void Empty<T1, T2, T3, T4, T5> (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
		}

		public static Task EmptyAsync ()
		{
			return TaskHelper.Completed;
		}

		public static Task EmptyAsync<T> (T value)
		{
			return TaskHelper.Completed;
		}

		public static Task EmptyAsync<T1, T2> (T1 value1, T2 value2)
		{
			return TaskHelper.Completed;
		}

		public static Task EmptyAsync<T1, T2, T3> (T1 value1, T2 value2, T3 value3)
		{
			return TaskHelper.Completed;
		}

		public static Task EmptyAsync<T1, T2, T3, T4> (T1 value1, T2 value2, T3 value3, T4 value4)
		{
			return TaskHelper.Completed;
		}

		public static Task EmptyAsync<T1, T2, T3, T4, T5> (T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
			return TaskHelper.Completed;
		}
	}

	public static class Functions
	{
		public static T Identity<T> (T value)
		{
			return value;
		}

		public static T0 Default<T0> ()
		{
			return default(T0);
		}

		public static T0 Default<T1, T0> (T1 value1)
		{
			return default(T0);
		}
	}
}

