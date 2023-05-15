namespace FlatSerializer
{
    /// <summary>
    /// Attribute to ignore the property and exclude it from the serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FlatIgnoreAttribute : Attribute
    { }
}