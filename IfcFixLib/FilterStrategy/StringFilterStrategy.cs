using GeometryGym.Ifc;

namespace IfcFixLib.FilterStrategy;
public class StringFilterStrategy : IFilterStrategy
{
    public required StringValueGetter StringValueGetter { get; set; }
    public required StringChecker StringChecker { get; set; }
    public string FilteredString { get; set; } = String.Empty;
    public bool IsMatch(IfcElement element)
    {
        return StringChecker.Check(StringValueGetter.GetValue(element), FilteredString);
    }
}
