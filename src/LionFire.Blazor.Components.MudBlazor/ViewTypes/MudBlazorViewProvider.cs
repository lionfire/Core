using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Blazor.Components.MudBlazor_;

public class MudBlazorViewTypeProvider : IViewTypeProvider
{
    public bool HasView(Type modelType)
    {
        return GetViewType(modelType) != null;
    }

    public Type? GetViewType(Type? type)
    {
        if (type == null) return null;

        if (type.IsGenericType)
        {
            var generic = type.GetGenericTypeDefinition();
            if (generic == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }
        }

        if (type == typeof(bool)) return typeof(MudCheckBox<bool>);

        if (type == typeof(string)) return typeof(MudTextField<string>);

        if (type == typeof(int)) return typeof(MudNumericField<int>);
        if (type == typeof(uint)) return typeof(MudNumericField<uint>);
        if (type == typeof(short)) return typeof(MudNumericField<short>);
        if (type == typeof(ushort)) return typeof(MudNumericField<ushort>);
        if (type == typeof(byte)) return typeof(MudNumericField<byte>);
        if (type == typeof(sbyte)) return typeof(MudNumericField<sbyte>);
        if (type == typeof(long)) return typeof(MudNumericField<long>);
        if (type == typeof(ulong)) return typeof(MudNumericField<ulong>);
        if (type == typeof(float)) return typeof(MudNumericField<float>);
        if (type == typeof(double)) return typeof(MudNumericField<double>);
        if (type == typeof(decimal)) return typeof(MudNumericField<decimal>);

        if (type == typeof(DateTime)) return typeof(MudDatePicker);
        if (type == typeof(DateTimeOffset)) return typeof(MudDatePicker);
        if (type == typeof(TimeSpan)) return typeof(MudTimePicker);

        if (type == typeof(char)) return typeof(MudTextField<char>);
        if (type == typeof(Guid)) return typeof(MudTextField<Guid>);
        if (type == typeof(Uri)) return typeof(MudTextField<Uri>);

        return null;
        //return typeof(MudTextField<>).MakeGenericType(type);
    }
}
