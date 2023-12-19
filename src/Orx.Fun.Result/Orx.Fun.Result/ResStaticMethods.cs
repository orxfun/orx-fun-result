namespace Orx.Fun.Result;

public readonly partial struct Res
{
    // eager
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <returns></returns>
    public static Res<(T1, T2)> AndAll
        <T1, T2>(
        Res<T1> res1, Res<T2> res2
        )
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap()));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3)> AndAll
        <T1, T2, T3>(
        Res<T1> res1, Res<T2> res2, Res<T3> res3
        )
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res3.IsOk) return new(res3.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap(), res3.Unwrap()
                ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4)> AndAll
        <T1, T2, T3, T4>(
        Res<T1> res1, Res<T2> res2, Res<T3> res3, Res<T4> res4
        )
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res3.IsOk) return new(res3.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res4.IsOk) return new(res4.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap(), res3.Unwrap(), res4.Unwrap()
                ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5)> AndAll
        <T1, T2, T3, T4, T5>(
        Res<T1> res1, Res<T2> res2, Res<T3> res3, Res<T4> res4,
        Res<T5> res5)
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res3.IsOk) return new(res3.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res4.IsOk) return new(res4.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res5.IsOk) return new(res5.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap(), res3.Unwrap(), res4.Unwrap(),
               res5.Unwrap()
               ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <param name="res6"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5, T6)> AndAll
        <T1, T2, T3, T4, T5, T6>(
        Res<T1> res1, Res<T2> res2, Res<T3> res3, Res<T4> res4,
        Res<T5> res5, Res<T6> res6)
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res3.IsOk) return new(res3.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res4.IsOk) return new(res4.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res5.IsOk) return new(res5.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res6.IsOk) return new(res6.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap(), res3.Unwrap(), res4.Unwrap(),
                 res5.Unwrap(), res6.Unwrap()
                 ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <param name="res6"></param>
    /// <param name="res7"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5, T6, T7)> AndAll
        <T1, T2, T3, T4, T5, T6, T7>(
        Res<T1> res1, Res<T2> res2, Res<T3> res3, Res<T4> res4,
        Res<T5> res5, Res<T6> res6, Res<T7> res7)
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res3.IsOk) return new(res3.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res4.IsOk) return new(res4.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res5.IsOk) return new(res5.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res6.IsOk) return new(res6.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res7.IsOk) return new(res7.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap(), res3.Unwrap(), res4.Unwrap(),
                res5.Unwrap(), res6.Unwrap(), res7.Unwrap()
                ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <param name="res6"></param>
    /// <param name="res7"></param>
    /// <param name="res8"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5, T6, T7, T8)> AndAll
        <T1, T2, T3, T4, T5, T6, T7, T8>(
        Res<T1> res1, Res<T2> res2, Res<T3> res3, Res<T4> res4,
        Res<T5> res5, Res<T6> res6, Res<T7> res7, Res<T8> res8)
    {
        if (!res1.IsOk) return new(res1.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res2.IsOk) return new(res2.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res3.IsOk) return new(res3.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res4.IsOk) return new(res4.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res5.IsOk) return new(res5.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res6.IsOk) return new(res6.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res7.IsOk) return new(res7.ErrorMessage().Unwrap(), "AndAll", null);
        if (!res8.IsOk) return new(res8.ErrorMessage().Unwrap(), "AndAll", null);

        return new((res1.Unwrap(), res2.Unwrap(), res3.Unwrap(), res4.Unwrap(),
              res5.Unwrap(), res6.Unwrap(), res7.Unwrap(), res8.Unwrap()));
    }


    // lazy
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <returns></returns>
    public static Res<(T1, T2)> AndAll
        <T1, T2>(
        Func<Res<T1>> res1, Func<Res<T2>> res2
        )
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap()
        ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3)> AndAll
        <T1, T2, T3>(
        Func<Res<T1>> res1, Func<Res<T2>> res2, Func<Res<T3>> res3
        )
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        var x3 = res3();
        if (!x3.IsOk) return new(x3.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap(), x3.Unwrap()
        ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4)> AndAll
        <T1, T2, T3, T4>(
        Func<Res<T1>> res1, Func<Res<T2>> res2, Func<Res<T3>> res3, Func<Res<T4>> res4
        )
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        var x3 = res3();
        if (!x3.IsOk) return new(x3.ErrorMessage().Unwrap(), "AndAll", null);

        var x4 = res4();
        if (!x4.IsOk) return new(x4.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap(), x3.Unwrap(), x4.Unwrap()
        ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5)> AndAll
        <T1, T2, T3, T4, T5>(
        Func<Res<T1>> res1, Func<Res<T2>> res2, Func<Res<T3>> res3, Func<Res<T4>> res4,
        Func<Res<T5>> res5
        )
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        var x3 = res3();
        if (!x3.IsOk) return new(x3.ErrorMessage().Unwrap(), "AndAll", null);

        var x4 = res4();
        if (!x4.IsOk) return new(x4.ErrorMessage().Unwrap(), "AndAll", null);

        var x5 = res5();
        if (!x5.IsOk) return new(x5.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap(), x3.Unwrap(), x4.Unwrap(),
            x5.Unwrap()
        ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <param name="res6"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5, T6)> AndAll
        <T1, T2, T3, T4, T5, T6>(
        Func<Res<T1>> res1, Func<Res<T2>> res2, Func<Res<T3>> res3, Func<Res<T4>> res4,
        Func<Res<T5>> res5, Func<Res<T6>> res6
        )
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        var x3 = res3();
        if (!x3.IsOk) return new(x3.ErrorMessage().Unwrap(), "AndAll", null);

        var x4 = res4();
        if (!x4.IsOk) return new(x4.ErrorMessage().Unwrap(), "AndAll", null);

        var x5 = res5();
        if (!x5.IsOk) return new(x5.ErrorMessage().Unwrap(), "AndAll", null);

        var x6 = res6();
        if (!x6.IsOk) return new(x6.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap(), x3.Unwrap(), x4.Unwrap(),
            x5.Unwrap(), x6.Unwrap()
        ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <param name="res6"></param>
    /// <param name="res7"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5, T6, T7)> AndAll
        <T1, T2, T3, T4, T5, T6, T7>(
        Func<Res<T1>> res1, Func<Res<T2>> res2, Func<Res<T3>> res3, Func<Res<T4>> res4,
        Func<Res<T5>> res5, Func<Res<T6>> res6, Func<Res<T7>> res7
        )
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        var x3 = res3();
        if (!x3.IsOk) return new(x3.ErrorMessage().Unwrap(), "AndAll", null);

        var x4 = res4();
        if (!x4.IsOk) return new(x4.ErrorMessage().Unwrap(), "AndAll", null);

        var x5 = res5();
        if (!x5.IsOk) return new(x5.ErrorMessage().Unwrap(), "AndAll", null);

        var x6 = res6();
        if (!x6.IsOk) return new(x6.ErrorMessage().Unwrap(), "AndAll", null);

        var x7 = res7();
        if (!x7.IsOk) return new(x7.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap(), x3.Unwrap(), x4.Unwrap(),
            x5.Unwrap(), x6.Unwrap(), x7.Unwrap()
        ));
    }
    /// <summary>
    /// Returns the tuple combining unwrapped values of the results if all of them are of Ok variant; returns first Err otherwise.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <param name="res1"></param>
    /// <param name="res2"></param>
    /// <param name="res3"></param>
    /// <param name="res4"></param>
    /// <param name="res5"></param>
    /// <param name="res6"></param>
    /// <param name="res7"></param>
    /// <param name="res8"></param>
    /// <returns></returns>
    public static Res<(T1, T2, T3, T4, T5, T6, T7, T8)> AndAll
        <T1, T2, T3, T4, T5, T6, T7, T8>(
        Func<Res<T1>> res1, Func<Res<T2>> res2, Func<Res<T3>> res3, Func<Res<T4>> res4,
        Func<Res<T5>> res5, Func<Res<T6>> res6, Func<Res<T7>> res7, Func<Res<T8>> res8)
    {
        var x1 = res1();
        if (!x1.IsOk) return new(x1.ErrorMessage().Unwrap(), "AndAll", null);

        var x2 = res2();
        if (!x2.IsOk) return new(x2.ErrorMessage().Unwrap(), "AndAll", null);

        var x3 = res3();
        if (!x3.IsOk) return new(x3.ErrorMessage().Unwrap(), "AndAll", null);

        var x4 = res4();
        if (!x4.IsOk) return new(x4.ErrorMessage().Unwrap(), "AndAll", null);

        var x5 = res5();
        if (!x5.IsOk) return new(x5.ErrorMessage().Unwrap(), "AndAll", null);

        var x6 = res6();
        if (!x6.IsOk) return new(x6.ErrorMessage().Unwrap(), "AndAll", null);

        var x7 = res7();
        if (!x7.IsOk) return new(x7.ErrorMessage().Unwrap(), "AndAll", null);

        var x8 = res8();
        if (!x8.IsOk) return new(x8.ErrorMessage().Unwrap(), "AndAll", null);

        return new((
            x1.Unwrap(), x2.Unwrap(), x3.Unwrap(), x4.Unwrap(),
            x5.Unwrap(), x6.Unwrap(), x7.Unwrap(), x8.Unwrap()
        ));
    }
}
