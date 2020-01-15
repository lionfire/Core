namespace LionFire.Vos
{
    /// <summary>
    /// Resolves the Root Vob for a VosContext, or combination of package and/or location
    /// </summary>
    /// <remarks>
    /// ENH: Make this n-dimensional based on a dictionary of specifiers?
    /// </remarks>
    public interface IVosContextResolver
    {
        Vob GetVobRoot(string path, VosContext context);
        Vob GetVobRoot(string path= null, string package = null, string location= null);
    }
}

