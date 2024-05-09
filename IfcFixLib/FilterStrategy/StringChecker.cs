using System.Text.RegularExpressions;

namespace IfcFixLib.FilterStrategy;

public class StringChecker
{
    public StringFilterType FilterType
    {
        get { return _stringFilterType; }
        set
        {
            switch (value)
            {
                case StringFilterType.Equals:
                    CheckDelegate = EqualsCheck;
                    break;
                case StringFilterType.Contains:
                    CheckDelegate = ContainsCheck;
                    break;
                case StringFilterType.Contains_Any:
                    CheckDelegate = ContainsAnyCheck;
                    break;
                case StringFilterType.Regex_Expression:
                    CheckDelegate = CheckRegex;
                    break;
                default:
                    throw new NotImplementedException();
            }
            _stringFilterType = value;
        }
    }
    private StringFilterType _stringFilterType;
    private delegate bool FilterNameCheckerDelegate(string prop, string searchString);
    private FilterNameCheckerDelegate CheckDelegate;
    private string? _containsCasheValue = null;
    private string[]? _containsCashe = null;
    private Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();
    private object _lock = new();

    public StringChecker(StringFilterType filterType = StringFilterType.Equals)
    {
        CheckDelegate = EqualsCheck;
        FilterType = filterType;
    }

    public bool Check(string prop, string searchString)
    {
        return CheckDelegate(prop, searchString);
    }

    private bool EqualsCheck(string prop, string searchString)
    {
        return String.Equals(prop, searchString);
    }

    private bool ContainsCheck(string prop, string searchString)
    {
        return prop.Contains(searchString, StringComparison.InvariantCultureIgnoreCase);
    }
    private bool ContainsAnyCheck(string prop, string searchString)
    {
        if (searchString != _containsCasheValue)
        {
            lock (_lock)
            {
                if (_containsCasheValue != searchString)
                {
                    _containsCasheValue = searchString;
                    _containsCashe = searchString
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        return _containsCashe!.Any(word => prop.Contains(word, StringComparison.InvariantCultureIgnoreCase));
    }

    private bool CheckRegex(string prop, string searchString)
    {
        if (!_regexCache.TryGetValue(searchString, out var regex))
        {
            lock (_lock)
            {
                if (!_regexCache.ContainsKey(searchString))
                {
                    regex = new Regex(searchString);
                    _regexCache[searchString] = regex;
                }
            }
        }

        return regex!.IsMatch(prop);
    }
}
