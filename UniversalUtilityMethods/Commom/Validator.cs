
public class Validator
{
    public static string formatDate = "dd/MM/yyyy";
    public static Decimal MIN_RESERVED_CASH_FLOW = 0;
    public static Decimal MAX_RESERVED_CASH_FLOW = 1000000;

    public static bool validStringEmpty(String s)
    {
        bool result = true;
        if (s != null && s != "")
        {
            result = false;
        }
        return result;
    }

    public static bool validRangeDate(DateTime value, DateTime coreDate)
    {
        bool result = false;
        if (value.CompareTo(coreDate) >= 0)
        {
            result = true;
        }
        return result;
    }

    public static bool validCharInRange(String sSource, String sCheck)
    {
        bool result = false;
        foreach (char c in sSource)
        {
            if (sCheck == c.ToString())
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public static bool validNotDate(String value)
    {
        bool result = false;
        if (isDate(value) == false)
        {
            result = true;
        }
        return result;
    }

    public static bool validNotTime(String value)
    {
        bool result = false;
        if (isTime(value) == false)
        {
            result = true;
        }
        return result;
    }

    public static bool isDate(String value)
    {
        try
        {
            //DateTime.ParseExact(value, formatDate, null);
            DateTime dt = DateTime.Parse(value, new System.Globalization.CultureInfo("vi-VN"));
            return true;
        }
        catch (Exception)
        {
            //debug(ex);
            return false;
        }
    }

    public static bool isTime(String value)
    {
        try
        {
            DateTime.ParseExact(value, "HH:mm", null);
            return true;
        }
        catch (Exception)
        {
            //debug(ex);
            return false;
        }
    }

    public static bool validStringOverRange(String s, int maxLength)
    {
        bool result = false;
        if (s.Length > maxLength)
        {
            result = true;
        }
        return result;
    }

    public static bool validNbrOverRange(String s, Decimal val)
    {
        bool result = false;
        Decimal sNbr = Decimal.Parse(s);
        if (sNbr > val)
        {
            result = true;
        }
        return result;
    }

    public static bool validNbrUnderRange(String s, Decimal val)
    {
        bool result = false;
        Decimal sNbr = Decimal.Parse(s);
        if (sNbr < val)
        {
            result = true;
        }
        return result;
    }

    public static bool isEvenByMUnit(String s)
    {
        Decimal sNbr = Decimal.Parse(s);
        if (sNbr % 1000000 == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

