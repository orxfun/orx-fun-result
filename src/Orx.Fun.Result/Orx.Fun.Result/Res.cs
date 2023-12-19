using System.Runtime.CompilerServices;

namespace Orx.Fun.Result;

/// <summary>
/// Valueless result type which can be either of the two variants: Ok or Err(error-message).
/// </summary>
public readonly partial struct Res
{
    // data
    internal readonly string? Err;


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
    /// Result type which can either be Ok or Err.
    /// Parameterless ctor returns Ok; better use <see cref="ResultExtensions.Ok"/> or <see cref="ResultExtensions.Err(string)"/> to construct results by adding `using static OptRes.Ext`.
    /// </summary>
    public Res()
    {
        Err = default;
    }
    internal Res(string msg, string when, Exception? e)
        => Err = ErrConfig.GetErrorString((msg, when, e));


    // throw
    /// <summary>
    /// Returns the result back when <see cref="IsOk"/>; throws NullReferenceException when <see cref="IsErr"/>.
    /// <code>
    /// static Res MakeApiCall() {
    ///     // method that makes an api call.
    ///     // might fail; hence, returns a Res rather than void.
    /// }
    /// var result = MakeApiCall().ThrowIfErr();
    /// // result will be:
    /// // - Ok() if MakeApiCall succeeds and returns Ok;
    /// // - the application will throw a NullReferenceException otherwise.
    /// </code>
    /// </summary>
    public Res ThrowIfErr()
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
    public Res ThrowIfErr<E>(Func<string, E> errorMessageToException) where E : Exception
    {
        if (Err != null)
            throw errorMessageToException(Err);
        else
            return this;
    }


    // okif
    /// <summary>
    /// Returns back the Err if this is Err.
    /// Otherwise, returns Ok if <paramref name="okCondition"/> holds; Err if it does not hold.
    /// Especially useful in fluent input validation.
    /// <code>
    /// static Res&lt;User> Login(string username, string passwordHash)
    /// {
    ///     return OkIf(!string.IsNullOrEmpty(username))    // validate username
    ///         .OkIf(!string.IsNullOrEmpty(passwordHash))  // validate password-hash
    ///         .OkIf(userRepo.ContainsKey(username))       // further validate user
    ///         .Map(() => GetUser(username, password));    // finally map into actual result;
    ///                                                     // any Err in validation steps will directly be mapped to Err, avoiding GetUser call.
    /// }
    /// </code>
    /// </summary>
    /// <param name="okCondition">Condition that should hold to get an Ok, rather than Err.</param>
    /// <param name="strOkCondition">Name of the condition; to be appended to the error message if it does not hold. Omitting the argument will automatically be filled with the condition's expression in the caller side.</param>
    public Res OkIf(bool okCondition, [CallerArgumentExpression("okCondition")] string strOkCondition = "")
        => IsErr ? this : (okCondition ? default : new(strOkCondition, "failed OkIf validation", null));
    /// <summary>
    /// <para>Lazy counterpart of <see cref="OkIf(bool, string)"/> where condition is evaluated only if this is Ok.</para>
    /// 
    /// Returns back the Err if this is Err.
    /// Otherwise, returns Ok if <paramref name="lazyOkCondition"/> holds; Err if it does not hold.
    /// Especially useful in fluent input validation.
    /// <code>
    /// static Res&lt;User> Login(string username, string passwordHash)
    /// {
    ///     return OkIf(!string.IsNullOrEmpty(username))    // validate username
    ///         .OkIf(!string.IsNullOrEmpty(passwordHash))  // validate password-hash
    ///         .OkIf(() => userRepo.ContainsKey(username)) // further validate user; assume this is an expensive call, so we prefer the lazy variant
    ///         .Map(() => GetUser(username, password));    // finally map into actual result;
    ///                                                     // any Err in validation steps will directly be mapped to Err, avoiding GetUser call.
    /// }
    /// </code>
    /// </summary>
    /// <param name="lazyOkCondition">Condition that should hold to get an Ok, rather than Err.</param>
    /// <param name="strOkCondition">Name of the condition; to be appended to the error message if it does not hold. Omitting the argument will automatically be filled with the condition's expression in the caller side.</param>
    public Res OkIf(Func<bool> lazyOkCondition, [CallerArgumentExpression("lazyOkCondition")] string strOkCondition = "")
        => IsErr ? this : (lazyOkCondition() ? default : new(strOkCondition, "failed OkIf validation", null));


    // unwrap
    /// <summary>
    /// Returns Some(error-message) if IsErr; None&lt;string>() if IsOk.
    /// <code>
    /// var result = Err("failed to connect");
    /// Assert(result.ErrorMessage() == Some("failed to connect"));
    /// </code>
    /// </summary>
    public Opt<string> ErrorMessage()
        => Err == null ? None<string>() : Some(Err);


    // match
    /// <summary>
    /// Maps into <paramref name="whenOk"/> whenever IsOk; and into <paramref name="whenErr"/>(errorMessage) otherwise.
    /// <code>
    /// Res&lt;User> user = TryGetUser(..);
    /// string greetingMessage = user.Match("Welcome", err => $"Error getting the user: {err}");
    /// </code>
    /// </summary>
    /// <param name="whenOk">Return value when IsOk.</param>
    /// <param name="whenErr">Map function (string -> TOut) to be called to get the return value when IsErr.</param>
    public TOut Match<TOut>(TOut whenOk, Func<string, TOut> whenErr)
        => IsOk ? whenOk : whenErr(ToString());
    /// <summary>
    /// Maps into <paramref name="whenOk"/>() lazily whenever IsOk; and into <paramref name="whenErr"/>(errorMessage) otherwise.
    /// Similar to <see cref="Match{TOut}(TOut, Func{string, TOut})"/> except that whenOk is lazy and evaluated only when IsOk.
    /// </summary>
    /// <param name="whenOk">Function (() -> TOut) to be called lazily to get the return value when IsOk.</param>
    /// <param name="whenErr">Map function (string -> TOut) to be called to get the return value when IsErr.</param>
    public TOut Match<TOut>(Func<TOut> whenOk, Func<string, TOut> whenErr)
        => IsOk ? whenOk() : whenErr(ToString());
    /// <summary>
    /// (async version) <inheritdoc cref="Match{TOut}(Func{TOut}, Func{string, TOut})"/>
    /// </summary>
    /// <param name="whenOk">Function (() -> TOut) to be called lazily to get the return value when IsOk.</param>
    /// <param name="whenErr">Map function (string -> TOut) to be called to get the return value when IsErr.</param>
    public Task<TOut> MatchAsync<TOut>(Func<Task<TOut>> whenOk, Func<string, Task<TOut>> whenErr)
        => IsOk ? whenOk() : whenErr(ToString());
    /// <summary>
    /// Executes <paramref name="whenOk"/>() whenever IsOk; and <paramref name="whenErr"/>(errorMessage) otherwise.
    /// <code>
    /// Res&lt;User> user = TryGetUser(..);
    /// user.MatchDo
    /// (
    ///     whenOk: () => Log.Info("New user login"),
    ///     whenErr: err => Log.Error($"Failed login. {err}")
    /// );
    /// </code>
    /// </summary>
    public void MatchDo(Action whenOk, Action<string> whenErr)
    {
        if (IsOk)
            whenOk();
        else
            whenErr(ToString());
    }


    // do
    /// <summary>
    /// Runs <paramref name="action"/>() only if IsOk; and returns itself back.
    /// <code>
    /// User user = CreateUser(/*inputs*/);
    /// Res result = TryPutUserToDb(user).Do(Log.Success("user created"));
    /// // result will be:
    /// // - Ok if TryPutUserToDb succeeds and returns Ok; in this case the success message will be logged; or
    /// // - Err if TryPutUserToDb returns Err; and the success message log will not be called.
    /// </code>
    /// </summary>
    /// <param name="action">Action to be executed only if this IsOk.</param>
    public Res Do(Action action)
    {
        if (IsOk)
            action();
        return this;
    }


    // do-if-err
    /// <summary>
    /// Runs <paramref name="action"/>() only if IsErr; and returns itself back.
    /// This is the counterpart of the <see cref="Do(Action)"/> method.
    /// <code>
    /// Res result = RefreshIndices(/*inputs*/).DoIfErr(() => Alert("database connection failed"));
    /// // result will be:
    /// // - Ok if refreshing db indices succeeded;
    /// // - Err if it failed, in which case the Alert call will be made.
    /// </code>
    /// </summary>
    /// <param name="action">Action to be executed only if this IsErr.</param>
    public Res DoIfErr(Action action)
    {
        if (Err != null)
            action();
        return this;
    }
    /// <summary>
    /// Runs <paramref name="action"/>(error-message) only if IsErr; and returns itself back.
    /// This is the counterpart of the <see cref="Do(Action)"/> method.
    /// <code>
    /// Res result = RefreshIndices(/*inputs*/).DoIfErr(err =>  Alert($"database connection failed: {err}"));
    /// // result will be:
    /// // - Ok if refreshing db indices succeeded;
    /// // - Err if it failed, in which case the Alert call will be made.
    /// </code>
    /// </summary>
    /// <param name="action">Action to be executed only if this IsErr.</param>
    public Res DoIfErr(Action<string> action)
    {
        if (Err != null)
            action(ToString());
        return this;
    }


    // map
    /// <summary>
    /// Returns the error when IsErr; Ok(<paramref name="map"/>()) when IsOk.
    /// <code>
    /// Res ValidateInputs(Inputs inputs) { /*checks*/ }
    /// Output CalcOutput(Inputs inputs) { /*maps inputs to output*/ }
    /// 
    /// Inputs inputs = GetInputs();
    /// Res&lt;Output> output = ValidateInputs(inputs).Map(() => CalcOutput(inputs));
    /// // output will be:
    /// // - Err if ValidateInputs returns Err omitting the call to CalcOutput;
    /// // - Ok(CalcOutput(inputs)) if ValidateInputs returns Ok.
    /// </code>
    /// </summary>
    /// <param name="map">Map function that will be called lazily to get the return value only if this IsOk.</param>
    public Res<TOut> Map<TOut>(Func<TOut> map)
        => Err == null ? new(map()) : new(Err, string.Empty, null);
    /// <summary>
    /// (async version) <inheritdoc cref="Map{TOut}(Func{TOut})"/>
    /// </summary>
    /// <param name="map">Map function that will be called lazily to get the return value only if this IsOk.</param>
    public async Task<Res<TOut>> MapAsync<TOut>(Func<Task<TOut>> map)
        => Err == null ? new(await map()) : new(Err, string.Empty, null);


    // flatmap
    /// <summary>
    /// Returns the error when IsErr; <paramref name="map"/>() when IsOk, flattenning the result.
    /// This is a shorthand for sequential Map and Flatten calls.
    /// <code>
    /// // assume we have two methods that can fail; hence returns a Res:
    /// static Res TryRunRiskyOperation() { .. }
    /// static Res TryLogCompletion() { .. }
    /// 
    /// // we want to call both operations; but the second one only if the first one succeeds.
    /// Res result = TryRunRiskyOperation().FlatMap(TryLogCompletion);
    /// // alternatively:
    /// Res result = TryRunRiskyOperation().FlatMap(() => TryLogCompletion());
    /// 
    /// // this is equivalent to:
    /// Res result = TryRunRiskyOperation().Map(() => TryLogCompletion()/*Res&lt;Res>*/).Flatten()/*Res*/;
    /// </code>
    /// </summary>
    /// <param name="map">Map function to be called lazily to get the final result only if this IsOk.</param>
    public Res FlatMap(Func<Res> map)
        => Err == null ? map() : new(Err, string.Empty, null);
    /// <summary>
    /// (async version) <inheritdoc cref="FlatMap(Func{Res})"/>
    /// </summary>
    /// <param name="map">Map function to be called lazily to get the final result only if this IsOk.</param>
    public Task<Res> FlatMapAsync(Func<Task<Res>> map)
        => Err == null ? map() : Task.FromResult(new Res(Err, string.Empty, null));
    /// <summary>
    /// Returns the error when IsErr; <paramref name="map"/>() when IsOk, flattenning the result.
    /// This is a shorthand for sequential Map and Flatten calls.
    /// <code>
    /// // assume we have two methods that can fail; hence returns a Res:
    /// static Res TryRunRiskyOperation() { .. }
    /// static Res&lt;int> TryGetCount() { .. }
    /// 
    /// // we want to call both operations; but the second one only if the first one succeeds.
    /// Res result = TryRunRiskyOperation().FlatMap(TryGetCount);
    /// // alternatively:
    /// Res result = TryRunRiskyOperation().FlatMap(() => TryGetCount());
    /// 
    /// // this is equivalent to:
    /// Res result = TryRunRiskyOperation().Map(() => TryGetCount()/*Res&lt;Res&lt;int>>*/).Flatten()/*Res&lt;int>*/;
    /// </code>
    /// </summary>
    /// <param name="map">Map function to be called lazily to get the final result only if this IsOk.</param>
    public Res<TOut> FlatMap<TOut>(Func<Res<TOut>> map)
        => Err == null ? map() : new(Err, string.Empty, null);
    /// <summary>
    /// (async version) <inheritdoc cref="FlatMap{TOut}(Func{Res{TOut}})"/>
    /// </summary>
    /// <param name="map">Map function to be called lazily to get the final result only if this IsOk.</param>
    public Task<Res<TOut>> FlatMapAsync<TOut>(Func<Task<Res<TOut>>> map)
        => Err == null ? map() : Task.FromResult(new Res<TOut>(Err, string.Empty, null));


    // try
    /// <summary>
    /// When IsOk executes <paramref name="action"/>() in a try-catch block: returns back itself if the process succeeds; Err if it throws.
    /// Does not do anything and returns back itself when IsErr.
    /// <code>
    /// static Res TryLogin() { .. }
    /// static void ClearSessionHistory() { /*risky function, might throw!*/ }
    /// 
    /// var result = TryLogin().Try(ClearSessionHistory);
    /// // the result will be:
    /// // - Err if TryLogin returns Err, in which case ClearSessionHistory is never called;
    /// // - Ok if TryLogin returns Ok; and ClearSessionHistory call succeeds without an exception;
    /// // - Err if TryLogin returns Ok, but ClearSessionHistory throws an exception.
    /// </code>
    /// </summary>
    /// <param name="action">Action to be executed in a try-catcy block only if this IsOk.</param>
    /// <param name="name">Name of the map action; to be appended to the error messages if the action throws. Omitting the argument will automatically be filled with the action's expression in the caller side.</param>
    public Res Try(Action action, [CallerArgumentExpression("action")] string name = "")
    {
        if (Err == null)
        {
            try
            {
                action();
                return this;
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return this;
    }
    /// <summary>
    /// (async version) <inheritdoc cref="Try(Action, string)"/>
    /// </summary>
    /// <param name="action">Action to be executed in a try-catcy block only if this IsOk.</param>
    /// <param name="name">Name of the map action; to be appended to the error messages if the action throws. Omitting the argument will automatically be filled with the action's expression in the caller side.</param>
    public async Task<Res> TryAsync(Func<Task> action, [CallerArgumentExpression("action")] string name = "")
    {
        if (Err == null)
        {
            try
            {
                await action();
                return this;
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return this;
    }


    // try-map
    /// <summary>
    /// Returns the error when IsErr.
    /// Otherwise, tries to map into Ok(<paramref name="map"/>()) in a try-catch block and returns the Err if it throws.
    /// <code>
    /// static Res ValidateUser(User user) { /*returns Ok if valid; Err o/w*/ }
    /// static Res&lt;Secret> TryGetSecrets(User user) { /*returns Ok(secrets) if succeds; Err if fails to get secrets*/ }
    /// 
    /// User user = GetUser(..);
    /// var secrets = ValidateUser(user).TryMap(() => TryGetSecrets(user));
    /// // TryGetSecrets will be called only if ValidateUser call returns Ok;
    /// // secrets will be Ok of the grabbed secrets if both ValidateUser and TryGetSecrets return Ok.
    /// </code>
    /// </summary>
    /// <param name="map">Map function to be called lazily to create the result only if this IsOk.</param>
    /// <param name="name">Name of the map function; to be appended to the error messages if the function throws. Omitting the argument will automatically be filled with the function's expression in the caller side.</param>
    public Res<TOut> TryMap<TOut>(Func<TOut> map, [CallerArgumentExpression("map")] string name = "")
    {
        if (Err == null)
        {
            try
            {
                return new(map());
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return new(Err, string.Empty, null);
    }
    /// <summary>
    /// (async version) <inheritdoc cref="TryMap{TOut}(Func{TOut}, string)"/>
    /// </summary>
    /// <param name="map">Map function to be called lazily to create the result only if this IsOk.</param>
    /// <param name="name">Name of the map function; to be appended to the error messages if the function throws. Omitting the argument will automatically be filled with the function's expression in the caller side.</param>
    public async Task<Res<TOut>> TryMapAsync<TOut>(Func<Task<TOut>> map, [CallerArgumentExpression("map")] string name = "")
    {
        if (Err == null)
        {
            try
            {
                return new(await map());
            }
            catch (Exception e)
            {
                return new(string.Empty, name, e);
            }
        }
        else
            return new(Err, string.Empty, null);
    }


    // try-flat-map
    /// <summary>
    /// Returns the error when IsErr.
    /// Otherwise, tries to return <paramref name="map"/>(val) in a try-catch block and returns the Err if it throws.
    /// <code>
    /// static Res&lt;User> TryGetUser() { .. }
    /// static Res&lt;long> PutUserToDbGetId(User user) {
    ///     // method that writes the user to a database table and returns back the auto-generated id/primary-key
    ///     // might throw in cases of connection errors!
    /// }
    /// 
    /// Res&lt;long> id = TryGetUser().TryFlatMap(PutUserToDbGetId);
    /// // equivalently:
    /// Res&lt;long> id = TryGetUser().TryFlatMap(user => PutUserToDbGetId(user));
    /// // Res&lt;long> id will be:
    /// // - Err(called on Err) when TryGetUser returns Err,
    /// // - Err(relevant error message) when TryGetUser returns Ok(user) but PutUserToDbGetId returns an Err,
    /// // - Err(relevant error message) when TryGetUser returns Ok(user) but the database transaction throws an exception,
    /// // - Ok(id) when TryGetUser returns Ok(user), the database transaction succeeds and returns the auto-generated id.
    /// </code>
    /// </summary>
    /// <param name="map">Function (() -> Res&lt;TOut>) to be called in try-catch block to get the result when Ok; will not be called when Err.</param>
    /// <param name="name">Name of the map function/operation; to be appended to the error messages if the function throws. Omitting the argument will automatically be filled with the function's expression in the caller side.</param>
    public Res<TOut> TryFlatMap<TOut>(Func<Res<TOut>> map, [CallerArgumentExpression("map")] string name = "")
    {
        if (Err == null)
        {
            try
            {
                return map();
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
    /// (async version) <inheritdoc cref="TryFlatMap{TOut}(Func{Res{TOut}}, string)"/>
    /// </summary>
    /// <param name="map">Function (() -> Res&lt;TOut>) to be called in try-catch block to get the result when Ok; will not be called when Err.</param>
    /// <param name="name">Name of the map function/operation; to be appended to the error messages if the function throws. Omitting the argument will automatically be filled with the function's expression in the caller side.</param>
    public async Task<Res<TOut>> TryFlatMapAsync<TOut>(Func<Task<Res<TOut>>> map, [CallerArgumentExpression("map")] string name = "")
    {
        if (Err == null)
        {
            try
            {
                return await map();
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
    /// <item>returns Ok if both are Ok;</item>
    /// <item>returns the error if one of the results is an Err;</item>
    /// <item>returns the combined error if both results are Err.</item>
    /// </list>
    /// </summary>
    /// <param name="other">Other result to combine with.</param>
    /// <returns></returns>
    public Res And(Res other)
    {
        if (Err == null)
            return other;
        else
        {
            if (other.Err == null)
                return this;
            else
                return new(Err + Environment.NewLine + other.Err, string.Empty, null);
        }
    }
    /// <summary>
    /// Combines two results: this and <paramref name="other"/> as follows:
    /// <list type="bullet">
    /// <item>returns this if this is Ok;</item>
    /// <item>returns <paramref name="other"/> otherwise.</item>
    /// </list>
    /// 
    /// <code>
    /// var or = Ok().Or(Ok());
    /// Assert.True(or.IsOk);
    /// 
    /// or = Ok().Or(Err("error-message"));
    /// Assert.True(or.IsOk);
    /// 
    /// or = Err("error-message").Or(Ok());
    /// Assert.True(or.IsOk);
    /// 
    /// or = Err("error-message").Or(Err("second-error-message"));
    /// Assert.True(or.IsErr);
    /// Assert.Equal(Some("second-error-message"), or.ErrorMessage());
    /// </code>
    /// </summary>
    /// <param name="other">Other result to combine with.</param>
    /// <returns></returns>
    public Res Or(Res other)
        => Err == null ? this : other;
    /// <summary>
    /// (lazy version) <inheritdoc cref="Or(Res)"/>
    /// </summary>
    /// <param name="other">Other result to combine with.</param>
    /// <returns></returns>
    public Res Or(Func<Res> other)
        => Err == null ? this : other();


    // compose
    /// <summary>
    /// Simply returns Ok function: () => Ok().
    /// Useful for composing functions of Res type.
    /// </summary>
    /// <returns></returns>
    public static Func<Res> Pure() => ResultExtensions.Ok;


    // operators
    /// <summary>
    /// <para>Limited ahd experimental for now; waiting for generics in operator overloading to be actually useful.</para>
    /// <inheritdoc cref="FlatMap(Func{Res})"/>
    /// </summary>
    /// <param name="result">Result to flat-map.</param>
    /// <param name="map">Function to flat-map the result if it is of Ok variant.</param>
    /// <returns></returns>
    public static Res operator |(Res result, Func<Res> map)
        => result.FlatMap(map);


    // common
    /// <summary>
    /// String representation.
    /// </summary>
    public override string ToString()
        => Err ?? "Ok";
}
