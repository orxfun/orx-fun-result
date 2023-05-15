# Orx.Fun.Result

A simple result type for C#, not minding the error type, instead aiming to be explicit, concise and fluent.

Complete auto-generated documentation can be found here:
**[sandcastle-documentation](https://orxfun.github.io/orx-fun-result/index.html)**.

## Why?

* Result (either) type is necessary.
    * Explicit result returns
        * we know that the method might fail;
        * however, it will not throw,
        * it will just return the error if it fails.
    * Continuations without overwhelming try-catch blocks



Consider the following login method, for instance.
It can return a valid user only after a series of validation tests are completed.
Here, the result type plays the role of an optional with additional information about the failed validation.

```csharp
static Res<User> Login(string username, string passwordHash)
{
    return OkIf(!string.IsNullOrEmpty(username))    // validate username
        .OkIf(!string.IsNullOrEmpty(passwordHash))  // validate password-hash
        .OkIf(userRepo.ContainsKey(username))       // further validate user
        .Map(() => GetUser(username, password));    // finally map into actual result;
                                                    // any Err in validation steps will directly be mapped to Err, avoiding GetUser call.
}
```

Alternatively, consider operations that has external risks of failure; such as connecting to a database or reading a file. There exist various reasons such operations can lead to exceptions; however, it is not always explicit. Further, try-catch blocks are too large to be reader friendly.

Consider the example below where we need to parse a user from a file and write it to a database. Both of these operations might fail due to external reasons.

```csharp
static User ParseUser(string fileContent)
{
	// assume this is a safe method once the file is read.
    // but 'File.ReadAllText' can throw
}
static void PutUserToDatabase(User user)
{
	// this can throw
}
```

Firstly, the concise but dangerous version:
```csharp
static void ParseAndPutUserDangerous(string filePath)
{
	string fileContent = File.ReadAllText(filePath);	// can throw
    User user = ParseUser(fileContent);
    PutUserToDatabase(user);							// can throw
}
```
The caller must catch the exceptions in order not to crash the app; but the danger is not explicit in the function signature, it just returns a void.

Alternatively, the method itself can try to deal with the errors:

```csharp
static void ParseAndPutUserConfused(string filePath)
{
	try
    {
    	string fileContent = File.ReadAllText(filePath);
        User user = ParseUser(fileContent);
        try
        {
        	PutUserToDatabase(user);
	    }
        catch (Exception dbException)
        {
        	// what do we do with the exception ?
        }
	}
    catch (Exception ioException)
    {
    	// what do we do with the exception ?
	}
}
```
But now, we don't know what to do with the error. Especially, if this method is called from at least two different methods, we cannot or should not decide. Let the caller decide. Also, try-catch blocks cause too much noise.

What we actually want is something like this:

```charp
static Res ParseAndPutUser(string filePath)
{
	return Ok()
    	.TryMap(() => File.ReadAllText(filePath))
        .Map(ParseUser)
        .Try(PutUserToDatabase);
}
```

Note that this is as concise as the dangerous version; however, it cannot throw and it is explicit in the following sense:
* whenever **Try** is used, we know that the internal operation can throw; however, if the exception is thrown it will safely be caught and converted to an error within the Try method;
* **TryMap** method is similar in this sense; however, it returns a value rather than void;
* when we see **Map**, on the other hand, we know that we are just mapping the value and we know that the internal operation does not throw or fail.

You may see that the result type also provides us with the continuations.
In the above example, if either of the "ReadAllText" or "PutUserToDatabase" methods fail; the error will be propagated to the end of the method. Another useful method in this regards is apparently the **FlatMap**.


