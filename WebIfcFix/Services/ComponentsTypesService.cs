using System.Reflection;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public interface IComponentsTypesService
{
	List<IComponentInformation> ComponentsTypes { get; }
}

public class ComponentsTypesService : IComponentsTypesService
{
    public List<IComponentInformation> ComponentsTypes { get; init; } = new();

    public ComponentsTypesService()
    {
        LoadComponentsInfo();
    }

	public void LoadComponentsInfo()
	{
		List<IComponentInformation?> componentsInfo = Assembly.GetAssembly(typeof(IComponentInformation))!
			.GetTypes()
			.Where(type => type.IsClass && typeof(IComponentInformation).IsAssignableFrom(type) && !type.IsGenericTypeDefinition)
			.Select(type => Activator.CreateInstance(type) as IComponentInformation)
			.ToList();

		foreach (IComponentInformation? info in componentsInfo)
		{
			if (info is not null)
			{
				ComponentsTypes.Add(info);
			}
		}

		componentsInfo.Clear();
	}
}
