using GeometryGym.Ifc;

namespace IfcFixLib.FilterStrategy;
public class StringValueGetter
{
    public IfcElementStringValueType ValueType { get; set; }
    public string? PropertyName { get; set; }

    public string GetFilteredValue(IfcElement element) => ValueType switch
    {
        IfcElementStringValueType.Name        => element.Name,
        IfcElementStringValueType.Description => element.Description,
        IfcElementStringValueType.Tag         => element.Tag,
        IfcElementStringValueType.Property    => GetProperty(element),
        _ => throw new NotImplementedException()
    };

    private string GetProperty(IfcElement element)
    {
        if (PropertyName is not null)
        {
            var prop = element.FindProperty(PropertyName) as IfcPropertySingleValue;
            if ((prop is not null) && (prop.NominalValue is not null))
            {
                return prop.NominalValue.ValueString;
            }
        }
        return string.Empty;
    }
}
