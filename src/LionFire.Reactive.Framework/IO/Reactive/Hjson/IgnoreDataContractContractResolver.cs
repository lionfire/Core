using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace LionFire.IO.Reactive.Hjson;

/// <summary>
/// DataContractAttribute infects derived classes, requiring [DataMember].  This resolver eliminates this problem.
/// </summary>
/// <remarks>
/// Inspired by https://stackoverflow.com/a/39300347/208304
/// 
/// See also:
/// 
// https://json.codeplex.com/discussions/357850
// https://stackoverflow.com/questions/8555089/datacontract-and-inheritance
// https://github.com/JamesNK/Newtonsoft.Json/issues/603
/// </remarks>
public class IgnoreDataContractContractResolver : DefaultContractResolver
{
 
    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var contract = base.CreateObjectContract(objectType);
        contract.MemberSerialization = GetMemberSerialization_IgnoreDataContractOptIn(objectType, contract.MemberSerialization);
        return contract;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        => base.CreateProperties(type, GetMemberSerialization_IgnoreDataContractOptIn(type, memberSerialization));

    static MemberSerialization GetMemberSerialization_IgnoreDataContractOptIn(Type type, MemberSerialization memberSerialization)
    {
        if (memberSerialization == MemberSerialization.OptIn)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var typeHasDataContract = BaseTypesAndSelf(type).Any(t => t.GetCustomAttribute<DataContractAttribute>() != null);

            if (typeHasDataContract
                && type.GetCustomAttribute<JsonObjectAttribute>() == null)
            {
                memberSerialization = MemberSerialization.OptOut;
            }
        }
        return memberSerialization;

        #region (local)

        static IEnumerable<Type> BaseTypesAndSelf(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType!;
            }
        }
        #endregion
    }

}

