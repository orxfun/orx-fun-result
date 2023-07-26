using static Orx.Fun.Result.ResultExtensions;
using System.Runtime.CompilerServices;

namespace Orx.Fun.Result;

/// <summary>
/// Result type which can be either of the two variants: Ok(value-of-<typeparamref name="T"/>) or Err(error-message).
/// </summary>
public readonly struct Res<T> : IEquatable<Res<T>>
{
    // data
    readonly T? Val;
    readonly string? Err;


    // prop
    /// <summary>
    /// True if the result is Ok; false otherwise.
    /// </summary>
    public bool IsOk
        => Err == null;
    /// <summary>
    /// True if the result is Err; false otherwise.
    /// </summary>
    public bool IsErr
        => Err != null;


    // ctor
    /// <summary>
    /// Result type which can either be Ok(value) or Err.
    /// Parameterless ctor returns Ok(default(T)); better use <see cref="Ok{T}(T)"/> or <see cref="Err{T}(string)"/> to construct results by adding `using static OptRes.Ext`.
    /// </summary>
    public Res()
    {
        Val = default;
        Err = null;
    }
    internal Res(T? value)
    {
        Val = value;
        if (typeof(T).IsClass)
            Err = value != null ? null : "Null value";
        else
            Err = null;
    }
    internal Res(string msg, string when, Exception? e)
    {
        Val = default;
        Err = ErrConfig.GetErrorString((msg, when, e));
    }


    /// <summary>
    /// Converts into <see cref="Res"/> dropping the value if it <see cref="IsOk"/>.
    /// </summary>
    /// <returns></returns>
    public Res WithoutVal()
        => Err == null ? new() : new(Err, string.Empty, null);


    // throw
    /// <summary>
    /// Returns the result back when <see cref="IsOk"/>; throws a NullReferenceException when <see cref="IsErr"/>.
    /// <code>
    /// static Res&lt;User> QueryUser(..) {
    ///     // might fail; hence, returns a Res&lt;User> rather than just User.
    /// }
    /// var result = QueryUser(..).ThrowIfErr();
    /// // result will be:
    /// // - Ok(user) if QueryUser succeeds and returns Ok of the user;
    /// // - the application will throw otherwise.
    /// </code>
    /// </summary>
    public Res<T> ThrowIfErr()
    {
        if (Err != null)
            throw new NullReferenceException(Err);
        else
            return this;
    }
    /// <summary>
    /// Returns the result back when <see cref="IsOk"/>; throws an exception of <typeparamref name="E"/> when <see cref="IsErr"/>.
    /// <code>
    /// static Res MakeApiCall() {
    ///     // method that makes an api call.
    ///     // might fail; hence, returns a Res rather than void.
    /// }
    /// var result = MakeApiCall().ThrowIfErr&lt;HttpRequestException>(err => new(err));
    /// // result will be:
    /// // - Ok() if MakeApiCall succeeds and returns Ok;
    /// // - the application will throw HttpRequestException created by the provided delegate otherwise.
    /// </code>
    /// </summary>
    /// <typeparam name="E">Type of the exception to be thrown.</typeparam>
    /// <param name="errorMessageToException">Function that crates the desired exception from the error message, to be used if the result IsErr.</param>
    public Res<T> ThrowIfErr<E>(Func<string, E> errorMessageToException) where E : Exception
    {
        if (Err != null)
            throw errorMessageToException(Err);
        else
            return this;
    }


    // okif
    /// <summary>
    /// Returns back the Err if this is Err.
    /// Otherwise, returns Ok(value) if <paramref name="condition"/>(value) holds; Err if it does not hold.
    /// Especially useful in fluent input validation.
    /// <code>
    /// static Res&lt;Account> TryParseAccount(..) { }
    /// static bool IsAccountNumberValid(int number) { }
    /// static bool DoesAccountExist(string code) { }
    /// 
    /// var account = TryParseAccount(..)
    ///                 .OkIf(acc => IsAccountNumberValid(acc.Number))
    ///                 .OkIf(acc => DoesAccountExist(acc.Code));
    /// // account will be Ok(account) only if:
    /// // - TryParseAccount returns Ok(account), and further,
    /// // - both IsAccountNumberValid and DoesAccountExist validation checks return true.
    /// </code>
    /// </summary>
    /// <param name="condition">Condition on the underlying value that should hold to get an Ok, rather than Err.</param>
    /// <param name="strOkCondition">Name of the condition; to be appended to the error message if it does not hold. Omitting the argument will automatically be filled with the condition's expression in the caller side.</param>
    public Res<T> OkIf(Func<T, bool> condition, [CallerArgumentExpression("condition")] string strOkCondition = "")
        => IsErr || Val == null ? this : (condition(Val) ? this : new(strOkCondition, "failed OkIf validation", null));


    // unwrap
    /// <summary>
    /// Returns Some(error-message) if IsErr; None&lt;string>() if IsOk.
    /// <code>
    /// var user = Err&lt;User>("failed to get user");
    /// Assert(user.ErrorMessage() == Some("failed to get user"));
    /// </code>
    /// </summary>
    public Opt<string> ErrorMessage()
        => Err == null ? None<string>() : Some(Err);
    /// <summary>
    /// Returns the underlying value when <see cref="IsOk"/>; or throws when <see cref="IsErr"/>.
    /// Must be called shyly, as it is not necessary to unwrap until the final result is achieved due to Map, FlatMap and TryMap methods.
    /// <code>
    /// Res&lt;int> resultAge = "42".ParseIntOrErr();
    /// if (resultAge.IsSome) {
    ///     int age = resultAge.Unwrap(); // use the uwrapped age
    /// } else { // handle the Err case
    /// }
    /// </code>
    /// </summary>
    public T Unwrap()
        => (Err == null && Val != null) ? Val : throw new NullReferenceException(string.Format("Cannot Unwrap Err.\n{0}", Err));
    /// <summary>
    /// Returns the underlying value when <see cref="IsOk"/>; or returns the <paramref name="fallbackValue"/> when <see cref="IsErr"/>.
    /// This is a safe way to unwrap the result, by explicitly handling the Err variant.
    /// Use the lazy <see cref="UnwrapOr(Func{T})"/> variant if the computation of the fallback value is expensive.
    /// <code>
    /// Assert(Ok(42).UnwrapOr(7) == 42);
    /// Assert(Err&lt;int>("error-message").UnwrapOr(7) == 7);
    /// </code>
    /// </summary>
    /// <param name="fallbackValue">Fallback value that will be returned if the result is Err.</param>
    public T UnwrapOr(T fallbackValue)
        => (Err == null && Val != null) ? Val : fallbackValue;
    /// <summary>
    /// Returns the underlying value when <see cref="IsOk"/>; or returns <paramref name="lazyFallbackValue"/>() when <see cref="IsErr"/>.
    /// This is a safe way to unwrap the result, by explicitly handling the Err variant.
    /// Use the eager <see cref="UnwrapOr(T)"/> variant if the fallback value is cheap or readily available.
    /// <code>
    /// static string ParseUserTablename(..) { /*parses the table name from command line input; might throw!*/ }
    /// static string QueryUserTablename(..) { /*makes an expensive db-call to find out the table name*/ }
    /// 
    /// string userTable = Ok()                                         // Res, certainly Ok
    ///                     .TryMap(() => ParseUserTablename(..))       // Res&lt;string>: might be Err if parser throws
    ///                     .UnwrapOr(() => QueryUserTablename(..));    // directly returns ParseUserTablename's result if it is Ok;
    ///                                                                 // calls QueryUserTablename otherwise and returns its result.
    /// </code>
    /// </summary>
    /// <param name="lazyFallbackValue">Function to be called lazily to create the return value if the option is None.</param>
    public T UnwrapOr(Func<T> lazyFallbackValue)
        => (Err == null && Val != null) ? Val : lazyFallbackValue();
    /// <summary>
    /// (async version) <inheritdoc cref="UnwrapOr(Func{T})"/>
    /// </summary>
    /// <param name="lazyFallbackValue">Function to be called lazily to create the return value if the option is None.</param>
    public Task<T> UnwrapOrAsync(Func<Task<T>> lazyFallbackValue)
        => (Err == null && Val != null) ? Task.FromResult(Val) : lazyFallbackValue();


    // match
    /// <summary>
    /// Maps into <paramref name="whenOk"/>(Unwrap()) whenever IsOk; and into <paramref name="whenErr"/>(error-message) otherwise.
    /// <code>
    /// Res&lt;User> user = TryGetUser(..);
    /// string greeting = user.Match(u => $"Welcome back {u.Name}", err => $"Failed to get user. {err}");
    /// // equivalently:
    /// greeting = user.Match(
    ///     whenOk: u => $"Welcome back {u.Name}",
    ///     whenErr: err => $"Failed to get user. {err}"
    /// );
    /// </code>
    /// </summary>
    /// <param name="whenOk">Mapping function (T -> TOut) that will be called with Unwrapped value to get the return value when Ok.</param>
    /// <param name="whenErr">Function of the error message to get the return value when Err.</param>
    public TOut Match<TOut>(Func<T, TOut> whenOk, Func<string, TOut> whenErr)
    {
        if (Err != null)
            return whenErr(Err);
        if (Val != null)
            return whenOk(Val);
        throw Exc.MustNotReach;
    }
    /// <summary>
    /// (async version) <inheritdoc cref="Match{TOut}(Func{T, TOut}, Func{string, TOut})"/>
    /// </summary>
    /// <param name="whenOk">Mapping function (T -> TOut) that will be called with Unwrapped value to get the return value when Ok.</param>
    /// <param name="whenErr">Function of the error message to get the return value when Err.</param>
    public Task<TOut> MatchAsync<TOut>(Func<T, Task<TOut>> whenOk, Func<string, Task<TOut>> whenErr)
    {
        if (Err != null)
            return whenErr(Err);
        if (Val != null)
            return whenOk(Val);
        throw Exc.MustNotReach;
    }
    /// <summary>
    /// Executes <paramref name="whenOk"/>(Unwrap()) if IsOk; <paramref name="whenErr"/>(error-message) otherwise.
    /// <code>
    /// Res&lt;User> user = LoginUser(..);
    /// user.MatchDo(
    ///     whenOk: u => Log.Info($"Logged in user: {u.Name}"),
    ///     whenErr: err => Log.Error($"Failed login. ${err}")
    /// );
    /// </code>
    /// </summary>
    /// <param name="whenOk">Action of the underlying value to be called lazily when IsOk.</param>
    /// <param name="whenErr">Action of error message to be called lazily when IsErr.</param>
    public void MatchDo(Action<T> whenOk, Action<string> whenErr)
    {
        if (Err != null)
            whenErr(Err);
        else if (Val != null)
            whenOk(Val);
        else
            throw Exc.MustNotReach;
    }


    // do
    /// <summary>
    /// Runs <paramref name="action"/>(Unwrap()) only if IsOk; and returns itself back.
    /// <code>
    /// // the logging call will only be made if the result of TryGetUser is Ok of a user.
    /// // Since Do returns back the result, it can still be assigned to var 'user'.
    /// Res&lt;User> user = TryGetUser().Do(u => Log.Info($"User '{u.Name}' grabbed"));
    /// </code>
    /// </summary>
    /// <param name="action">Action that will be called with the underlying value when Ok.</param>
    public Res<T> Do(Action<T> action)
    {
        if (Err == null && Val != null)
            action(Val);
        return this;
    }
    // do-if-err
    /// <summary>
    /// Runs <paramref name="actionOnErr"/>() only if IsErr; and returns itself back.
    /// Counterpart of <see cref="Do(Action{T})"/> for the Err variant.
    /// <code>
    /// // the logging call will only be made if the result of TryGetUser is Err.
    /// // Since DoIfErr returns back the result, it can still be assigned to var 'user'.
    /// Res&lt;User> user = TryGetUser().DoIfErr(err => Log.Warning($"User could not be read. {err}"));
    /// </code>
    /// </summary>
    /// <param name="actionOnErr">Action that will be called when Err.</param>
    public Res<T> DoIfErr(Action<string> actionOnErr)
    {
        if (Err != null)
            actionOnErr(ToString());
        return this;
    }


    // map
    /// <summary>
    /// Returns the Err back when IsErr; Ok(<paramref name="map"/>(Unwrap())) when IsOk.
    /// <code>
    /// // session will be Err if the user is Err; Ok of a session for the user when Ok.
    /// Res&lt;Session> session = TryGetUser.Map(user => NewSession(user.Secrets));
    /// </code>
    /// </summary>
    /// <param name="map">Mapper function (T -> TOut) to be called with the underlying value when Ok.</param>
    public Res<TOut> Map<TOut>(Func<T, TOut> map)
    {
        if (Err == null && Val != null)
            return Ok(map(Val));
        else if (Err != null)
            return Err<TOut>(Err);
        else
            throw Exc.MustNotReach;
    }
    /// <summary>
    /// (async version) <inheritdoc cref="Map{TOut}(Func{T, TOut})"/>
    /// </summary>
    /// <param name="map">Mapper function (T -> TOut) to be called with the underlying value when Ok.</param>
    public async Task<Res<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> map)
    {
        if (Err == null && Val != null)
            return Ok(await map(Val));
        else if (Err != null)
            return Err<TOut>(Err);
        else
            throw Exc.MustNotReach;
    }


    // flatmap
    /// <summary>
    /// Returns the error when IsErr; <paramref name="map"/>(Unwrap()) when IsOk flattenning the result.
    /// Shorthand combining Map and Flatten calls.
    /// <code>
    /// static Res&lt;Team> TryGetTeam() { .. } // tries to grab a team; might fail, hence, returns Res.
    /// static Res TryPutTeam(Team team) { .. } // tries to put the team; might fail, hence, returns Res.
    /// 
    /// Res result = TryGetTeam().FlatMap(TryPutTeam);
    /// // equivalently:
    /// Res result = TryGetTeam().FlatMap(team => TryPutTeam(team));
    /// 
    /// // this is a shorthand for:
    /// Res result = TryGetTeam()   // Res&lt;Team>
    ///     .Map(TryPutTeam)        // Res&lt;Res>
    ///     .Flatten();             // Res
    /// </code>
    /// </summary>
    /// <param name="map">Function (T -> Res) that maps the underlying value to a Res when IsOk.</param>
    public Res FlatMap(Func<T, Res> map)
    {
        if (Err == null && Val != null)
            return map(Val);
        else if (Err != null)
            return Err(Err);
        else
            throw Exc.MustNotReach;
    }
    /// <summary>
    /// (async version) <inheritdoc cref="FlatMap(Func{T, Res})"/>
    /// </summary>
    /// <param name="map">Function (T -> Res) that maps the underlying value to a Res when IsOk.</param>
    public Task<Res> FlatMapAsync(Func<T, Task<Res>> map)
    {
        if (Err == null && Val != null)
            return map(Val);
        else if (Err != null)
            return Task.FromResult(Err(Err));
        else
            throw Exc.MustNotReach;
    }
    /// <summary>
    /// Returns None when IsNone; <paramref name="map"/>(val) when IsOk flattening the result.
    /// Shorthand combining Map and Flatten calls.
    /// <code>
    /// static Res&lt;User> TryGetUser() {
    ///     // method that tries to get the user, return Ok(user) or Err.
    /// }
    /// static Res&lt; double> TryGetBalance(User user) {
    ///     // method that tries to get usedr's balance; which might fail, returns:
    ///     // Ok(balance) or Err
    /// }
    /// Res&lt;double> balance = TryGetUser().FlatMap(TryGetBalance);
    /// // equivalent to both below:
    /// var balance = TryGetUser().FlatMap(user => TryGetBalance(user));
    /// var balance = TryGetUser()              // Res&lt;User>
    ///     .Map(user => TryGetBalance(user))   // Res&lt;Res>
    ///     .Flatten();                         // Res
    /// </code>
    /// </summary>
    /// <param name="map">Function (T -> Res&lt;TOut>) mapping the underlying value to result of TOut if this.IsOk.</param>
    public Res<TOut> FlatMap<TOut>(Func<T, Res<TOut>> map)
    {
        if (Err == null && Val != null)
            return map(Val);
        else if (Err != null)
            return Err<TOut>(Err);
        else
            throw Exc.MustNotReach;
    }
    /// <summary>
    /// (async version) <inheritdoc cref="FlatMap{TOut}(Func{T, Res{TOut}})"/>
    /// </summary>
    /// <param name="map">Function (T -> Res&lt;TOut>) mapping the underlying value to result of TOut if this.IsOk.</param>
    public Task<Res<TOut>> FlatMapAsync<TOut>(Func<T, Task<Res<TOut>>> map)
    {
        if (Err == null && Val != null)
            return map(Val);
        else if (Err != null)
            return Task.FromResult(Err<TOut>(Err));
        else
            throw Exc.MustNotReach;
    }
    // flatmapback
    /// <summary>
    /// Returns the error when IsErr; <paramref name="map"/>(Unwrap()).Map(() => this) when IsOk flattenning the result.
    /// Shorthand combining Map, Flatten and Map calls.
    /// 
    /// <para>
    /// Note that the difference of <see cref="FlatMapBack(Func{T, Res})"/> from <see cref="FlatMap(Func{T, Res})"/>
    /// is in the return type; returns Res&lt;T> rather than Res.
    /// </para>
    /// 
    /// <para>
    /// It appends back the original value to the result if the result was and is Ok after the map call.
    /// </para>
    /// 
    /// <code>
    /// static Res&lt;Team> TryGetTeam() { .. } // tries to grab a team; might fail, hence, returns Res.
    /// static Res TryPutTeam(Team team) { .. } // tries to put the team; might fail, hence, returns Res.
    /// 
    /// Res&lt;Team> result = TryGetTeam().FlatMapBack(TryPutTeam);
    /// // equivalently:
    /// Res&lt;Team> result = TryGetTeam().FlatMapBack(team => TryPutTeam(team));
    /// 
    /// // this is a shorthand for:
    /// Res&lt;Team> result = TryGetTeam()                               // Res&lt;Team>
    ///     .FlatMap(team => TryPutTeam(team).Map(() => team)); // Res&lt;Team>
    /// </code>
    /// </summary>
    /// <param name="map">Function (T -> Res) that maps the underlying value to a Res when IsOk.</param>
    public Res<T> FlatMapBack(Func<T, Res> map)
    {
        var flatmap = FlatMap(map);
        if (flatmap.IsErr)
            return flatmap.Err is null ? throw Exc.MustNotReach : Err<T>(flatmap.Err);
        else
            return this;
    }
    /// <summary>
    /// (async version) <inheritdoc cref="FlatMapBack(Func{T, Res})"/>
    /// </summary>
    /// <param name="map">Function (T -> Res) that maps the underlying value to a Res when IsOk.</param>
    public async Task<Res<T>> FlatMapBackAsync(Func<T, Task<Res>> map)
    {
        var flatmap = await FlatMapAsync(map);
        if (flatmap.IsErr)
            return flatmap.Err is null ? throw Exc.MustNotReach : Err<T>(flatmap.Err);
        else
            return this;
    }


    // try
    /// <summary>
    /// When IsOk executes <paramref name="action"/>(val) in a try-catch block: returns Ok if the process succeeds; Err if it throws.
    /// Does not do anything and returns the Err when this IsErr.
    /// <code>
    /// static Res&lt;User> TryGetUser() { .. }
    /// static void PutUserToDb(User user) {
    ///     // method that writes the user to a database table
    ///     // might fail and throw!
    /// }
    /// 
    /// Res&lt;User> user = TryGetUser().Try(PutUserToDb);
    /// // equivalently:
    /// Res&lt;User> user = TryGetUser().Try(() => PutUserToDb());
    /// 
    /// // user will be:
    /// // - Err(called on Err) if () returns Err.
    /// // - Err(relevant error message) if () returns Ok(user) but database action throws an exception.
    /// // - Ok(user) if () returns Ok(user), further the action is operated successfully;
    ///
    /// // it provides a shorthand for the following verbose/unpleasant version:
    /// Res&lt;User> user = TryGetUser();
    /// if (user.IsOk)
    /// {
    ///     try
    ///     {
    ///         PutUserToDb(user.Unwrap());
    ///     }
    ///     catch (Exception e)
    ///     {
    ///         user = Err&lt;User>("db-operation failed, check the exception message: " + e.Message);
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="action">Action to be called with the underlying value in try-catch block when Ok.</param>
    /// <param name="name">Name of the operation/action; to be appended to the error messages if the action throws. Omitting the argument will automatically be filled with the action's expression in the caller side.</param>
    public Res Try(Action<T> action, [CallerArgumentExpression("action")] string name = "")
    {
        if (Err == null && Val != null)
        {
            try
            {
                action(Val);
                return Ok();
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return WithoutVal();
    }


    // try-map
    /// <summary>
    /// Returns the error when IsErr.
    /// Otherwise, tries to map into Ok(<paramref name="map"/>(val)) in a try-catch block and returns the Err if it throws.
    /// <code>
    /// static Res&lt;User> TryGetUser() { .. }
    /// static long PutUserToDbGetId(User user) {
    ///     // method that writes the user to a database table and returns back the auto-generated id/primary-key
    ///     // might fail and throw!
    /// }
    /// 
    /// Res&lt;long> id = TryGetUser().TryMap(PutUserToDbGetId);
    /// // equivalently:
    /// Res&lt;long> id = TryGetUser().TryMap(user => PutUserToDbGetId(user));
    /// // Res&lt;long> id will be:
    /// // - Err(called on Err) when TryGetUser returns Err,
    /// // - Err(relevant error message) when TryGetUser returns Ok(user) but the database transaction throws an exception,
    /// // - Ok(id) when TryGetUser returns Ok(user), the database transaction succeeds and returns the auto-generated id.
    /// 
    /// // it provides a shorthand for the following verbose/unpleasant version:
    /// Opt&lt;User> user = TryGetUser();
    /// Res&lt;long> id;
    /// if (user.IsNone)
    ///     id = Err&lt;long>("no user");
    /// else
    /// {
    ///     try
    ///     {
    ///         id = Ok(PutUserToDb(user.Unwrap()));
    ///     }
    ///     catch (Exception e)
    ///     {
    ///         id = Err&lt;long>("db-operation failed, check the exception message: " + e.Message);
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="map">Function (T -> TOut) to be called in try-catch block to get the result when Ok; will not be called when Err.</param>
    /// <param name="name">Name of the map function/operation; to be appended to the error messages if the function throws. Omitting the argument will automatically be filled with the function's expression in the caller side.</param>
    public Res<TOut> TryMap<TOut>(Func<T, TOut> map, [CallerArgumentExpression("map")] string name = "")
    {
        if (Err == null && Val != null)
        {
            try
            {
                return new(map(Val));
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return new(Err ?? string.Empty, string.Empty, null);
    }
    /// <summary>
    /// (async version) <inheritdoc cref="TryMap{TOut}(Func{T, TOut}, string)"/>
    /// </summary>
    /// <param name="map">Function (T -> TOut) to be called in try-catch block to get the result when Ok; will not be called when Err.</param>
    /// <param name="name">Name of the map function/operation; to be appended to the error messages if the function throws. Omitting the argument will automatically be filled with the function's expression in the caller side.</param>
    public async Task<Res<TOut>> TryMapAsync<TOut>(Func<T, Task<TOut>> map, [CallerArgumentExpression("map")] string name = "")
    {
        if (Err == null && Val != null)
        {
            try
            {
                return new(await map(Val));
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return new(Err ?? string.Empty, string.Empty, null);
    }


    // map - err
    /// <summary>
    /// Converts the result to Err&lt;TOut> regardless of state of this result:
    /// <list type="bullet">
    /// <item>The error message will be carried on when this is of Err variant,</item>
    /// <item>A generic error message will be created otherwise.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TOut">Enclosed type of the target result.</typeparam>
    /// <returns></returns>
    public Res<TOut> ToErrOf<TOut>()
    {
        if (Err == null)
            return new($"{nameof(ToErrOf)} is called on '{ToString}'.", "", null);
        else
            return new(Err, "", null);
    }


    // logical combinations
    /// <summary>
    /// Combines two results: this and <paramref name="other"/> as follows:
    /// <list type="bullet">
    /// <item>returns this if both are Ok;</item>
    /// <item>returns the error if one of the results is an Err;</item>
    /// <item>returns the combined error if both results are Err.</item>
    /// </list>
    /// 
    /// <code>
    /// var combined = Ok(12).And(Ok());
    /// Assert.Equal(Ok(12), combined);
    /// 
    /// combined = Ok(12).And(Err("failure"));
    /// Assert.True(combined.IsErr);
    /// 
    /// combined = Err&lt;int>("error").And(Ok());
    /// Assert.True(combined.IsErr);
    /// 
    /// combined = Err&lt;int>("error").And(Err("failure"));
    /// Assert.True(combined.IsErr);
    /// </code>
    /// </summary>
    /// <param name="other">Other result to combine with.</param>
    /// <returns></returns>
    public Res<T> And(Res other)
    {
        if (Err == null && Val != null)
        {
            if (other.IsOk)
                return this;
            else
            {
                var t = this;
                return other.Map(() => t).Flatten();
            }
        }
        else
        {
            if (other.IsOk)
                return this;
            else
                return Err<T>(Err + Environment.NewLine + other.Err);
        }
    }
    /// <summary>
    /// <inheritdoc cref="And(Res)"/>
    /// </summary>
    /// <param name="lazyOther">Other result to combine with; lazily evaluated only if this is Ok.</param>
    /// <returns></returns>
    public Res<T> And(Func<Res> lazyOther)
    {
        if (Err != null)
            return this;
        else
            return And(lazyOther());
    }
    /// <summary>
    /// Combines two results: this and <paramref name="other"/> as follows:
    /// <list type="bullet">
    /// <item>returns Ok of a tuple of both values if both results are Ok;</item>
    /// <item>returns the error if one of the results is an Err;</item>
    /// <item>returns the combined error if both results are Err.</item>
    /// </list>
    /// 
    /// <code>
    /// var combined = Ok(12).And(Ok(true));
    /// Assert.Equal(Ok((12, true)), combined);
    /// 
    /// combined = Ok(12).And(Err&lt;bool>("failure"));
    /// Assert.True(combined.IsErr);
    /// 
    /// combined = Err&lt;int>("error").And(Ok(true));
    /// Assert.True(combined.IsErr);
    /// 
    /// combined = Err&lt;int>("error").And(Err&lt;bool>("failure"));
    /// Assert.True(combined.IsErr);
    /// </code>
    /// </summary>
    /// <param name="other">Other result to combine with.</param>
    /// <returns></returns>
    public Res<(T, T2)> And<T2>(Res<T2> other)
    {

        if (Err == null && Val != null)
        {
            if (other.IsOk && other.Val != null)
                return Ok((Val, other.Val));
            else
            {
                var val = Val;
                return other.Map(x => (val, x));
            }
        }
        else
        {
            if (other.IsOk)
            {
                return new(Err ?? string.Empty, string.Empty, null);
            }
            else
                return new(Err + Environment.NewLine + other.Err, string.Empty, null);
        }
    }
    /// <summary>
    /// <inheritdoc cref="And{T2}(Res{T2})"/>
    /// </summary>
    /// <param name="lazyOther">Other result to combine with; lazily evaluated only if this is Ok.</param>
    /// <returns></returns>
    public Res<(T, T2)> And<T2>(Func<Res<T2>> lazyOther)
    {
        if (Err != null)
            return Err<(T, T2)>(Err);
        else
            return And(lazyOther());
    }


    // into-opt
    /// <summary>
    /// Converts the result type into the option type as follows:
    /// <list type="bullet">
    /// <item>Ok(value) => Some(value),</item>
    /// <item>Err(error) => None, ignoring the error message.</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public Opt<T> IntoOpt()
        => Some(Val);


    // compose
    /// <summary>
    /// Simply returns Ok&lt;T> function: val => Ok(val).
    /// Useful for composing functions of Res&lt;T> type.
    /// </summary>
    /// <returns></returns>
    public static Func<T, Res<T>> Pure() => Ok;


    // operators
    /// <summary>
    /// <para>Limited ahd experimental for now; waiting for generics in operator overloading to be actually useful.</para>
    /// <inheritdoc cref="Map{TOut}(Func{T, TOut})"/>
    /// </summary>
    /// <param name="result">Result to map.</param>
    /// <param name="map">Function to map the result if it is of Ok variant.</param>
    /// <returns></returns>
    public static Res<T> operator /(Res<T> result, Func<T, T> map)
        => result.Map(map);
    /// <summary>
    /// <para>Limited ahd experimental for now; waiting for generics in operator overloading to be actually useful.</para>
    /// <inheritdoc cref="FlatMap{TOut}(Func{T, Res{TOut}})"/>
    /// </summary>
    /// <param name="result">Result to flat-map.</param>
    /// <param name="map">Function to flat-map the result if it is of Ok variant.</param>
    /// <returns></returns>
    public static Res<T> operator |(Res<T> result, Func<T, Res<T>> map)
        => result.FlatMap(map);
    /// <summary>
    /// <para>Limited ahd experimental for now; waiting for generics in operator overloading to be actually useful.</para>
    /// <inheritdoc cref="FlatMap(Func{T, Res})"/>
    /// </summary>
    /// <param name="result">Result to flat-map.</param>
    /// <param name="map">Function to flat-map the result if it is of Ok variant.</param>
    /// <returns></returns>
    public static Res operator |(Res<T> result, Func<T, Res> map)
        => result.FlatMap(map);


    // common
    /// <summary>
    /// String representation.
    /// </summary>
    public override string ToString()
        => Err ?? string.Format("Ok({0})", Val);
    /// <summary>
    /// Returns whether this result is equal to the <paramref name="other"/>.
    /// </summary>
    /// <param name="other">Other result to compare to.</param>
    public override bool Equals(object? other)
        => other != null && (other is Opt<T>) && (Equals(other));
    /// <summary>
    /// Returns true if both values are <see cref="IsOk"/> and their unwrapped values are equal; false otherwise.
    /// </summary>
    /// <param name="other">Other result to compare to.</param>
    public bool Equals(Res<T> other)
    {
        if (IsOk)
        {
            if (other.IsOk)
            {
                if (Val != null)
                    return Val.Equals(other.Val);
                else
                    throw Exc.MustNotReach;
            }
            else
                return false;
        }
        else
        {
            if (other.IsOk)
                return false;
            else
            {
                if (Err != null)
                    return Err.Equals(other.Err);
                else
                    throw Exc.MustNotReach;
            }
        }
    }
    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    public override int GetHashCode()
        => Val == null ? int.MinValue : Val.GetHashCode();
    /// <summary>
    /// Returns true if both results are <see cref="IsOk"/> and their unwrapped values are equal; false otherwise.
    /// <code>
    /// AssertEqual(Err&lt;int>() == Err&lt;int>(), false);
    /// AssertEqual(Err&lt;int>() == Ok(42), false);
    /// AssertEqual(Ok(42) == Err&lt;int>(), false);
    /// AssertEqual(Ok(42) == Ok(7), false);
    /// AssertEqual(Ok(42) == Ok(42), true);
    /// </code>
    /// </summary>
    /// <param name="left">Lhs of the equality operator.</param>
    /// <param name="right">Rhs of the equality operator.</param>
    public static bool operator ==(Res<T> left, Res<T> right)
        => left.Equals(right);
    /// <summary>
    /// Returns true if either value is <see cref="IsErr"/> or their unwrapped values are not equal; false otherwise.
    /// <code>
    /// AssertEqual(Err&lt;int>() != Err&lt;int>(), true);
    /// AssertEqual(Err&lt;int>() != Ok(42), true);
    /// AssertEqual(Ok(42) != Err&lt;int>(), true);
    /// AssertEqual(Ok(42) != Ok(7), true);
    /// AssertEqual(Ok(42) != Ok(42), false);
    /// </code>
    /// </summary>
    /// <param name="left">Lhs of the inequality operator.</param>
    /// <param name="right">Rhs of the inequality operator.</param>
    public static bool operator !=(Res<T> left, Res<T> right)
        => !(left.Equals(right));
}
