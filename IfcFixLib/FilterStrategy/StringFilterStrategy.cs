using GeometryGym.Ifc;

namespace IfcFixLib.FilterStrategy;
public class StringFilterStrategy : IFilterStrategy
{
    public StringValueGetter StringValueGetter { get; set; }
    public StringChecker StringChecker { get; set; }
    public string FilteredString { get; set; }
    public bool IsMatch(IfcElement element)
    {
        return StringChecker.Check(StringValueGetter.GetFilteredValue(element), FilteredString);
    }
}
