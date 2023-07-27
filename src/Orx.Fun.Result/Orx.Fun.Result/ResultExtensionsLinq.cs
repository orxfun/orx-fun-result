namespace Orx.Fun.Result;

/// <summary>
/// Extension methods for linq methods using the result types <see cref="Res"/> and <see cref="Res{T}"/>.
/// </summary>
public static class ResultExtensionsLinq
{
    // first/last
    /// <summary>
    /// Returns Some of the value of first Ok result of the <paramref name="collection"/> if any; None otherwise.
    /// <code>
    /// var results = new Res&lt;int>[3] { Err&lt;int>("err"), Ok(42), Ok(7) };
    /// Assert.Equal(Some(42), results.FirstOk());
    /// 
    /// results = new Res&lt;int>[1] { Err&lt;int>("err") };
    /// Assert.True(results.FirstOk().IsNone);
    /// </code>
    /// </summary>
    /// <param name="collection">Collection.</param>
    public static Opt<T> FirstOk<T>(this IEnumerable<Res<T>> collection)
    {
        foreach (var item in collection)
            if (item.IsOk)
                return Some(item.Unwrap());
        return None<T>();
    }
    /// <summary>
    /// Returns Some of the value of last Ok result of the <paramref name="collection"/> if any; None otherwise.
    /// <code>
    /// var results = new Res&lt;int>[3] { Err&lt;int>("err"), Ok(42), Ok(7) };
    /// Assert.Equal(Some(7), results.LastOk());
    /// 
    /// results = new Res&lt;int>[1] { Err&lt;int>("err") };
    /// Assert.True(results.LastOk().IsNone);
    /// </code>
    /// </summary>
    /// <param name="collection">Collection.</param>
    public static Opt<T> LastOk<T>(this IEnumerable<Res<T>> collection)
    {
        var reversed = collection.Reverse();
        foreach (var item in reversed)
            if (item.IsOk)
                return Some(item.Unwrap());
        return None<T>();
    }
    /// <summary>
    /// Returns Some of the error message of first Err result of the <paramref name="collection"/> if any; None otherwise.
    /// <code>
    /// var results = new Res&lt;int>[3] { Ok(42), Err&lt;int>("err"), Err&lt;int>("second-err") };
    /// Assert.Equal(Some("err"), results.FirstErr());
    /// 
    /// results = new Res&lt;int>[1] { Ok(7) };
    /// Assert.True(results.FirstErr().IsNone);
    /// </code>
    /// </summary>
    /// <param name="collection">Collection.</param>
    public static Opt<string> FirstErr<T>(this IEnumerable<Res<T>> collection)
    {
        foreach (var item in collection)
            if (item.IsErr)
                return item.ErrorMessage();
        return None<string>();
    }
    /// <summary>
    /// Returns Some of the value of last Ok result of the <paramref name="collection"/> if any; None otherwise.
    /// <code>
    /// var results = new Res&lt;int>[3] { Ok(42), Err&lt;int>("err"), Err&lt;int>("second-err") };
    /// Assert.Equal(Some("second-err"), results.LastErr());
    /// 
    /// results = new Res&lt;int>[1] { Ok(7) };
    /// Assert.True(results.LastErr().IsNone);
    /// </code>
    /// </summary>
    /// <param name="collection">Collection.</param>
    public static Opt<string> LastErr<T>(this IEnumerable<Res<T>> collection)
    {
        var reversed = collection.Reverse();
        foreach (var item in reversed)
            if (item.IsErr)
                return item.ErrorMessage();
        return None<string>();
    }


    // any/all
    /// <summary>
    /// Returns whether any of the results in the <paramref name="collection"/> are Ok or not.
    /// </summary>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AnyOk(this IEnumerable<Res> collection)
        => collection.Any(x => x.IsOk);
    /// <summary>
    /// Returns whether any of the results results in the <paramref name="collection"/> are Ok or not.
    /// </summary>
    /// <typeparam name="T">Underlying value type of the results.</typeparam>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AnyOk<T>(this IEnumerable<Res<T>> collection)
        => collection.Any(x => x.IsOk);
    /// <summary>
    /// Returns whether all results in the <paramref name="collection"/> are Ok or not.
    /// </summary>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AllOk(this IEnumerable<Res> collection)
        => collection.Any(x => x.IsOk);
    /// <summary>
    /// Returns whether all results in the <paramref name="collection"/> are Ok or not.
    /// </summary>
    /// <typeparam name="T">Underlying value type of the results.</typeparam>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AllOk<T>(this IEnumerable<Res<T>> collection)
        => collection.Any(x => x.IsOk);
    /// <summary>
    /// Returns whether any of the results in the <paramref name="collection"/> are Err or not.
    /// </summary>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AnyErr(this IEnumerable<Res> collection)
        => collection.Any(x => x.IsErr);
    /// <summary>
    /// Returns whether any of the results results in the <paramref name="collection"/> are Err or not.
    /// </summary>
    /// <typeparam name="T">Underlying value type of the results.</typeparam>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AnyErr<T>(this IEnumerable<Res<T>> collection)
        => collection.Any(x => x.IsErr);
    /// <summary>
    /// Returns whether all results in the <paramref name="collection"/> are Err or not.
    /// </summary>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AllErr(this IEnumerable<Res> collection)
        => collection.Any(x => x.IsErr);
    /// <summary>
    /// Returns whether all results in the <paramref name="collection"/> are Err or not.
    /// </summary>
    /// <typeparam name="T">Underlying value type of the results.</typeparam>
    /// <param name="collection">Results collection.</param>
    /// <returns></returns>
    public static bool AllErr<T>(this IEnumerable<Res<T>> collection)
        => collection.Any(x => x.IsErr);


    // unwrap
    /// <summary>
    /// Unwraps all elements in the collection and returns Ok of the resulting list.
    /// If any of the elements is Err; then, the method returns the mapped error.
    /// 
    /// <code>
    /// var array = new Res&lt;int>[3] { Ok(0), Ok(1), Ok(2) };
    /// Res&lt;List&lt;int>> unwrapped = array.MapUnwrap();
    /// Assert.True(unwrapped.IsOk);
    /// Assert.Equal(new int[3] { 0, 1, 2 }, unwrapped.Unwrap());
    /// 
    /// var arrayWithErr = new Res&lt;int>[3] { Ok(0), Err&lt;int>("errmsg"), Ok(2) };
    /// Res&lt;List&lt;int>> unwrappedWithErr = arrayWithErr.MapUnwrap();
    /// Assert.True(unwrappedWithErr.IsErr);
    /// Assert.Equal(Some("errmsg"), unwrappedWithErr.ErrorMessage());
    /// </code>
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="collection">Collection.</param>
    /// <returns></returns>
    public static Res<List<T>> MapUnwrap<T>(this IEnumerable<Res<T>> collection)
    {
        var list = collection.GetNonEnumeratedCount().Map(count => new List<T>(count)).UnwrapOr(() => new List<T>());
        foreach (var item in collection)
        {
            if (item.IsErr)
                return item.ToErrOf<List<T>>();
            else
                list.Add(item.Unwrap());
        }
        return new Res<List<T>>(list);
    }
    /// <summary>
    /// Returns unwrapped values of the results of Ok variant in the <paramref name="collection"/>.
    /// 
    /// <code>
    /// var array = new Res&lt;int>[3] { Ok(0), Err&lt;int>("errmsg"), Ok(2) };
    /// List&lt;int> unwrapped = array.FilterMapUnwrap();
    /// Assert.Equal(new int[3] { 0, 2 }, unwrapped);
    /// </code>
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="collection">Collection.</param>
    /// <returns></returns>
    public static IEnumerable<T> FilterMapUnwrap<T>(this IEnumerable<Res<T>> collection)
    {
        foreach (var item in collection)
            if (item.IsOk)
                yield return item.Unwrap();
    }
}
