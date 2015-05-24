using System;

namespace Core.Common
{
	public delegate TR ParamsFunc<TP, TR> (params TP[] array);
	public delegate TR ParamsFunc<T1, TP, TR> (T1 a, params TP[] array);
	public delegate TR ParamsFunc<T1, T2, TP, TR> (T1 a, T2 b, params TP[] array);
	public delegate TR ParamsFunc<T1, T2, T3, TP, TR> (T1 a, T2 b, T3 c, params TP[] array);
	public delegate TR ParamsFunc<T1, T2, T3, T4, TP, TR> (T1 a, T2 b, T3 c, T4 d, params TP[] array);
	public delegate void ParamsAction<TP> (params TP[] array);
	public delegate void ParamsAction<T1, TP> (T1 a, params TP[] array);
	public delegate void ParamsAction<T1, T2, TP> (T1 a, T2 b, params TP[] array);
	public delegate void ParamsAction<T1, T2, T3, TP> (T1 a, T2 b, T3 c, params TP[] array);
	public delegate void ParamsAction<T1, T2, T3, T4, TP> (T1 a, T2 b, T3 c, T4 d, params TP[] array);
}

