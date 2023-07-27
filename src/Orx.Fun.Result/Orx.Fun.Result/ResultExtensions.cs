using System.Runtime.CompilerServices;

namespace Orx.Fun.Result;

/// <summary>
/// Extension methods for the result types <see cref="Res"/> and <see cref="Res{T}"/>.
/// </summary>
public static class ResultExtensions
{
    // res
    /// <summary>
    /// Creates a result as the Ok variant.
    /// <code>
    /// Res result = Ok();
    /// Assert(result.IsOk);
    /// </code>
    /// </summary>
    public static Res Ok()
        => default;
    /// <summary>
    /// Creates a result as the Err variant; with the given error information: <paramref name="errorMessage"/>.
    /// <code>
    /// static Res AddUser(User user)
    /// {
    ///     if (AlreadyExists(user))
    ///         return Err($"user '{user.Id}' already exists.");
    ///     if (HasAvailableCapacity(session))
    ///         return Err("not enough capacity");
    ///     else
    ///     {
    ///         // add user
    ///         return Ok();
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    public static Res Err(string errorMessage)
        => new(errorMessage, string.Empty, null);
    /// <summary>
    /// Creates a result as the Err variant; with the given error information: <paramref name="errorMessage"/>, <paramref name="when"/>.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <param name="when">Operation when the error is observed.</param>
    public static Res Err(string errorMessage, string when)
        => new(errorMessage, when, null);
    /// <summary>
    /// Creates a result as the Err variant; with the given error information: <paramref name="when"/>, <paramref name="exception"/>.
    /// <code>
    /// static Res PutItem(Item item)
    /// {
    ///     try
    ///     {
    ///         PutItemToDatabase(item);
    ///         return Ok();
    ///     }
    ///     catch (Exception e)
    ///     {
    ///         return Err(nameof(PutItem), e);
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="when">Operation when the error is observed.</param>
    /// <param name="exception">Exception causing the error.</param>
    public static Res Err(string when, Exception exception)
        => new(string.Empty, when, exception);
    /// <summary>
    /// Creates a result as the Err variant; with the given error information: <paramref name="errorMessage"/>, <paramref name="when"/>, <paramref name="exception"/>.
    /// <code>
    /// static Res PutItem(Item item)
    /// {
    ///     try
    ///     {
    ///         PutItemToDatabase(item);
    ///         return Ok();
    ///     }
    ///     catch (Exception e)
    ///     {
    ///         return Err("failed to execute sql command.", nameof(PutItem), e);
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <param name="when">Operation when the error is observed.</param>
    /// <param name="exception">Exception causing the error.</param>
    public static Res Err(string errorMessage, string when, Exception exception)
        => new(errorMessage, when, exception);

    /// <summary>
    /// Creates a result as Ok variant if the <paramref name="okCondition"/> holds.
    /// Otherwise, it will map into an Err.
    /// <code>
    /// static Res ValidateInput(Form form)
    /// {
    ///     return OkIf(!form.HasEmptyFields())
    ///         .OkIf(form.Date &lt;= DateTime.Now)
    ///         // chained validation calls
    ///         .OkIf(repo.AlreadyContains(form.Id));
    /// }
    /// </code>
    /// </summary>
    /// <param name="okCondition">Condition that must hold for the return value to be Ok().</param>
    /// <param name="strOkCondition">Name of the condition; to be appended to the error message if it does not hold. Omitting the argument will automatically be filled with the condition's expression in the caller side.</param>
    public static Res OkIf(bool okCondition, [CallerArgumentExpression("okCondition")] string strOkCondition = "")
        => okCondition ? default : new(strOkCondition, "failed OkIf validation", null);
    /// <summary>
    /// Flattens the result of result; i.e., Res&lt;Res> -> Res, by mapping:
    /// <list type="bullet">
    /// <item>Err => Err,</item>
    /// <item>Ok(Err) => Err,</item>
    /// <item>Ok(Ok) => Ok.</item>
    /// </list>
    /// <code>
    /// Res&lt;Res> nestedResult = Err&lt;Res>("msg");
    /// Res result = nestedResult.Flatten();
    /// Assert(result.IsErr and result.ErrorMessage() == Some("msg"));
    /// 
    /// Res&lt;Res> nestedResult = Ok(Err("msg"));
    /// Res result = nestedResult.Flatten();
    /// Assert(result.IsErr and result.ErrorMessage() == Some("msg"));
    /// 
    /// Res&lt;Res> nestedResult = Ok(Ok());
    /// Res result = nestedResult.Flatten();
    /// Assert(result.IsOk);
    /// </code>
    /// </summary>
    /// <param name="result">Res of Res to be flattened.</param>
    public static Res Flatten(this Res<Res> result)
    {
        if (result.IsErr)
            return new(result.ToString(), string.Empty, null);
        else
            return result.Unwrap();
    }

    // res-t
    /// <summary>
    /// Creates a result of <typeparamref name="T"/> as Ok variant with value <paramref name="value"/>.
    /// However, if the <paramref name="value"/> is null, it will map into Err.
    /// <code>
    /// Res&lt;double> number = Ok(42.5);
    /// Assert(number.IsOk and number.Unwrap() == 42.5);
    /// 
    /// // on the other hand:
    /// string name = null;
    /// Res&lt;string> optName = Ok(name);
    /// Assert(optName.IsErr);
    /// </code>
    /// </summary>
    /// <param name="value">Expectedly non-null value of T.</param>
    public static Res<T> Ok<T>(T value)
        => new(value);
    /// <summary>
    /// Creates a result of <typeparamref name="T"/> as Err with the given <paramref name="errorMessage"/>.
    /// <code>
    /// static Res&lt;double> Divide(double number, double divider)
    /// {
    ///     if (divider == 0)
    ///         return Err&lt;double>("Cannot divide to zero");
    ///     else
    ///         return Ok(number / divider);
    /// }
    /// </code>
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    public static Res<T> Err<T>(string errorMessage)
        => new(errorMessage, string.Empty, null);
    /// <summary>
    /// Creates a result of <typeparamref name="T"/> as Err with the given <paramref name="errorMessage"/> which is observed during <paramref name="when"/>.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <param name="when">Operation when the error is observed.</param>
    public static Res<T> Err<T>(string errorMessage, string when)
        => new(errorMessage, when, null);

    /// <summary>
    /// Creates a result of <typeparamref name="T"/> as Ok variant with value <paramref name="value"/> if the <paramref name="okCondition"/> holds.
    /// Otherwise, it will map into an Err.
    /// <code>
    /// Shape shape = GetShape(); // valid only if shape has a positive base area.
    /// Res&lt;Shape> resultShape = OkIf(shape.GetBaseArea() > 0, shape);
    /// </code>
    /// </summary>
    /// <param name="okCondition">Condition that must hold for the return value to be Ok(value).</param>
    /// <param name="value">Underlying value of the Ok variant to be returned if okCondition holds.</param>
    /// <param name="strOkCondition">Name of the condition; to be appended to the error message if it does not hold. Omitting the argument will automatically be filled with the condition's expression in the caller side.</param>
    public static Res<T> OkIf<T>(bool okCondition, T value, [CallerArgumentExpression("okCondition")] string strOkCondition = "")
        => okCondition ? new(value) : new(strOkCondition, "failed OkIf validation", null);
    /// <summary>
    /// Creates a result of <typeparamref name="T"/> as Ok variant with value <paramref name="lazyGetValue"/>() if the <paramref name="okCondition"/> holds.
    /// Otherwise, it will map into an Err.
    /// Note that the <paramref name="lazyGetValue"/> is only evaluated if the <paramref name="okCondition"/> holds.
    /// <code>
    /// Res&lt;User> user = TryGetUser();
    /// // create a database connection (expensive) only if the user IsOk.
    /// Res&lt;Conn> conn = OkIf(user.IsOk, () => CreateDatabaseConnection());
    /// </code>
    /// </summary>
    /// <param name="okCondition">Condition that must hold for the return value to be Ok(value).</param>
    /// <param name="lazyGetValue">Function to create the underlying value of the Ok variant to be returned if okCondition holds.</param>
    /// <param name="strOkCondition">Name of the condition; to be appended to the error message if it does not hold. Omitting the argument will automatically be filled with the condition's expression in the caller side.</param>
    public static Res<T> OkIf<T>(bool okCondition, Func<T> lazyGetValue, [CallerArgumentExpression("okCondition")] string strOkCondition = "")
        => okCondition ? new(lazyGetValue()) : new(strOkCondition, "failed OkIf validation", null);
    // ctors - extension
    /// <summary>
    /// Creates a result of <typeparamref name="T"/> as Ok variant with the given <paramref name="value"/>.
    /// However, if the <paramref name="value"/> is null, it will map into Err.
    /// <code>
    /// string name = null;
    /// static string? GetName(int id)
    ///     => id == 0 ? "Mr Crabs" : null;
    /// Res&lt;string> resName = GetName(0).OkIfNotnull();
    /// Assert.Equal(Ok("Mr Crabs"), resName);
    /// 
    /// resName = GetName(42).OkIfNotnull();
    /// Assert.True(resName.IsErr);
    /// </code>
    /// </summary>
    /// <param name="value">A nullable value of T to be converted to the result type.</param>
    public static Res<T> OkIfNotnull<T>(this T? value) where T : class
        => new(value);
    /// <summary>
    /// Flattens the result of result of <typeparamref name="T"/>; i.e., Res&lt;Res&lt;T>> -> Res&lt;T>, by mapping:
    /// <list type="bullet">
    /// <item>Err => Err,</item>
    /// <item>Ok(Err) => Err,</item>
    /// <item>Ok(Ok(value)) => Ok(value).</item>
    /// </list>
    /// <code>
    /// Res&lt;Res&lt;int>> nestedResult = Err&lt;Res&lt;int>>("msg");
    /// Res&lt;int> result = nestedResult.Flatten();
    /// Assert(result.IsErr and result.ErrorMessage() == Some("msg"));
    /// 
    /// Res&lt;Res&lt;int>> nestedResult = Ok(Err&lt;int>("msg"));
    /// Res&lt;int> result = nestedResult.Flatten();
    /// Assert(result.IsErr and result.ErrorMessage() == Some("msg"));
    /// 
    /// Res&lt;Res&lt;int>> nestedResult = Ok(Ok(42));
    /// Res&lt;int> result = nestedResult.Flatten();
    /// Assert(result.IsOk and result.Unwrap() == 42);
    /// </code>
    /// </summary>
    public static Res<T> Flatten<T>(this Res<Res<T>> result)
    {
        if (result.IsErr)
            return new(result.ToString(), string.Empty, null);
        else
            return result.Unwrap();
    }


    // res-t: map - match with tuples
    /// <summary>
    /// Allows a result of a tuple (t1, t2) to map with a function taking two arguments t1 and t2.
    /// 
    /// <code>
    /// static int Add(int a, int b) => a + b;
    /// 
    /// var numbers = Ok((1, 2));
    /// var sum = numbers.Map(Add);
    /// Assert(sum == Some(3));
    /// </code>
    /// 
    /// This is mostly useful in enabling function composition.
    /// </summary>
    /// <typeparam name="T1">Type of the first argument of the map function.</typeparam>
    /// <typeparam name="T2">Type of the second argument of the map function.</typeparam>
    /// <typeparam name="TOut">Type of return value of the map function.</typeparam>
    /// <param name="result">Result to be mapped.</param>
    /// <param name="map">Map function.</param>
    /// <returns></returns>
    public static Res<TOut> Map<T1, T2, TOut>(this Res<(T1, T2)> result, Func<T1, T2, TOut> map)
        => result.Map(x => map(x.Item1, x.Item2));
    /// <summary>
    /// (async version) Allows a result of a tuple (t1, t2) to map with a function taking two arguments t1 and t2.
    /// 
    /// <code>
    /// static int Add(int a, int b) => a + b;
    /// 
    /// var numbers = Ok((1, 2));
    /// var sum = numbers.Map(Add);
    /// Assert(sum == Some(3));
    /// </code>
    /// 
    /// This is mostly useful in enabling function composition.
    /// </summary>
    /// <typeparam name="T1">Type of the first argument of the map function.</typeparam>
    /// <typeparam name="T2">Type of the second argument of the map function.</typeparam>
    /// <typeparam name="TOut">Type of return value of the map function.</typeparam>
    /// <param name="result">Result to be mapped.</param>
    /// <param name="map">Map function.</param>
    /// <returns></returns>
    public static Task<Res<TOut>> Map<T1, T2, TOut>(this Res<(T1, T2)> result, Func<T1, T2, Task<TOut>> map)
        => result.MapAsync(x => map(x.Item1, x.Item2));


    // opt-t to res-t
    /// <summary>
    /// Shorthand for mapping options to results as follows:
    /// <list type="bullet">
    /// <item>None&lt;T> => Err&lt;T> with a generic error message;</item>
    /// <item>Some(T) => Ok(T).</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    /// <param name="option">Option to be converted to result.</param>
    /// <returns></returns>
    public static Res<T> IntoRes<T>(this Opt<T> option)
        => option.IsSome ? Ok(option.Unwrap()) : Err<T>(string.Format("{0} is called on None of {1}.", nameof(IntoRes), typeof(T).Name));
    /// <summary>
    /// Shorthand for mapping options to results as follows:
    /// <list type="bullet">
    /// <item>None&lt;T> => Err&lt;T> with the given error message;</item>
    /// <item>Some(T) => Ok(T).</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    /// <param name="option">Option to be converted to result.</param>
    /// <param name="errorMessageIfNone">Error message to be stored in the Err to be returned if the option is None.</param>
    /// <returns></returns>
    public static Res<T> IntoRes<T>(this Opt<T> option, string errorMessageIfNone)
        => option.IsSome ? Ok(option.Unwrap()) : Err<T>(errorMessageIfNone);


    // collections
    /// <summary>
    /// Applies the result mapper to the collection and reduces it to a single result:
    /// <list type="bullet">
    /// <item>returns Ok when all results are Ok;</item>
    /// <item>Ok if the collection is empty;</item>
    /// <item>the first Err otherwise.</item>
    /// </list>
    /// </summary>
    /// <param name="collection">Collection of values.</param>
    /// <param name="map">Function that maps each element of the collection to a result.</param>
    /// <returns></returns>
    public static Res MapReduce<T>(this IEnumerable<T> collection, Func<T, Res> map)
        => collection.Select(map).Reduce();
    /// <summary>
    /// (async version) <inheritdoc cref="MapReduce{T}(IEnumerable{T}, Func{T, Res})"/>
    /// </summary>
    /// <param name="collection">Collection of values.</param>
    /// <param name="map">Function that maps each element of the collection to a result.</param>
    /// <returns></returns>
    public static Task<Res> MapReduceAsync<T>(this IEnumerable<T> collection, Func<T, Task<Res>> map)
        => collection.Select(map).ReduceAsync();
    /// <summary>
    /// Applies the result mapper to the collection and reduces it to a single result:
    /// <list type="bullet">
    /// <item>returns Ok(List&lt;T>) when all results are Ok;</item>
    /// <item>Ok(empty list of T) if the collection is empty;</item>
    /// <item>the first Err otherwise.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">Type of the underlying values of the collection.</typeparam>
    /// <typeparam name="TOut">Type of the underlying values of the mapped results.</typeparam>
    /// <param name="collection">Collection of results of T.</param>
    /// <param name="map">Function that maps each element of the collection to a result.</param>
    /// <returns></returns>
    public static Res<List<TOut>> MapReduce<T, TOut>(this IEnumerable<T> collection, Func<T, Res<TOut>> map)
        => collection.Select(map).Reduce();
    /// <summary>
    /// (async version) <inheritdoc cref="MapReduce{T, TOut}(IEnumerable{T}, Func{T, Res{TOut}})"/>
    /// </summary>
    /// <typeparam name="T">Type of the underlying values of the collection.</typeparam>
    /// <typeparam name="TOut">Type of the underlying values of the mapped results.</typeparam>
    /// <param name="collection">Collection of results of T.</param>
    /// <param name="map">Function that maps each element of the collection to a result.</param>
    /// <returns></returns>
    public static Task<Res<List<TOut>>> MapReduceAsync<T, TOut>(this IEnumerable<T> collection, Func<T, Task<Res<TOut>>> map)
        => collection.Select(map).ReduceAsync();
    /// <summary>
    /// Reduces the collection of results to a single result:
    /// <list type="bullet">
    /// <item>returns Ok when all results are Ok;</item>
    /// <item>Ok if the collection is empty;</item>
    /// <item>the first Err otherwise.</item>
    /// </list>
    /// </summary>
    /// <param name="results">Collection of results.</param>
    /// <returns></returns>
    public static Res Reduce(this IEnumerable<Res> results)
    {
        foreach (var item in results)
            if (item.IsErr)
                return item;
        return Ok();
    }
    /// <summary>
    /// (async version) <inheritdoc cref="Reduce(IEnumerable{Res})"/>
    /// </summary>
    /// <param name="results">Collection of results.</param>
    /// <returns></returns>
    public static async Task<Res> ReduceAsync(this IEnumerable<Task<Res>> results)
    {
        var awaitedResults = await Task.WhenAll(results);
        return awaitedResults.Reduce();
    }
    /// <summary>
    /// Reduces the collection of results to result of list of values:
    /// <list type="bullet">
    /// <item>returns Ok(List&lt;T>) when all results are Ok;</item>
    /// <item>Ok(empty list of T) if the collection is empty;</item>
    /// <item>the first Err otherwise.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    /// <param name="results">Collection of results of T.</param>
    /// <returns></returns>
    public static Res<List<T>> Reduce<T>(this IEnumerable<Res<T>> results)
    {
        bool knownCount = results.TryGetNonEnumeratedCount(out int count);
        var values = knownCount ? new List<T>(count) : new List<T>();
        foreach (var item in results)
            if (item.IsOk)
                values.Add(item.Unwrap());
            else
                return item.ToErrOf<List<T>>();
        return Ok(values);
    }
    /// <summary>
    /// (async version) <inheritdoc cref="Reduce{T}(IEnumerable{Res{T}})"/>
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    /// <param name="results">Collection of results of T.</param>
    /// <returns></returns>
    public static async Task<Res<List<T>>> ReduceAsync<T>(this IEnumerable<Task<Res<T>>> results)
    {
        var awaitedResults = await Task.WhenAll(results);
        return awaitedResults.Reduce();
    }
}
