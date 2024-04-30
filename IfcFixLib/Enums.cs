﻿namespace IfcFixLib;
public enum StringFilterType
{
    Equals,
    Contains,
    Contains_Any,
    Regex_expression
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