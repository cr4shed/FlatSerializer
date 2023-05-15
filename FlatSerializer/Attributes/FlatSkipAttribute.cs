namespace FlatSerializer
{
    /// <summary>
    /// Attribute to skip flattening and serialize the normally.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FlatSkipAttribute : Attribute
    { }
}
