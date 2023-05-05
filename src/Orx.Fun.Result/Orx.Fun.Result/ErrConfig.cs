using System.Diagnostics;
using System.Text;

namespace Orx.Fun.Result;

/// <summary>
/// Static configurations for error messages of the Err variant of <see cref="Res"/> or <see cref="Res{T}"/> result types.
/// </summary>
public static class ErrConfig
{
    /// <summary>
    /// When true stack trace will be added to the error messages of Res types in cases of exceptions.
    /// </summary>
    public static bool AddStackTraceToErr { get; set; } = false;
    /// <summary>
    /// Method to convert (message, when, exception) into an error string.
    /// </summary>
    public static Func<(string Message, string When, Exception? Exception), string> GetErrorString { get; set; } = DefGetErrorString;


    // constants
    internal const string ErrParserFailed = "Failed parsing.";


    // helpers
    static readonly HashSet<string> StackSkipMethods = new()
    {
        nameof(DefGetErrorString),
        nameof(GetWhenAsStackTrace),
    };
    static readonly HashSet<string> StackSkipTypes = new()
    {
        $"{nameof(Orx)}.{nameof(Fun)}.{nameof(Result)}.{nameof(Res)}",
        $"{nameof(Orx)}.{nameof(Fun)}.{nameof(Result)}.{nameof(Res)}`1",
        $"{nameof(Orx)}.{nameof(Fun)}.{nameof(Result)}.{nameof(ResultExtensions)}",
    };
    static string GetWhenAsStackTrace(bool withFullStackTrace)
    {
        var sb = new StringBuilder();
        StackTrace trace = new();
        var frames = trace.GetFrames();
        if (withFullStackTrace)
            sb.AppendLine("# Stack Trace");
        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            if (method != null)
            {
                if (StackSkipMethods.Contains(method.Name))
                    continue;

                var declaringType = method.DeclaringType;
                if (declaringType != null && declaringType.FullName != null)
                {
                    if (StackSkipTypes.Contains(declaringType.FullName))
                        continue;
                }

                if (withFullStackTrace)
                    sb.Append("* ");
                sb.Append(declaringType?.FullName)
                    .Append('.')
                    .Append(method.Name);

                if (!withFullStackTrace)
                    break;

                sb.AppendLine();
            }
        }
        return sb.ToString();
    }
    static string DefGetErrorString((string Message, string When, Exception? Exception) err)
    {
        if (err.Message.StartsWith("Err") && err.When.Length == 0 && err.Exception == null)
            return err.Message;

        if (err.When.Length == 0)
            err.When = GetWhenAsStackTrace(false);

        if (AddStackTraceToErr)
        {
            string stack = GetWhenAsStackTrace(true);
            if (err.Exception == null)
                return string.Format("Err[{0}] {1}\r\n{2}", err.When, err.Message, stack);
            else
                return string.Format("Err[{0} - {1}] {2} {3}\r\n{4}",
                    err.When, err.Exception.GetType().Name, err.Exception.Message, err.Message, stack);
        }
        else
        {
            if (err.Exception == null)
                return string.Format("Err[{0}] {1}", err.When, err.Message);
            else
                return string.Format("Err[{0} - {1}] {2} {3}",
                    err.When, err.Exception.GetType().Name, err.Exception.Message, err.Message);
        }
    }
}
