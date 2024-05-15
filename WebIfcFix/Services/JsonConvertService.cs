using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebIfcFix.Shared;

namespace WebIfcFix.Services;
public interface IJsonConvertService
{
	T? DeserializeObject<T>(string json);
	string SerializeObject(object? obj);
}

public class JsonConvertService : IJsonConvertService
{
	public string SerializeObject(object? obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.Indented);
	}

	public T? DeserializeObject<T>(string json)
	{
		var settings = new JsonSerializerSettings
		{
			Converters = { new ChildComponentModelConverter() }
		};
		return JsonConvert.DeserializeObject<T>(json, settings);
	}

	// Custom converters for this web app logic.
	class ChildComponentModelConverter : JsonConverter<ChildComponentModelBase>
	{
		public override bool CanRead => true;
		public override bool CanWrite => false;

		public override ChildComponentModelBase ReadJson(JsonReader reader, Type objectType, ChildComponentModelBase existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);
			string? modelType = jsonObject.GetValue(nameof(ChildComponentModelBase.ModelType))?.ToString();
			if (modelType is null)
			{
				throw new JsonSerializationException($"Could not deserialize");
			}

			// Determine the concrete type to create based on the ModelDerivedType property.
			ChildComponentModelBase? component;
			var type = Type.GetType(modelType);
			if (type is null)
			{
				throw new JsonSerializationException($"Unknown component type: {modelType}");
			}

			component = Activator.CreateInstance(type!) as ChildComponentModelBase;
			if (component is null)
			{
				throw new JsonSerializationException($"Error on casting to {nameof(ChildComponentModelBase)}");
			}

			// Populate the properties of the concrete type.
			serializer.Populate(jsonObject.CreateReader(), component);
			return component;
		}

		public override void WriteJson(JsonWriter writer, ChildComponentModelBase value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
