namespace Orx.Fun.Result.Tests;

public class ResTests
{
    record RefInt(int Value);
    record User(string UserName);


    [Fact]
    public void DefaultCtors()
    {
        // instead use static ctors Ok() and Err(e)
        Res res = default;
        Assert.True(res.IsOk);
        Assert.True(res.ErrorMessage().IsNone);

        res = new Res();
        Assert.True(res.IsOk);
        Assert.True(res.ErrorMessage().IsNone);
    }

    [Fact]
    public void Ctors()
    {
        ErrConfig.AddStackTraceToErr = true;

        Res res = Ok();
        Assert.True(res.IsOk);
        Assert.True(res.ErrorMessage().IsNone);

        res = Err("sth went wrong");
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(Ctors), res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(Xunit), res.ErrorMessage().Unwrap()); // due to: ErrConfig.AddStackTraceToErr = true;

        ErrConfig.AddStackTraceToErr = false;

        res = Err("sth went wrong");
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(Ctors), res.ErrorMessage().Unwrap());
        Assert.DoesNotContain(nameof(Xunit), res.ErrorMessage().Unwrap()); // since: ErrConfig.AddStackTraceToErr = false;

        // we usually don't need to add when; it will be filled by the Try.. expression or the method causing the Err as above.
        res = Err("sth went wrong", when: "while trying dangerous things");
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains("while trying dangerous things", res.ErrorMessage().Unwrap());
        Assert.DoesNotContain(nameof(Ctors), res.ErrorMessage().Unwrap()); // when overwrites the method where Err is created

        // we usually don't need this; Try.. methods generate errors from exceptions
        res = Err("sth went wrong", exception: new NullReferenceException("bad bad reference"));
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(NullReferenceException), res.ErrorMessage().Unwrap());
        Assert.Contains("bad bad reference", res.ErrorMessage().Unwrap()); // exception message comes in
    }

    [Fact]
    public void EqualityChecks()
    {
        Assert.Equal(Ok(), Ok());
        Assert.NotEqual(Ok(), Err("problems"));

        Assert.Equal(Err("problems"), Err("problems"));
        Assert.NotEqual(Err("problems"), Err("different problems"));
    }

    [Fact]
    public void ThrowIfErr()
    {
        // just a shorthand for:
        // if (res.IsErr)
        //     throw new NullReferenceException(res.ErrorMessage.Unwrap());

        var res = Ok();
        bool _isOkay = res.ThrowIfErr().IsOk; // no exception thrown

        // why does it throw NullReferenceException?
        // - not to introduce yet another exception type
        // - but see below to throw the exception you want
        res = Err("not okay");
        Assert.Throws<NullReferenceException>(() => res.ThrowIfErr().IsOk);

        res = Err("radius is zero; can't divide by zero");
        Assert.Throws<DivideByZeroException>(
            () => res.ThrowIfErr<DivideByZeroException>(err => new(err)));
    }

    [Fact]
    public void Match()
    {
        Res res = Ok();
        string status = res.Match("200", err => $"400 - {err}");
        Assert.Equal("200", status);

        res = Err("bad request");
        status = res.Match("200", err => $"400 - {err}");
        Assert.Contains("400", status);
        Assert.Contains("bad request", status);
    }

    [Fact]
    public void MatchDo()
    {
        // ...Do for the side effect!
        // bad example for testing.
        // for instance, might be handy to log an error when Err, or log info when Ok.
        string status = string.Empty;
        Res res = Ok();
        res.MatchDo(
            whenOk: () => status = "200",
            whenErr: err => status = "400 - " + err);
        Assert.Equal("200", status);

        // similarly the following individual variants exist
        res = Err("bad request");
        res.Do(() => status = "200");
        res.DoIfErr(err => status = "400 - " + err);
        Assert.Contains("400", status);
        Assert.Contains("bad request", status);
    }

    [Fact]
    public void Map()
    {
        // Map converts Res into Res<T> wrt given T creating lambda

        static Res GetDbStatus(int instanceId)
            => instanceId == 0 ? Err("unhealthy") : Ok();
        static User GetDefaultUser()
            => new("Guest");

        var user = GetDbStatus(100).Map(GetDefaultUser);
        Assert.Equal(Ok(new User("Guest")), user);

        var userFailed = GetDbStatus(0).Map(GetDefaultUser);
        Assert.True(userFailed.IsErr);
    }

    [Fact]
    public void TryMap()
    {
        // Map converts Res into Res<T> wrt given T creating lambda

        static Res GetDbStatus(int instanceId)
            => instanceId == 0 ? Err("unhealthy") : Ok();
        static User GetUserCanThrow(string username)
        {
            if (username == "notinvited")
                throw new ArgumentException("not allowed");
            else
                return new(username);
        }

        // we might use TryMap to handle the exception
        Assert.Throws<ArgumentException>(() => GetDbStatus(100).Map(() => GetUserCanThrow("notinvited")));

        // happy path
        var user = GetDbStatus(100).TryMap(() => GetUserCanThrow("Gandalf"));
        Assert.Equal(Ok(new User("Gandalf")), user);

        // failed before TryMap
        var userFailure1 = GetDbStatus(0).TryMap(() => "Gandalf");
        Assert.True(userFailure1.IsErr);

        // failed within TryMap call
        var userFailure2 = GetDbStatus(1000).TryMap(() => "notinvited");
        Assert.True(userFailure1.IsErr);
    }

    [Fact]
    public void FlowWithFlatMap()
    {
        // disclaimer: better to use Res<T> to avoid dependency to the outside the scope of the functions

        // task:
        // we'll perform a series of fallable operations.
        // we need an Err if any of them fails.
        // we'll also perform a call to a method that does nothing but side effects, if the result was Ok so far.

        RefInt number = new(0);

        Res RiskyFun1()
            => OkIf(number.Value >= 0, "expected nonnegative, but got " + number.Value);
        Res RiskyFun2()
            => OkIf(number.Value % 2 == 1); // the expression will be used as the failed validation in the error message
        void DoSomeBadSideEffectsIfOk() { }
        Res NotReallyRisky()
            => Ok();

        void ValidateGetResult(Func<Res> getResult)
        {
            number = new(-1);
            Assert.True(getResult().IsErr);

            number = new(2);
            Assert.True(getResult().IsErr);

            number = new(3);
            Assert.True(getResult().IsOk);
        }

        // with Res.FlatMap
        var getResultOnRes = () => Ok()
            .FlatMap(() => RiskyFun1())
            .FlatMap(() => RiskyFun2())
            .Do(DoSomeBadSideEffectsIfOk)
            .FlatMap(() => NotReallyRisky());
        ValidateGetResult(getResultOnRes);

        // pointfree
        var riskyFun1 = RiskyFun1;
        var getResultTacit =
            riskyFun1
            .FlatMap(RiskyFun2)
            .Do(DoSomeBadSideEffectsIfOk)
            .FlatMap(NotReallyRisky);
        ValidateGetResult(getResultTacit);
    }

    [Fact]
    public void Try()
    {
        // performs a risky operation within a try-catch block
        // * returns Ok if everything worked fine
        // * returns the Err created from the exception otherwise

        // setup
        // as author of the method, we would ideally convert this method to "Res SaveUser(User user)" which does not throw.
        // but assume this is an external library.
        static void SaveUser(User user)
        {
            if (user.UserName.Length > 256)
                throw new ArgumentException("username is too long to be stored");
            // else, write the user nicely to the database
        }

        Res res = Ok().Try(() => SaveUser(new User("vandetta")));
        Assert.True(res.IsOk);

        res = Ok().Try(() => SaveUser(new User(new string('x', 257))));
        Assert.True(res.IsErr);
        Assert.Contains("too long", res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(ArgumentException), res.ErrorMessage().Unwrap());
    }

    [Fact]
    public void ErrOf()
    {
        // maps Res to Err<TOut>, independent of whether the original res is Ok or Err.

        var resultOk = Ok();
        var resultInt = resultOk.ToErrOf<int>();
        Assert.True(resultInt.IsErr);

        var resultErr = Err("parsing error");
        var resultBool = resultErr.ToErrOf<bool>();
        Assert.True(resultBool.IsErr);
        Assert.Contains("parsing error", resultBool.ErrorMessage().Unwrap());
    }
}
