namespace LionFire.Serialization
{
    public static class SerializerSelectionContextExtensions
    {
        public static int DefaultShouldWeight = 100;

        public static SerializerSelectionContext MustBe(SerializerSelectionContext c, SerializerSelectionFlag flag)
        {
            c.FlagWeights[flag.ToString()] = int.MaxValue;
            return c;
        }
        public static SerializerSelectionContext MustNotBe(SerializerSelectionContext c, SerializerSelectionFlag flag)
        {
            c.FlagWeights[flag.ToString()] = int.MinValue;
            return c;
        }
        public static SerializerSelectionContext Prefer(SerializerSelectionContext c, SerializerSelectionFlag flag)
        {
            c.FlagWeights[flag.ToString()] = DefaultShouldWeight;
            return c;
        }
        public static SerializerSelectionContext PreferNot(SerializerSelectionContext c, SerializerSelectionFlag flag)
        {
            c.FlagWeights[flag.ToString()] = -DefaultShouldWeight;
            return c;
        }
    }

}
