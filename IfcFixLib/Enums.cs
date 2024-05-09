namespace IfcFixLib;
public enum StringFilterType
{
    Equals,
    Contains,
    Contains_Any,
    Regex_Expression
}

public enum ElementStringValueType
{
    Name,
    Description,
    Tag,
    Property
}

public enum ProcessStatus
{
    Waiting,
    Processing,
    Done,
    Cancelled,
    Error
}
public enum IfcFormatOutput
{
	STEP,
	XML,
	JSON
}
