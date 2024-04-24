using GeometryGym.Ifc;

namespace IfcFixLib;
public class DataIFC (DatabaseIfc databaseIfc, List<IfcBuiltElement> elements)
{
    public DatabaseIfc DatabaseIfc { get; } = databaseIfc;
    public List<IfcBuiltElement> Elements { get; } = elements;
}
