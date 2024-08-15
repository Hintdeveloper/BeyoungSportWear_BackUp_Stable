
public class NumberToEnglish
{
    public String changeNumericToWords(int num)
    {
        if (num <= 0)
        {
            return num.ToString();
        }

        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "ᵗʰ";
        }

        switch (num % 10)
        {
            case 1:
                return num + "ˢᵗ";
            case 2:
                return num + "ⁿᵈ";
            case 3:
                return num + "ʳᵈ";
            default:
                return num + "ᵗʰ";
        }
    }
}

