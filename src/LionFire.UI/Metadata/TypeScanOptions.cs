namespace LionFire.UI.Metadata;

public class TypeScanOptions
{
    #region Static

    public static Func<TypeScanOptions> CreateDefault { get; set; } = () =>
        {
            var o = new TypeScanOptions();
            o.Methods.Enabled = false;
            return o;
        };

    public static TypeScanOptions Default { get => @default ?? CreateDefault(); set => @default = value; }
    public static TypeScanOptions? @default;

    #endregion

    public TypeScanOptions()
    {
        IgnoreMemberAttributes = new(DefaultIgnoreMemberAttributes);
    }

    public List<Type> IgnoreMemberAttributes { get; set; } = new();
    public static List<Type> DefaultIgnoreMemberAttributes = new List<Type>
        {
            typeof(IgnoreAttribute),
        };

    public MemberScanOptions Properties { get; set; } = new();
    public MemberScanOptions Fields { get; set; } = new();
    public MemberScanOptions Methods { get; set; } = new();
}
