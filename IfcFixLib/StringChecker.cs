using System.Text.RegularExpressions;

namespace IfcFixLib;

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
                case StringFilterType.Regex_expression:
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

    public bool Check(string prop, string searchString)
    {
        return CheckDelegate(prop, searchString);
    }

    private bool EqualsCheck(string prop, string searchString)
    {
        return prop == searchString;
    }

    private bool ContainsCheck(string prop, string searchString)
    {
        return prop.Contains(searchString);
    }
    private bool ContainsAnyCheck(string prop, string searchString)
    {
        if ((searchString != _containsCasheValue))
        {
            _containsCasheValue = searchString;
            _containsCashe = searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        foreach (string word in _containsCashe)
        {
            if (prop.Contains(word))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckRegex(string prop, string searchString)
    {
        if (!_regexCache.TryGetValue(searchString, out Regex regex))
        {
            regex = new Regex(searchString);
            _regexCache[searchString] = regex;
        }

        return regex.IsMatch(prop);
    }
}
