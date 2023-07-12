using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LionFire.Collections;
using System.Collections;

namespace LionFire.Serialization;

public static class LionSerializers
{
    // FUTURE: also add mime-types or something like that, so that compression 
    // serialization types like zip can recurse

    public static IReadOnlyCollection<LionSerializer> Serializers
    { get { return serializers; } }
    private static MultiBindableCollection<LionSerializer> serializers = new MultiBindableCollection<LionSerializer>();

    private static MultiValueDictionary<string, LionSerializer> serializersByExtension = new MultiValueDictionary<string, LionSerializer>();

    #region Serializers (HARDCODE)

    #region Json

    public static LionJsonSerializer Json
    {
        get
        {
            return lionJsonSerializer;
        }
    } private static LionJsonSerializer lionJsonSerializer = new LionJsonSerializer();

    #endregion

    #region FastJson
#if FASTJSON
    public static FastJsonSerializer FastJson
    {
        get
        {
            return fastJsonSerializer;
        }
    } private static FastJsonSerializer fastJsonSerializer = new FastJsonSerializer();
#endif
    #endregion

    #region LionPack

#if MSGPACK
    public static LionPackSerializer LionPack
    {
        get
        {
            return lionPackSerializer;
        }
    } private static LionPackSerializer lionPackSerializer = new LionPackSerializer();
#endif
    #endregion

    #endregion

    #region Construction

    static LionSerializers()
    {
        RegisterSerializer(lionJsonSerializer);
#if MSGPACK
        RegisterSerializer(lionPackSerializer);
#endif
        //byte[] untypedJsonIdentifier = UTF8Encoding.UTF8.GetBytes("{");
        //byte[] xmlIdentifier = UTF8Encoding.UTF8.GetBytes("<");
    }

    #endregion

    public static void RegisterSerializer(LionSerializer serializer)
    {
        serializers.Add(lionJsonSerializer);

        foreach (var fileExtension in serializer.FileExtensions)
        {
            serializersByExtension.Add(fileExtension, serializer);
        }
    }

    public static LionSerializer DetectSerializer(Stream stream, string path = null)
    {
        string extension = path == null ? null : System.IO.Path.GetExtension(path).TrimStart('.');

        if (extension != null)
        {
            var serializers = serializersByExtension.TryGetValue(extension, returnEmptySet:true);
            foreach (LionSerializer prospectiveSerializer in
#if AOT
(IEnumerable)
#endif
                serializers)
            {
                if (prospectiveSerializer.MatchesHeader(stream))
                {
                    return prospectiveSerializer;
                }
            }
        }
        foreach (var prospectiveSerializer in Serializers)
        {
            if (prospectiveSerializer.MatchesHeader(stream))
            {
                return prospectiveSerializer;
            }
        }
        return null;
    }
}
