using GeometryGym.Ifc;

namespace IfcFixLib;
public class DataIFC (DatabaseIfc databaseIfc, List<IfcElement> elements)
{
    public DatabaseIfc DatabaseIfc { get; } = databaseIfc;
    public List<IfcElement> Elements { get; } = elements;
}
