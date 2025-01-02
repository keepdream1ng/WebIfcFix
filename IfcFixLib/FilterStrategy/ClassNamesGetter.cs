using GeometryGym.Ifc;
using System.Reflection;

namespace IfcFixLib.FilterStrategy;
public static class ClassNamesGetter
{
	public static string[] IfcClassNames { get; private set; }
    static ClassNamesGetter()
    {
		IfcClassNames = GetIfcClassNames();
    }
	private static string[] GetIfcClassNames()
	{
		string[] elementsClassNames = Assembly.GetAssembly(typeof(IfcElement))!
			.GetTypes()
			.Where(type => type.IsClass && typeof(IfcElement).IsAssignableFrom(type) && !type.IsGenericTypeDefinition)
			.Select(type => type.Name)
			.ToArray();

		return elementsClassNames;
	}
}
