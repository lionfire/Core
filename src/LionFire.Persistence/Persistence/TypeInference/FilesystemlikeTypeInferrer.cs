#nullable enable

using LionFire.Ontology;
using LionFire.Persistence.Filesystemlike;
using LionFire.Serialization;
using System.Threading.Tasks;

namespace LionFire.Persistence.TypeInference;

public class FilesystemlikeTypeInferrer : SerializingTypeInferrer, ITypeInferrer
{
    #region Relationships

    IVirtualFilesystemPersister Persister;

    #endregion

    public TypeInferenceOptions TypeInferrerOptions => (Persister as IHas<PersistenceOptions>)?.Object?.TypeInferenceOptions ?? TypeInferenceOptionsDefault;

    public FilesystemlikeTypeInferrer(IVirtualFilesystemPersister p, ISerializationProvider serializationProvider) : base(serializationProvider)
    {
        Persister = p;
    }

    public async Task<TypeInferenceResult?> GetTypeForKey(string key)
    {
        TypeInferenceResult? result = null;
        if (TypeInferrerOptions.Modes.HasFlag(TypeInferenceMode.Extension))
        {
            result = await GetTypeForKeyFromExtension(key);
            if (result != null) return result;
        }
        if (TypeInferrerOptions.Modes.HasFlag(TypeInferenceMode.Deserialize))
        {
            result = await GetTypeForKeyFromExtension(key);
            if (result != null) return result;
        }

        return null;
    }

    public static TypeInferenceOptions TypeInferenceOptionsDefault { get; set; } = new()
    {
        Modes = TypeInferenceMode.Deserialize | TypeInferenceMode.Extension | TypeInferenceMode.Magic
    };


}

