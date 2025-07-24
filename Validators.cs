using Func.Extensions;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Func;

/// <summary>
/// Provides helper methods to validate parameters.
/// </summary>
public static class Preconditions
{

    [return: NotNull]
    public static T CheckNotNull<T>([NotNull] this T value, [CallerArgumentExpression("value")] string name = "")
    {
        return Check.NotNull(value, name);
    }

    public static string CheckNotNullOrEmpty(string? value, [CallerArgumentExpression("value")] string name = "")
    {
        return Check.NotNullOrEmpty(value, name);
    }

    public static IEnumerable CheckNotNullOrEmpty([NotNull] this IEnumerable? value, [CallerArgumentExpression("value")] string name = "")
    {
        return Check.NotNullOrEmpty(value, name);
    }

    public static IEnumerable<T> CheckNotNullOrEmpty<T>([NotNull] this IEnumerable<T>? value, [CallerArgumentExpression("value")] string name = "")
    {
        return Check.NotNullOrEmpty(value, name);
    }

    public static Type CheckAssignableFrom(this Type? value, Type baseType, [CallerArgumentExpression("value")] string name = "")
    {
        return Check.AssignableFrom(value, baseType, name);
    }

    public static Type CheckDerivesFrom(this Type? value, Type baseType, [CallerArgumentExpression("value")] string name = "")
    {
        return Check.DerivesFrom(value, baseType, name);
    }

    public static T CheckEnumValid<T>(this T value, [CallerArgumentExpression("value")] string name = "") where T : Enum
    {
        return Check.EnumValid(value, name);
    }

    public static bool IsInRange<T>(this T value, T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true) where T : struct, IComparable<T>
    {
        return Check.IsInRange(value, min, minInclusive, max, maxInclusive);
    }

    public static T CheckRange<T>(this T value, [CallerArgumentExpression("value")] string name = "", T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true) where T : struct, IComparable<T>
    {
        return Check.Range(value, min, minInclusive, max, maxInclusive, name);
    }

    public static string? GetRangeError<T>(this T value, [CallerArgumentExpression("value")] string name = "", T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true) where T : struct, IComparable<T>
    {
        return Check.GetRangeError(value, min, minInclusive, max, maxInclusive, name);
    }

    public static void ThrowArgumentNullOrEmpty(this string name)
    {
        throw new ArgumentException(Lang.Resources.ValueEmpty.FormatInvariant(name), name);
    }

}

/// <summary>
/// Provides methods to validate objects based on DataAnnotations.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates an object based on its DataAnnotations and throws an exception if the object is not valid.
    /// </summary>
    /// <param name="v">The object to validate.</param>
    public static T ValidateAndThrow<T>(this T v)
    {
        v.CheckNotNull(nameof(v));
        Validator.ValidateObject(v, new ValidationContext(v), true);
        return v;
    }

    /// <summary>
    /// Validates an object based on its DataAnnotations and returns a list of validation errors.
    /// </summary>
    /// <param name="v">The object to validate.</param>
    /// <returns>A list of validation errors.</returns>
    public static ICollection<ValidationResult>? Validate<T>(this T v)
    {
        v.CheckNotNull(nameof(v));
        var results = new List<ValidationResult>();
        var context = new ValidationContext(v);
        if (!Validator.TryValidateObject(v, context, results, true))
        {
            return results;
        }
        return null;
    }
}
