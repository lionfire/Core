#nullable enable

using LionFire.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.TypeInference;

public class SerializingTypeInferrer
{
    #region Dependencies

    public ISerializationProvider SerializationProvider { get; }

    #endregion

    public SerializingTypeInferrer(ISerializationProvider serializationProvider)
    {
        SerializationProvider = serializationProvider;
    }

    public Task<TypeInferenceResult?> GetTypeForKeyFromExtension(string key)
    {
        var extension = System.IO.Path.GetExtension(key);

        foreach (var strategy in SerializationProvider.Strategies)
        {
            foreach (var format in strategy.Formats)
            {
                if (format.FileExtensions.Contains(extension))
                {
                    return Task.FromResult<TypeInferenceResult?>(new()
                    {
                        SerializationFormat = format,
                    });
                }
            }
        }
        return Task.FromResult<TypeInferenceResult?>(null);
    }

    public Task<TypeInferenceResult?> GetTypeForKeyFromDeserialize(string key)
    {
        throw new NotImplementedException();
        return Task.FromResult<TypeInferenceResult?>(null);
    }
}

