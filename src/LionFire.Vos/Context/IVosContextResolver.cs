namespace LionFire.Vos
{
    public interface IVosContextResolver
    {
        Vob GetVobRoot(string path, VosContext context);
        Vob GetVobRoot(string path= null, string package = null, string location= null);
    }
}

