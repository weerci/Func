using Func.Extensions;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Func;

/// <summary>
///     Provides easy methods to validate values.
/// </summary>
public static class Check
{
    /// <summary>
    ///  Validates whether specific value is not null, and throws an exception if it is null.
    /// </summary>
    [return: NotNull]
    public static T NotNull<T>([NotNull] T value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value == null)
        {
            throw new ArgumentNullException(name);
        }

        return value;
    }

    /// <summary>
    ///     Validates whether specific value is not null or empty, and throws an exception if it is null or empty.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The name of the parameter.</param>
    public static string NotNullOrEmpty([NotNull] string? value, [CallerArgumentExpression("value")] string name = "")
    {
        NotNull(value, name);
        if (string.IsNullOrEmpty(value))
        {
            ThrowArgumentNullOrEmpty(name);
        }

        return value;
    }

    /// <summary>
    ///     Validates whether specific list is not null or empty, and throws an exception if it is null or empty.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The name of the parameter.</param>
    public static IEnumerable NotNullOrEmpty([NotNull] IEnumerable? value, [CallerArgumentExpression("value")] string name = "")
    {
        NotNull(value, name);
        if (!value.GetEnumerator().MoveNext())
        {
            ThrowArgumentNullOrEmpty(name);
        }

        return value;
    }

    //
    // Сводка:
    //     Validates whether specific list is not null or empty, and throws an exception
    //     if it is null or empty.
    //
    // Параметры:
    //   value:
    //     The value to validate.
    //
    //   name:
    //     The name of the parameter.
    public static IEnumerable<T> NotNullOrEmpty<T>([NotNull] IEnumerable<T>? value, [CallerArgumentExpression("value")] string name = "")
    {
        NotNull(value, name);
        if (!value.Any())
        {
            ThrowArgumentNullOrEmpty(name);
        }

        return value;
    }

    //
    // Сводка:
    //     Validates whether specified type is assignable from specific base class.
    //
    // Параметры:
    //   value:
    //     The Type to validate.
    //
    //   baseType:
    //     The base type that value type must derive from.
    //
    //   name:
    //     The name of the parameter.
    public static Type AssignableFrom(Type? value, Type baseType, [CallerArgumentExpression("value")] string name = "")
    {
        NotNull(value, name);
        NotNull(baseType, "baseType");
        if (!value.IsAssignableFrom(baseType))
        {
            throw new ArgumentException(Lang.Resources.TypeMustBeAssignableFromBase.FormatInvariant(name, value.Name, baseType.Name), name);
        }

        return value;
    }

    //
    // Сводка:
    //     Validates whether specified type derives from specific base class.
    //
    // Параметры:
    //   value:
    //     The Type to validate.
    //
    //   baseType:
    //     The base type that value type must derive from.
    //
    //   name:
    //     The name of the parameter.
    public static Type DerivesFrom(Type? value, Type baseType, [CallerArgumentExpression("value")] string name = "")
    {
        NotNull(value, name);
        NotNull(baseType, "baseType");
        if (!value.IsSubclassOf(baseType))
        {
            throw new ArgumentException(Lang.Resources.TypeMustDeriveFromBase.FormatInvariant(name, value.Name, baseType.Name), name);
        }

        return value;
    }

    //
    // Сводка:
    //     Validates whether an enumeration value is valid, since it can contain any integer
    //     value. If the enumeration has FlagsAttribute, it also checks whether value is
    //     a combination of valid values.
    //
    // Параметры:
    //   value:
    //     The value to validate.
    //
    //   name:
    //     The name of the property.
    //
    // Параметры типа:
    //   T:
    //     The type of enumeration.
    public static T EnumValid<T>(T value, [CallerArgumentExpression("value")] string name = "") where T : Enum
    {
        int num = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        bool flag = Enum.IsDefined(typeof(T), num);
        if (!flag && IsEnumTypeFlags<T>())
        {
            flag = CheckEnumValidFlags<T>(num);
        }

        if (!flag)
        {
            throw new ArgumentException(Lang.Resources.ValueInvalidEnum.FormatInvariant(value, name, "T"), name);
        }

        return value;
    }

    private static bool IsEnumTypeFlags<T>() where T : Enum
    {
        return typeof(T).GetCustomAttributes(typeof(FlagsAttribute), inherit: true).Any();
    }

    private static bool CheckEnumValidFlags<T>(int value) where T : Enum
    {
        int num = 0;
        foreach (object value2 in Enum.GetValues(typeof(T)))
        {
            num |= (int)value2;
        }

        return (num & value) == value;
    }

    //
    // Сводка:
    //     Returns whether specified value is in valid range.
    //
    // Параметры:
    //   value:
    //     The value to validate.
    //
    //   min:
    //     The minimum valid value.
    //
    //   minInclusive:
    //     Whether the minimum value is valid.
    //
    //   max:
    //     The maximum valid value.
    //
    //   maxInclusive:
    //     Whether the maximum value is valid.
    //
    // Параметры типа:
    //   T:
    //     The type of data to validate.
    //
    // Возврат:
    //     Whether the value is within range.
    public static bool IsInRange<T>(T value, T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true) where T : struct, IComparable<T>
    {
        bool num = !min.HasValue || (minInclusive && value.CompareTo(min.Value) >= 0) || (!minInclusive && value.CompareTo(min.Value) > 0);
        bool flag = !max.HasValue || (maxInclusive && value.CompareTo(max.Value) <= 0) || (!maxInclusive && value.CompareTo(max.Value) < 0);
        return num && flag;
    }

    //
    // Сводка:
    //     Validates whether specified value is in valid range, and throws an exception
    //     if out of range.
    //
    // Параметры:
    //   value:
    //     The value to validate.
    //
    //   name:
    //     The name of the parameter.
    //
    //   min:
    //     The minimum valid value.
    //
    //   minInclusive:
    //     Whether the minimum value is valid.
    //
    //   max:
    //     The maximum valid value.
    //
    //   maxInclusive:
    //     Whether the maximum value is valid.
    //
    // Параметры типа:
    //   T:
    //     The type of data to validate.
    //
    // Возврат:
    //     The value if valid.
    public static T Range<T>(T value, T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true, [CallerArgumentExpression("value")] string name = "") where T : struct, IComparable<T>
    {
        if (!IsInRange(value, min, minInclusive, max, maxInclusive))
        {
            if (min.HasValue && minInclusive && max.HasValue && maxInclusive)
            {
                string valueRangeBetween = Lang.Resources.ValueRangeBetween;
                throw new ArgumentOutOfRangeException(name, value, valueRangeBetween.FormatInvariant(name, min, max));
            }

            string rangeError = GetRangeError(value, min, minInclusive, max, maxInclusive, name)!;
            throw new ArgumentOutOfRangeException(name, value, rangeError);
        }

        return value;
    }

    //
    // Сводка:
    //     Returns the range validation message.
    //
    // Параметры:
    //   value:
    //     The value to validate.
    //
    //   min:
    //     The minimum valid value.
    //
    //   minInclusive:
    //     Whether the minimum value is valid.
    //
    //   max:
    //     The maximum valid value.
    //
    //   maxInclusive:
    //     Whether the maximum value is valid.
    //
    //   name:
    //     The name of the parameter.
    //
    // Параметры типа:
    //   T:
    //     The type of data to validate.
    //
    // Возврат:
    //     The range validation message.
    public static string? GetRangeError<T>(T value, T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true, [CallerArgumentExpression("value")] string name = "") where T : struct, IComparable<T>
    {
        if (IsInRange(value, min, minInclusive, max, maxInclusive))
        {
            return null;
        }

        string text = (min.HasValue ? GetOpText(greaterThan: true, minInclusive).FormatInvariant(min) : null);
        string text2 = (max.HasValue ? GetOpText(greaterThan: false, maxInclusive).FormatInvariant(max) : null);
        return ((text != null && text2 != null) ? Lang.Resources.ValueRangeAnd : Lang.Resources.ValueRange).FormatInvariant(name, text ?? text2, text2);
    }

    private static string GetOpText(bool greaterThan, bool inclusive)
    {
        if (!(greaterThan && inclusive))
        {
            if (!greaterThan)
            {
                if (!inclusive)
                {
                    return Lang.Resources.ValueRangeLessThan;
                }

                return Lang.Resources.ValueRangeLessThanInclusive;
            }

            return Lang.Resources.ValueRangeGreaterThan;
        }

        return Lang.Resources.ValueRangeGreaterThanInclusive;
    }

    //
    // Сводка:
    //     Throws an exception of type ArgumentException saying an argument is null or empty.
    //
    //
    // Параметры:
    //   name:
    //     The name of the parameter.
    private static void ThrowArgumentNullOrEmpty(string name)
    {
        throw new ArgumentException(Lang.Resources.ValueEmpty.FormatInvariant(name), name);
    }
}
