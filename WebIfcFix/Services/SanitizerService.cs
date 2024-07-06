using Ganss.Xss;

namespace WebIfcFix.Services;

public interface ISanitizerService
{
	string SanitizeJson(string text);
}

public class SanitizerService : ISanitizerService
{
	public string SanitizeJson(string text)
	{
		HtmlSanitizer sanitizer = new HtmlSanitizer();
		return sanitizer.Sanitize(text);
	}
}
