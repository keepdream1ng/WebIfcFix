using GeometryGym.Ifc;

namespace IfcFixLib.FilterStrategy;
public class StringValueGetter
{
    public ElementStringValueType ValueType { get; set; }
    public string? PropertyName { get; set; }

    public string GetValue(IfcElement element) => ValueType switch
    {
        ElementStringValueType.Name        => element.Name,
        ElementStringValueType.Description => element.Description,
        ElementStringValueType.Tag         => element.Tag,
        ElementStringValueType.Property    => GetProperty(element),
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
