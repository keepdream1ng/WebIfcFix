using Ganss.Xss;

namespace WebIfcFix.Services;

public interface ISanitizerService
{
	string Sanitize(string text);
}

public class SanitizerService : ISanitizerService
{
	public string Sanitize(string text)
	{
		HtmlSanitizer sanitizer = new HtmlSanitizer();
		return sanitizer.Sanitize(text);
	}
}
