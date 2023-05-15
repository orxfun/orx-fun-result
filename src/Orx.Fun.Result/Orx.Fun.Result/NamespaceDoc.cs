using System.Runtime.CompilerServices;

namespace Orx.Fun.Result;

/// <summary>
/// A simple result type for C#, not minding the error type, instead aiming to be explicit, concise and fluent.
/// <list type="bullet">
/// <item>Res: Ok or Err.</item>
/// <item>Res&lt;T>: Ok(T) or Err.</item>
/// </list>
/// In order to enable the types globally, add the following in project's global usings file:
/// <code>
/// global using Orx.Fun.Result;
/// global using static Orx.Fun.Result.ResultExtensions.
/// </code>
/// </summary>
[CompilerGeneratedAttribute()]
class NamespaceDoc { }
