using System.Net.Http.Json;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;

public class PredefinedLayoutsService(HttpClient httpClient, IJsonConvertService convertService)
{
	private Dictionary<string, List<SerializableModelBase>>? _predefinedFilterComponentsLayouts;

	public async Task LoadDataAsync()
	{
		string json = await httpClient.GetStringAsync("data/predefinedFilterComponentsLayouts.json");
		_predefinedFilterComponentsLayouts = convertService.DeserializeObject<Dictionary<string, List<SerializableModelBase>>>(json);
	}

	public string[] GetPredefinedLayoutsNames()
	{
		string[] names;
		if (_predefinedFilterComponentsLayouts is not null)
		{
			names = _predefinedFilterComponentsLayouts.Keys.ToArray();
		}
		else
		{
			names = Array.Empty<string>();
		}
		return names;
	}

	public List<SerializableModelBase>? GetLayoutByName(string name)
	{
		return _predefinedFilterComponentsLayouts?[name];
	}
}
