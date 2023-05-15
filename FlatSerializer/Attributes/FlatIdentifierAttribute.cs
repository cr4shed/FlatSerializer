namespace FlatSerializer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FlatIdentifierAttribute : Attribute
    {
        private string Identifier { get; set; }

        public FlatIdentifierAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public string GetIdentifierProperty() => Identifier;
        public string GetFormattedValue(string value) => Identifier + value?.Replace(" ", "").ReplaceLineEndings("") ?? null;
    }
}
