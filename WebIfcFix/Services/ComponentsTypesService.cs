using System.Reflection;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public interface IComponentsTypesService
{
	IReadOnlyList<IComponentInformation> ComponentsTypes { get; }
}

public class ComponentsTypesService : IComponentsTypesService
{
    public IReadOnlyList<IComponentInformation> ComponentsTypes { get; private set; }

    public ComponentsTypesService()
    {
		ComponentsTypes = LoadComponentsInfo();
    }

	private List<IComponentInformation> LoadComponentsInfo()
	{
		IEnumerable<IComponentInformation?> componentsInfo = Assembly.GetAssembly(typeof(IComponentInformation))!
			.GetTypes()
			.Where(type => type.IsClass && typeof(IComponentInformation).IsAssignableFrom(type) && !type.IsGenericTypeDefinition)
			.Select(type => Activator.CreateInstance(type) as IComponentInformation);

		List<IComponentInformation> successfullyCreatedInfo = new();
		foreach (IComponentInformation? componentInfo in componentsInfo)
		{
			if (componentInfo is not null)
			{
				successfullyCreatedInfo.Add(componentInfo);
			}
		}
		return successfullyCreatedInfo;
	}
}
