namespace LionFire.Persistence.Testing
{
 
    public class TestClass1
    {

        public string StringProp { get; set; }
        public int IntProp { get; set; }
        public TestClass2 Object { get; set; }

        public static TestClass1 Create => new TestClass1()
        {
            StringProp = "string1",
            IntProp = 1,
            Object = new TestClass2
            {
                StringProp2 = "string2",
                IntProp2 = 2,
            }
        };
        
        public static string ExpectedNewtonsoftJson = @"{""$type"":""LionFire.Persistence.Testing.TestClass1, LionFire.Persistence.Testing"",""StringProp"":""string1"",""IntProp"":1,""Object"":{""StringProp2"":""string2"",""IntProp2"":2}}";

    }
}
