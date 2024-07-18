using System.IO.Compression;
using System.Net;
using System.Text;

namespace WebIfcFix.Services;

public interface IUrlQueryParameterService
{
	string GetQueryParameterFromJson(string jsonLayout);
	string GetSanitizedJsonFromQueryParameter(string unsaveStringParameter);
}
public class UrlQueryService(ISanitizerService Sanitizer) : IUrlQueryParameterService
{
	public string GetQueryParameterFromJson(string jsonLayout)
	{
		byte[] data = Encoding.UTF8.GetBytes(jsonLayout);
		byte[] compressedData;

		using (var memoryStream = new MemoryStream())
		{
			using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
			{
				gzipStream.Write(data, 0, data.Length);
			}
			compressedData = memoryStream.ToArray();
		}
		string compressedString = Convert.ToBase64String(compressedData);	
		string queryParameter = WebUtility.UrlEncode(compressedString);
		return queryParameter;
	}

	public string GetSanitizedJsonFromQueryParameter(string unsaveStringParameter)
	{
		string jsonCompressed = WebUtility.UrlDecode(unsaveStringParameter);
		byte[] compressedData = Convert.FromBase64String(jsonCompressed);
		byte[] decompressedData;

		using (var compressedStream = new MemoryStream(compressedData))
		using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
		using (var resultStream = new MemoryStream())
		{
			gzipStream.CopyTo(resultStream);
			decompressedData = resultStream.ToArray();
		}

		string decompressedJson = Encoding.UTF8.GetString(decompressedData);
		string sanitazedJson = Sanitizer.SanitizeJson(decompressedJson);
		return sanitazedJson;
	}
}
