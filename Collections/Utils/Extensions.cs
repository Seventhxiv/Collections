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
}