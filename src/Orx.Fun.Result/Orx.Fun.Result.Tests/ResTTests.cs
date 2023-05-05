namespace Orx.Fun.Result.Tests;

public class ResTTests
{
    record User(string UserName);
    record Role(string RoleName);
    record View(string ViewName);


    [Fact]
    public void DefaultCtors()
    {
        // instead use static ctors Ok(value) and Err(e)
        Res<int> resInt = default;
        Assert.True(resInt.IsOk);
        Assert.Equal(Ok(0), resInt);

        var resChar = new Res<char>();
        Assert.True(resChar.IsOk);
        Assert.Equal(Ok('\0'), resChar);
    }

    [Fact]
    public void Ctors()
    {
        ErrConfig.AddStackTraceToErr = true;

        var res = Ok(12);
        Assert.True(res.IsOk);
        Assert.True(res.ErrorMessage().IsNone);
        Assert.Equal(Ok(12), res);
        Assert.Equal(12, res.Unwrap());

        res = Err<int>("sth went wrong");
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Throws<NullReferenceException>(() => res.Unwrap());
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(Ctors), res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(Xunit), res.ErrorMessage().Unwrap()); // due to: ErrConfig.AddStackTraceToErr = true;

        ErrConfig.AddStackTraceToErr = false;

        res = Err<int>("sth went wrong");
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains(nameof(Ctors), res.ErrorMessage().Unwrap());
        Assert.DoesNotContain(nameof(Xunit), res.ErrorMessage().Unwrap()); // since: ErrConfig.AddStackTraceToErr = false;

        // we usually don't need to add when; it will be filled by the Try.. expression or the method causing the Err as above.
        res = Err<int>("sth went wrong", when: "while trying dangerous things");
        Assert.True(res.IsErr);
        Assert.True(res.ErrorMessage().IsSome);
        Assert.Contains("sth went wrong", res.ErrorMessage().Unwrap());
        Assert.Contains("while trying dangerous things", res.ErrorMessage().Unwrap());
        Assert.DoesNotContain(nameof(Ctors), res.ErrorMessage().Unwrap()); // when overwrites the method where Err is created
    }

    [Fact]
    public void OkIfCtor()
    {
        // chained validation
        // we want a number to be Ok if
        // * it is nonnegative
        // * even
        // * and divisible by 3

        static void AssertValidator(Func<int, Res<int>> validator)
        {
            var res = validator(-1);
            Assert.True(res.IsErr);

            res = validator(3);
            Assert.True(res.IsErr);

            res = validator(4);
            Assert.True(res.IsErr);

            res = validator(6);
            Assert.Equal(Ok(6), res);
        }

        static Res<int> ValidateLambdas(int number)
        {
            return Ok(number)
                .OkIf(num => num >= 0)
                .OkIf(num => num % 2 == 0)
                .OkIf(num => num % 3 == 0);
        }
        AssertValidator(ValidateLambdas);

        // composed - lambdas
        var validateLambdasComposed = Res<int>.Pure()
            .OkIf(num => num >= 0)
            .OkIf(num => num % 2 == 0)
            .OkIf(num => num % 3 == 0);
        AssertValidator(validateLambdasComposed);

        // pointfree
        static bool IsNonnegative(int num) => num >= 0;
        static bool IsEven(int num) => num % 2 == 0;
        static bool IsDivisableBy3(int num) => num % 3 == 0;
        var validatePointfree = Res<int>.Pure()
            .OkIf(IsNonnegative)
            .OkIf(IsEven)
            .OkIf(IsDivisableBy3);
        AssertValidator(validatePointfree);
    }

    [Fact]
    public void OkIfNotnullCtor()
    {
        // consider an external method which can return a nullable of T?
        // say the method returns 'value', then
        // value.OkIfNotnull() converts T? into Res<T>
        // which is Err if value happened to be null.

        static string? GetNickName(string name)
            => name == "Akasha" ? "QoP" : null;

        Res<string> nickSome = GetNickName("Akasha").OkIfNotnull();
        Assert.Equal(Ok("QoP"), nickSome);
        Assert.Equal("QoP", nickSome.Unwrap());

        Res<string> nickNone = GetNickName("Tinker").OkIfNotnull();
        Assert.True(nickNone.IsErr);
    }

    [Fact]
    public void NullGuard()
    {
        // Possible null reference return.
#pragma warning disable CS8603
        static string GetNullString() => null;
#pragma warning restore CS8603

        string str = GetNullString();
        Res<string> opt = Ok(str); // null references cannot be Ok(T)
        Assert.True(opt.IsErr);
        Assert.Throws<NullReferenceException>(() => opt.Unwrap());
    }

    [Fact]
    public void EqualityChecks()
    {
        Assert.Equal(Ok(12), Ok(12));
        Assert.NotEqual(Ok(12), Ok(42));

        Assert.NotEqual(Ok(12), Err<int>("problem"));

        Assert.Equal(Err<int>("problem"), Err<int>("problem"));

        Assert.NotEqual(Err<int>("problem"), Err<int>("a different problem"));
    }

    [Fact]
    public void Unwrap()
    {
        Res<int> res = Ok(42);
        int value = res.Unwrap(); // dangerous call! -> the only method that can throw
        Assert.Equal(42, value);

        res = Err<int>("failure");
        Assert.Throws<NullReferenceException>(() => res.Unwrap());
    }

    [Fact]
    public void UnwrapOr()
    {
        Res<int> res = Ok(42);
        int value = res.UnwrapOr(10);
        Assert.Equal(42, value);

        res = Err<int>("failure");
        value = res.UnwrapOr(10);
        Assert.Equal(10, value);

        // lazy version in case computation of fallback value is demanding
        res = Err<int>("failure");
        value = res.UnwrapOr(() => 10);
        Assert.Equal(10, value);
    }

    [Fact]
    public void WithoutVal()
    {
        // sometimes we are only interested in the status, but not the value
        var resInt = Ok(42).Map(x => x / 7);
        Assert.Equal(Ok(6), resInt);

        Res res = resInt.WithoutVal();
        Assert.True(res.IsOk);

        resInt = Ok(42).TryMap(x => x / 0);
        Assert.True(resInt.IsErr);

        res = resInt.WithoutVal();
        Assert.True(res.IsErr);

        Assert.Equal(resInt.ErrorMessage(), res.ErrorMessage());
    }

    [Fact]
    public void ThrowIfErr()
    {
        // just a shorthand for:
        // if (res.IsErr)
        //     throw new NullReferenceException(res.ErrorMessage().Unwrap());

        Res<int> res = Ok(42);
        int value = res.ThrowIfErr().Unwrap();
        Assert.Equal(42, value);

        // why does it throw NullReferenceException?
        // - not to introduce yet another exception type
        // - but see below to throw the exception you want
        res = Err<int>("failure");
        Assert.Throws<NullReferenceException>(() => res.ThrowIfErr());

        res = Err<int>("radius is zero; can't divide by zero");
        Assert.Throws<DivideByZeroException>(
            () => res.ThrowIfErr(err => new DivideByZeroException(err)));

    }

    [Fact]
    public void Match()
    {
        Res<string> name = Ok("Merlin");
        string greeting = name.Match(name => $"Welcome {name}", _ => "Welcome guest");
        // equivalently:
        greeting = name.Match(
            whenOk: name => $"Welcome {name}",
            whenErr: _err => "Welcome guest");
        Assert.Equal("Welcome Merlin", greeting);

        name = Err<string>("cannot find user");
        greeting = name.Match(name => $"Welcome {name}", _ => "Welcome guest");
        Assert.Equal("Welcome guest", greeting);

        // or match none lazily
        greeting = name.Match(name => $"Welcome {name}", _ => "Welcome guest");
        Assert.Equal("Welcome guest", greeting);
    }

    [Fact]
    public void MatchDo()
    {
        // ...Do for the side effect!
        // bad example for testing.
        // for instance, might be handy to log an error when None, or log info when Some.
        string greeting = string.Empty;
        var name = Ok("Merlin");
        name.MatchDo(
            whenOk: name => greeting = $"Welcome {name}",
            whenErr: _err => greeting = "Welcome guest");
        Assert.Equal("Welcome Merlin", greeting);

        // similarly the following individual variants exist
        name.Do(name => greeting = $"Welcome {name}");
        name.DoIfErr(_ => greeting = "Welcome guest");
        Assert.Equal("Welcome Merlin", greeting);
    }

    [Fact]
    public void MapBasic()
    {
        // string -> int
        static int CountLetterA(string name)
            => name.Count(c => c == 'a' || c == 'A');

        var starName = Ok("Antares");
        Res<int> countA = starName.Map(CountLetterA);
        Assert.Equal(countA, Ok(2));

        starName = Err<string>("failure");
        countA = starName.Map(CountLetterA);
        Assert.Equal(countA, Err<int>("failure"));

        // alternatively
        countA = starName.Map(name => CountLetterA(name)); // or
        countA = starName.Map(name => name.Count(c => c == 'a' || c == 'A')); // or
    }

    [Fact]
    public void FlowWithMap()
    {
        // task:
        // * maybe get user by its username
        // * get whether it is authenticated or not
        // * get the role associated with the user if authenticated
        // * get views that the role is authorized to see

        // setup: available functions.
        static Res<User> GetUser(string username)
            => username == "Pixie" ? Ok(new User("Pixie")) : Err<User>("can't fetch user");
        static bool IsAuthenticated(User user)
            => true;
        static Role GetRole(User user, bool isAuthenticated)
            => isAuthenticated ? new Role("Admin") : new Role("Guest");
        static View[] GetAuthorizedViews(Role role)
        {
            if (role.RoleName == "Guest")
                return new View[] { new View("Home") };
            else
                return new View[] { new View("Home"), new View("Account") };
        }

        // just map
        var views = GetUser("Pixie")
            .Map(user => (user, IsAuthenticated(user)))
            .Map(x => GetRole(x.user, x.Item2))
            .Map(GetAuthorizedViews);
        Assert.True(views.IsOk);
        Assert.Equal(views.Unwrap(), new View[] { new("Home"), new("Account") });

        views = GetUser("Witch")
            .Map(user => (user, IsAuthenticated(user)))
            .Map(x => GetRole(x.user, x.Item2))
            .Map(GetAuthorizedViews);
        Assert.True(views.IsErr);

        // just map - local func
        Func<string, Res<View[]>> getViewsForUser = username =>
        {
            return GetUser(username)
                .Map(user => (user, IsAuthenticated(user)))
                .Map(x => GetRole(x.user, x.Item2))
                .Map(GetAuthorizedViews);
        };
        Assert.Equal(getViewsForUser("Pixie").Unwrap(), new View[] { new("Home"), new("Account") });
        Assert.True(getViewsForUser("Witch").IsErr);

        // pointfree version
        var getUser = GetUser;
        var isAuth = IsAuthenticated;
        getViewsForUser = getUser.Map(isAuth.Cached()).Map(GetRole).Map(GetAuthorizedViews);

        Assert.Equal(getViewsForUser("Pixie").Unwrap(), new View[] { new("Home"), new("Account") });
        Assert.True(getViewsForUser("Witch").IsErr);
    }

    [Fact]
    public void FlatMap()
    {
        // setup
        static Res<User> QueryUser(string username)
            => OkIf<User>(username == "jdoe" || username == "foo", () => new User(username));
        static Res<string> GetMiddleName(User user)
            => user.UserName == "jdoe" ? Err<string>("who dis?") : Ok("Middle");

        // much more nested than desired; FlatMap to automatically bypass the None track
        Res<Res<string>> _ = QueryUser("jdoe").Map(GetMiddleName);

        Res<string> nobodyMiddle = QueryUser("nobody").FlatMap(GetMiddleName);
        Assert.True(nobodyMiddle.IsErr);

        Res<string> jdoeMiddle = QueryUser("jdoe").FlatMap(GetMiddleName);
        Assert.True(jdoeMiddle.IsErr);

        Res<string> fooMiddle = QueryUser("foo").FlatMap(GetMiddleName);
        Assert.Equal(Ok("Middle"), fooMiddle);
    }

    [Fact]
    public void Flatten()
    {
        // just a backup method to rescue when we make a naive mistake to end up with Res<Res<T>>.
        // see FlatMap & Compose in above test to avoid this.

        // setup
        static Res<User> QueryUser(string username)
            => OkIf<User>(username == "jdoe" || username == "foo", () => new User(username));
        static Res<string> GetMiddleName(User user)
            => user.UserName == "jdoe" ? Err<string>("who dis?") : Ok("Middle");

        Res<Res<string>> tooNested = QueryUser("nobody").Map(GetMiddleName);
        Res<string> nobodyMiddle = tooNested.Flatten();
        Assert.True(nobodyMiddle.IsErr);

        tooNested = QueryUser("jdoe").Map(GetMiddleName);
        Res<string> jdoeMiddle = tooNested.Flatten();
        Assert.True(jdoeMiddle.IsErr);

        tooNested = QueryUser("foo").Map(GetMiddleName);
        Res<string> fooMiddle = tooNested.Flatten();
        Assert.Equal(Ok("Middle"), fooMiddle);
    }

    [Fact]
    public void FlowWithFlatMap()
    {
        // setup
        static Res<string> GetUsername(string userEntry)
            => OkIf<string>(!string.IsNullOrEmpty(userEntry), userEntry);
        static bool ValidUsername(string userName)
            => userName.Length >= 3;
        static Res<User> QueryUser(string username)
            => OkIf<User>(username == "jdoe" || username == "foo", () => new User(username));
        static Role GetRoleOf(User user)
            => new Role("Admin");

        // tests
        static void TestFun(Func<string, Res<Role>> getRole)
        {
            var leadingToNone = new string[]
            {
                "",     // fails GetUsername's internal check
                "x",    // fails ValidUsername check
                "bar",  // QueryUser returns None
            };
            Assert.True(leadingToNone.Select(entry => getRole(entry)).All(role => role.IsErr));

            var role = getRole("jdoe");
            Assert.Equal(Ok(new Role("Admin")), role);
        }

        // transform using opt methods, almost pointfree
        var getRole = (string userEntry) =>
        {
            return GetUsername(userEntry)
                .OkIf(ValidUsername)
                .FlatMap(QueryUser)
                .Map(GetRoleOf);
        };
        TestFun(getRole);


        // transform using composition method, pointfree
        var getUsername = GetUsername;
        var pntfreeGetRole = getUsername.OkIf(ValidUsername).FlatMap(QueryUser).Map(GetRoleOf);
        TestFun(pntfreeGetRole);
    }

    [Fact]
    public void FlowWithFlatMapAndCached()
    {
        // setup
        static Res<string> GetUsername(string userEntry)
            => OkIf<string>(!string.IsNullOrEmpty(userEntry), userEntry);
        static bool ValidUsername(string userName)
            => userName.Length >= 3;
        static Res<User> QueryUser(string username)
            => OkIf<User>(username == "jdoe" || username == "foo", () => new User(username));
        static Role GetRoleOf(User user)
            => new Role("Admin");
        static string Greeting(User user, Role role)
            => $"{user.UserName}, welcome as {role.RoleName}!";

        // tests
        static void TestFun(Func<string, Res<string>> getGreeting)
        {
            var leadingToNone = new string[]
            {
                "",     // fails GetUsername's internal check
                "x",    // fails ValidUsername check
                "bar",  // QueryUser returns None
            };
            Assert.True(leadingToNone.Select(entry => getGreeting(entry)).All(role => role.IsErr));

            var role = getGreeting("jdoe");
            Assert.Equal(Ok("jdoe, welcome as Admin!"), role);
        }


        // transform using opt methods, almost pointfree
        var getRoleOf = GetRoleOf;
        var getGreeting = (string userEntry) =>
        {
            return GetUsername(userEntry)
                .OkIf(ValidUsername)
                .FlatMap(QueryUser)
                .Map(getRoleOf.Cached())
                .Map(Greeting);
        };
        TestFun(getGreeting);

        // transform using composition method, pointfree
        var getUsername = GetUsername;
        var pntfreeGetGreeting = getUsername
            .OkIf(ValidUsername)
            .FlatMap(QueryUser)
            .Map(getRoleOf.Cached())
            .Map(Greeting);
        TestFun(pntfreeGetGreeting);
    }

    [Fact]
    public void FlowWithTryMap()
    {
        // setup
        // we will call the following external functions on doubles, all of which might throw
        static double Divide(double a, double b)
        {
            if (b == 0.0)
                throw new DivideByZeroException("can't divide by zero");
            else
                return a / b;
        }
        static double Sqrt(double a)
        {
            if (a < 0)
                throw new ArgumentException("cannot handle imaginary numbers");
            else
                return Math.Sqrt(a);
        }

        // unhandled versions
        Assert.Throws<DivideByZeroException>(() => Ok(0.0).Map(x => Divide(1, x)).Map(Sqrt));
        Assert.Throws<ArgumentException>(() => Ok(-10.0).Map(x => Divide(1, x)).Map(Sqrt));
        var _ = Ok(10.0).Map(x => Divide(1, x)).Map(Sqrt);

        // validation
        static void AssertGetResult(Func<double, Res<double>> getResult)
        {
            Assert.True(getResult(0.0).IsErr);
            Assert.True(getResult(-10.0).IsErr);
            Assert.True(getResult(10.0).IsOk);
        }

        // flow using Res<T> methods
        Func<double, Res<double>> getResult = num => Ok(num).TryMap(x => Divide(1, x)).TryMap(Sqrt);
        AssertGetResult(getResult);

        // partially apply divide
        var reciprocal = (double x) => Divide(1, x);
        getResult = num => Ok(num).TryMap(reciprocal).TryMap(Sqrt);
        AssertGetResult(getResult);

        // pointfree
        getResult = Res<double>.Pure().TryMap(reciprocal).TryMap(Sqrt);
        AssertGetResult(getResult);
    }

    [Fact]
    public void LogicalAnd()
    {
        // combine with res
        var combinedWithRes = Ok(12).And(Ok());
        Assert.Equal(Ok(12), combinedWithRes);

        combinedWithRes = Ok(12).And(Err("failure"));
        Assert.True(combinedWithRes.IsErr);
        Assert.Contains("failure", combinedWithRes.ErrorMessage().Unwrap());

        combinedWithRes = Err<int>("error").And(Ok());
        Assert.True(combinedWithRes.IsErr);
        Assert.Contains("error", combinedWithRes.ErrorMessage().Unwrap());

        combinedWithRes = Err<int>("error").And(Err("failure"));
        Assert.True(combinedWithRes.IsErr);
        Assert.Contains("error", combinedWithRes.ErrorMessage().Unwrap());
        Assert.Contains("failure", combinedWithRes.ErrorMessage().Unwrap());

        // combine with res<T> => converting resulting type to a tuple
        var combined = Ok(12).And(Ok(true));
        Assert.Equal(Ok((12, true)), combined);

        combined = Ok(12).And(Err<bool>("failure"));
        Assert.True(combined.IsErr);
        Assert.Contains("failure", combined.ErrorMessage().Unwrap());

        combined = Err<int>("error").And(Ok(true));
        Assert.True(combined.IsErr);
        Assert.Contains("error", combined.ErrorMessage().Unwrap());

        combined = Err<int>("error").And(Err<bool>("failure"));
        Assert.True(combined.IsErr);
        Assert.Contains("error", combined.ErrorMessage().Unwrap());
        Assert.Contains("failure", combined.ErrorMessage().Unwrap());
    }

    [Fact]
    public void ErrOf()
    {
        // maps Res<T> to Err<TOut>, independent of whether the original res is Ok or Err.

        var resultOk = Ok("hello");
        var resultInt = resultOk.ToErrOf<int>();
        Assert.True(resultInt.IsErr);

        var resultErr = Err<string>("parsing error");
        var resultBool = resultErr.ToErrOf<bool>();
        Assert.True(resultBool.IsErr);
        Assert.Contains("parsing error", resultBool.ErrorMessage().Unwrap());
    }
}
