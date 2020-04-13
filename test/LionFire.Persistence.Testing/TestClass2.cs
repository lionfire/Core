namespace LionFire.Persistence.Testing
{
    public class TestClass2
    {
        public string StringProp2 { get; set; }
        public int IntProp2 { get; set; }

        public static TestClass2 Create => new TestClass2()
        {
            StringProp2 = "string2",
            IntProp2 = 2,
        };
    }
}
