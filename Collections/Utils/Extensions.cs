using System.Text;
using System.Text.RegularExpressions;

namespace Collections;

public static class Extensions
{
    // Enums
    public static IEnumerable<T> GetEnumValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static string GetEnumName<T>(this T t) where T : struct, IConvertible
    {
        return Enum.GetName(typeof(T), t);
    }

    public static T GetEnumValue<T>(this string t) where T : struct
    {
        return (T)Enum.Parse(typeof(T), t);
    }

    // Reflection
    public static TReturn GetProperty<TReturn>(this object t, string property)
    {
        return (TReturn)t.GetType().GetProperty(property).GetValue(t);
    }

    // Strings
    public static string ToSentence(this string s)
    {
        return Regex.Replace(s, "([a-z]{2})_?([A-Z])", "$1 $2");
    }

    public static string UpperCaseAfterSpaces(this string s)
    {
        var stringBuilder = new StringBuilder(s);
        for (var i = 0; i < stringBuilder.Length - 1; i++)
        {
            if (i == 0)
            {
                stringBuilder[0] = char.ToUpper(stringBuilder[0]);
            }
            else if (stringBuilder[i] == ' ')
            {
                stringBuilder[i + 1] = char.ToUpper(stringBuilder[i + 1]);
            }
        }
        return stringBuilder.ToString();
    }

    public static string LowerCaseWords(this string s, List<String> words)
    {
        foreach (var word in words)
        {
            s = s.Replace(" " + word + " ", " " + word.ToLower() + " ");
        }
        return s;
    }

    public static string LowerCaseAfter(this string s, List<Char> chars)
    {
        var stringBuilder = new StringBuilder(s);
        for (var i = 0; i < stringBuilder.Length - 1; i++)
        {
            if (chars.Contains(stringBuilder[i]))
            {
                stringBuilder[i + 1] = char.ToLower(stringBuilder[i + 1]);
            }
        }
        return stringBuilder.ToString();
    }

    public static string RemoveSuffix(this string s, string suffix)
    {
        if (s.EndsWith(suffix))
        {
            return s.Substring(0, s.Length - suffix.Length);
        }

        return s;
    }

    public static string RemovePrefix(this string s, string prefix)
    {
        if (s.StartsWith(prefix))
        {
            return s.Substring(prefix.Length);
        }

        return s;
    }
}