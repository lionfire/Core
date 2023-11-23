
public static class ZipTest
{
    public const string TestSourceFile = "TestSourceFile.zip";

    public static VobReference ZipBaseVobReference = "/testdata/zip".ToVobReference();

    public static VobReference ZipVobReference => ZipBaseVobReference.GetChildSubpath(TestSourceFile);
}

