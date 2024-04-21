using GeometryGym.Ifc;

namespace IfcFixLib.FilterStrategy;
public interface IFilterStrategy
{
    bool IsMatch(IfcElement element);
}
