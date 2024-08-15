using System.Reflection;

public static class CustNull
{
    public static System.DateTime MIN_DATE = DateTime.MinValue;
    public static System.DateTime MAX_DATE = DateTime.MaxValue;
    public static System.DateTime NEXT_DATE = DateTime.Now.AddDays(1);


    public static short NullShort
    {
        get { return short.MinValue; }
    }
    public static int NullInteger
    {
        get { return int.MinValue; }
    }

    public static long NullLong
    {
        get { return long.MinValue; }
    }

    public static float NullSingle
    {
        get { return float.MinValue; }
    }
    public static double NullDouble
    {
        get { return double.MinValue; }
    }
    public static decimal NullDecimal
    {
        get { return decimal.MinValue; }
    }
    public static System.DateTime NullDate
    {
        get { return System.DateTime.MinValue; }
    }
    public static string NullString
    {
        get { return string.Empty; }
    }
    public static bool NullBoolean
    {
        get { return false; }
    }
    public static Guid NullGuid
    {
        get { return Guid.Empty; }
    }

    // sets a field to an application encoded null value ( used in BLL layer )
    public static object SetNull(PropertyInfo objPropertyInfo)
    {
        object? functionReturnValue = null;
        switch (objPropertyInfo.PropertyType.ToString())
        {
            case "System.Int16":
                functionReturnValue = NullShort;
                break;
            case "System.Int32":
                functionReturnValue = NullInteger;
                break;
            case "System.Int64":
                functionReturnValue = NullLong;
                break;
            case "System.Single":
                functionReturnValue = NullSingle;
                break;
            case "System.Double":
                functionReturnValue = NullDouble;
                break;
            case "System.Decimal":
                functionReturnValue = NullDecimal;
                break;
            case "System.DateTime":
                functionReturnValue = NullDate;
                break;
            case "System.String":
            case "System.Char":
                functionReturnValue = NullString;
                break;
            case "System.Boolean":
                functionReturnValue = NullBoolean;
                break;
            case "System.Guid":
                functionReturnValue = NullGuid;
                break;
            default:
                // Enumerations default to the first entry
                Type pType = objPropertyInfo.PropertyType;
                if (pType.BaseType.Equals(typeof(System.Enum)))
                {
                    System.Array objEnumValues = System.Enum.GetValues(pType);
                    Array.Sort(objEnumValues);
                    functionReturnValue = System.Enum.ToObject(pType, objEnumValues.GetValue(0));
                }
                else
                {
                    // complex object
                    functionReturnValue = null;
                }

                break;
        }
        return functionReturnValue;
    }

    // convert an application encoded null value to a database null value ( used in DAL )
    public static object GetNull(object objField, object objDBNull)
    {
        object functionReturnValue = null;
        functionReturnValue = objField;
        if (objField == null)
        {
            functionReturnValue = objDBNull;
        }
        else if (objField is short)
        {
            if (Convert.ToInt16(objField) == NullShort)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is int)
        {
            if (Convert.ToInt32(objField) == NullInteger)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is long)
        {
            if (Convert.ToInt64(objField) == NullLong)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is float)
        {
            if (Convert.ToSingle(objField) == NullSingle)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is double)
        {
            if (Convert.ToDouble(objField) == NullDouble)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is decimal)
        {
            if (Convert.ToDecimal(objField) == NullDecimal)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is System.DateTime)
        {
            // compare the Date part of the DateTime with the DatePart of the NullDate ( this avoids subtle time differences )
            if (Convert.ToDateTime(objField).Date == NullDate.Date | Convert.ToDateTime(objField).Date == MIN_DATE | Convert.ToDateTime(objField).Date == MAX_DATE)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is string)
        {
            if (objField == null)
            {
                functionReturnValue = objDBNull;
            }
            else
            {
                if (objField.ToString() == NullString)
                {
                    functionReturnValue = objDBNull;
                }
            }
        }
        else if (objField is bool)
        {
            if (Convert.ToBoolean(objField) == NullBoolean)
            {
                functionReturnValue = objDBNull;
            }
        }
        else if (objField is Guid)
        {
            if (((System.Guid)objField).Equals(NullGuid))
            {
                functionReturnValue = objDBNull;
            }
        }
        return functionReturnValue;
    }

    public static bool IsNull(object objField)
    {
        bool functionReturnValue = false;
        if ((objField != null))
        {
            if (objField is int)
            {
                functionReturnValue = objField.Equals(NullInteger);
            }
            else if (objField is short)
            {
                functionReturnValue = objField.Equals(NullShort);
            }
            else if (objField is long)
            {
                functionReturnValue = objField.Equals(NullLong);
            }
            else if (objField is float)
            {
                functionReturnValue = objField.Equals(NullSingle);
            }
            else if (objField is double)
            {
                functionReturnValue = objField.Equals(NullDouble);
            }
            else if (objField is decimal)
            {
                functionReturnValue = objField.Equals(NullDecimal);
            }
            else if (objField is System.DateTime)
            {
                DateTime objDate = (DateTime)objField;
                functionReturnValue = objDate.Date.Equals(NullDate.Date);
            }
            else if (objField is string)
            {
                functionReturnValue = objField.Equals(NullString);
            }
            else if (objField is bool)
            {
                functionReturnValue = objField.Equals(NullBoolean);
            }
            else if (objField is Guid)
            {
                functionReturnValue = objField.Equals(NullGuid);
            }
            else if (objField is DBNull)
            {
                functionReturnValue = true;
            }
            else
            {
                functionReturnValue = false;
            }
        }
        else
        {
            functionReturnValue = true;
        }
        return functionReturnValue;
    }
}
