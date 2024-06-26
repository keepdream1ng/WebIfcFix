﻿using GeometryGym.Ifc;

namespace IfcFixLib.FilterStrategy;
public class StringFilterStrategy : IFilterStrategy
{
    public StringValueGetter StringValueGetter { get; set; } = new StringValueGetter() { ValueType = ElementStringValueType.Name };
    public StringChecker StringChecker { get; set; } = new StringChecker();
    public string FilteredString { get; set; } = String.Empty;
    public bool IsMatch(IfcElement element)
    {
        return StringChecker.Check(StringValueGetter.GetFilteredValue(element), FilteredString);
    }
}
