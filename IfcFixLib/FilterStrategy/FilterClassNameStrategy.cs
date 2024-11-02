using GeometryGym.Ifc;
using System.Reflection;

namespace IfcFixLib.FilterStrategy;
public class FilterClassNameStrategy : IFilterStrategy
{
	public string FilterInClassName { get; set; } = String.Empty;
	public bool IsMatch(IfcElement element)
	{
		return element.StepClassName.Equals(FilterInClassName);
	}
}
