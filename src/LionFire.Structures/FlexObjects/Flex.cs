using System.Reflection;

namespace LionFire.FlexObjects
{
    public partial class Flex : IFlex
    {
        public object FlexData { get; set; }
    }

    public partial class Flex
    {
        private static MethodInfo addMethod = typeof(IFlexExtensions).GetMethod("Add");

        public static IFlex Create(params object[] components)
        {
            var flex = new Flex();
            foreach(var obj in components)
            {
                addMethod.MakeGenericMethod(obj.GetType()).Invoke(flex, new object[] { obj });
            }
            return flex;
        }
    }

}
