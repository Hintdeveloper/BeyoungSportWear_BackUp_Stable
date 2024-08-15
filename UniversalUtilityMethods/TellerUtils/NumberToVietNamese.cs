
    public class NumberToVietNamese
    {
        public String changeNumericToWords(double numb)
        {
            String num = numb.ToString();
            return changeToWords(num, false);
        }

        public String changeCurrencyToWords(String numb)
        {
            return changeToWords(numb, true);
        }

        public String changeNumericToWords(String numb)
        {
            Boolean checkPadding = false; ;
            Boolean checkHeader = false; ;
            Int16 step = 0;
            Boolean checkBeginZero = false;
            String result = null;
            result = translateWholeNumber(numb, checkPadding, checkHeader, step, checkBeginZero);
            if (result != null)
            {
                int size = result.Length;
                result = result.Substring(0, 1).ToUpper() + result.Substring(1, size - 1);
                result = result.TrimEnd(',');
            }
            return result;
        }

        public String changeCurrencyToWords(double numb)
        {
            String result = "";
            String numbS = numb.ToString();
            if (numbS.IndexOf('.') != -1) // found '.' char
            {
                String intPart = numbS.Split('.')[0];
                String decimalPart = numbS.Split('.')[1];
                String decimalWords = "";
                String intWords = "";
                if (numb - double.Parse(intWords) != 0)
                {
                    decimalWords = changeNumericToWords(decimalPart).ToLower();
                    intWords = changeNumericToWords(intPart);
                    result = intWords + " phẩy " + decimalWords;
                }
                else
                {
                    intWords = changeNumericToWords(intPart);
                    result = intWords;
                }

            }
            else
            {
                result = changeNumericToWords(numbS);
            }

            return result;
        }

        private String changeToWords(String numb, bool isCurrency)
        {
            String val = "", wholeNo = numb, points = "", pointStr = "";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        pointStr = translateCents(points);
                        val = changeNumericToWords(wholeNo).Trim();
                        val = val + " phẩy " + pointStr;
                    }
                }
                else
                {
                    val = changeNumericToWords(numb).Trim();
                }
            }
            catch {; }
            return val;
        }

        private String translateWholeNumber(String number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(number));                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = number.StartsWith("0");
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    int numDigits = number.Length;
                    switch (numDigits)
                    {
                        case 1://ones' range
                            word = ones(number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " trăm ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " nghìn ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " triệu ";
                            break;
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20://Billions's range
                            pos = (numDigits % 10) + 1;
                            place = " tỷ ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        word = translateWholeNumber(number.Substring(0, pos)) + place + translateWholeNumber(number.Substring(pos));
                        //check for trailing zeros
                        //if (beginsZero) word = " and " + word.Trim();
                        if (beginsZero) word = word.Trim();
                    }
                    //add padding
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch {; }
            return word.Trim();
        }

        private String translateWholeNumber(String number
                                            , Boolean checkPadding
                                            , Boolean checkHeader
                                            , Int16 step
                                            , Boolean chekBeginZero)
        {
            string word = "";
            string paddingWordForOne = " linh ";
            string headerWordForHundred = " không ";
            string oneVal = null;
            string tenVal = null;
            string hundredVal = null;
            try
            {
                oneVal = null;
                tenVal = null;
                hundredVal = null;
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                bool beginZero1 = false; // tests for 0xx (xx != 00)
                double dblAmt = (Convert.ToDouble(number));
                int numDigits = number.Length;
                step++;
                //if ((dblAmt > 0) && number.StartsWith("0"))
                //if ((dblAmt > 0) || ((step != 2) && (dblAmt==0)))
                //{//test for zero or digit zero in a nuemric
                beginsZero = number.StartsWith("0");

                if (numDigits == 3)
                {
                    step = 0;
                    checkPadding = false;
                    checkHeader = false;
                    hundredVal = number.Substring(0, 1);
                    tenVal = number.Substring(1, 1);
                    oneVal = number.Substring(2, 1);
                    beginZero1 = beginsZero && ("0".Equals(tenVal) == false || "0".Equals(oneVal) == false);

                    if ("0".Equals(tenVal) == true
                        && "0".Equals(oneVal) == false)
                    {
                        checkPadding = true;
                    }

                    if ("0".Equals(hundredVal) == true
                        && "0".Equals(tenVal) == true
                        && "0".Equals(oneVal) == false)
                        checkHeader = true;
                }
                int pos = 0;//store digit grouping
                String place = "";//digit grouping name:hundres,thousand,etc...
                switch (numDigits)
                {
                    case 1://ones' range
                        if (step == 1 & checkHeader == true)
                        {
                            word = headerWordForHundred;
                        }
                        else if (step == 3 & checkPadding == true)
                        {
                            word = paddingWordForOne;
                        }
                        else
                        {
                            if (chekBeginZero == false) //03x
                            {
                                word = ones(number);
                            }
                            else
                            { //03x
                                word = "không";
                            }
                        }
                        isDone = true;
                        break;
                    case 2://tens' range
                        word = tens(number);
                        isDone = true;
                        break;
                    case 3://hundreds' range
                        pos = (numDigits % 3) + 1;
                        place = " trăm ";
                        break;
                    case 4://thousands' range
                    case 5:
                    case 6:
                        pos = (numDigits % 4) + 1;
                        place = " nghìn ";
                        break;
                    case 7://millions' range
                    case 8:
                    case 9:
                        pos = (numDigits % 7) + 1;
                        place = " triệu ";
                        break;
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20://Billions's range
                        pos = (numDigits % 10) + 1;
                        place = " tỷ ";
                        break;
                    //add extra case options for anything above Billion...
                    default:
                        isDone = true;
                        break;
                }
                if (!isDone)
                {//if transalation is not done, continue...(Recursion comes in now!!)
                    if (number.Substring(0, pos) == "000")
                    {
                        place = " ";
                    }
                    word = translateWholeNumber(number.Substring(0, pos), checkPadding, checkHeader, step, beginZero1)
                            + place
                            + translateWholeNumber(number.Substring(pos), checkPadding, checkHeader, step, beginZero1);
                    //check for trailing zeros
                    //if (beginsZero) word = " and " + word.Trim();
                    if (beginsZero) word = word.Trim();
                }

                //add padding
                //ignore digit grouping names
                if (word.Trim().Equals(place.Trim())) word = "";
                //}
            }
            catch {; }
            return word.Trim();
        }

        private String tens(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = null;

            switch (digt)
            {
                case 01:
                    name = "linh một";
                    break;
                case 10:
                    name = "mười";
                    break;
                case 11:
                    name = "mười một";
                    break;
                case 12:
                    name = "mười hai";
                    break;
                case 13:
                    name = "mười ba";
                    break;
                case 14:
                    name = "mười bốn";
                    break;
                case 15:
                    name = "mười lăm";
                    break;
                case 25:
                    name = "hai lăm";
                    break;
                case 35:
                    name = "ba lăm";
                    break;
                case 45:
                    name = "bốn lăm";
                    break;
                case 55:
                    name = "năm lăm";
                    break;
                case 65:
                    name = "sáu lăm";
                    break;
                case 75:
                    name = "bẩy lăm";
                    break;
                case 85:
                    name = "tám lăm";
                    break;
                case 95:
                    name = "chín lăm";
                    break;
                case 16:
                    name = "mười sáu";
                    break;
                case 17:
                    name = "mười bẩy";
                    break;
                case 18:
                    name = "mười tám";
                    break;
                case 19:
                    name = "mười chín";
                    break;
                case 20:
                    name = "hai mươi";
                    break;
                case 30:
                    name = "ba mươi";
                    break;
                case 40:
                    name = "bốn mươi";
                    break;
                case 50:
                    name = "năm mươi";
                    break;
                case 60:
                    name = "sáu mươi";
                    break;
                case 70:
                    name = "bẩy mươi";
                    break;
                case 80:
                    name = "tám mươi";
                    break;
                case 90:
                    name = "chín mươi";
                    break;
                case 21:
                    name = "hai mươi mốt";
                    break;
                case 31:
                    name = "ba mươi mốt";
                    break;
                case 41:
                    name = "bốn mươi mốt";
                    break;
                case 51:
                    name = "năm mươi mốt";
                    break;
                case 61:
                    name = "sáu mươi mốt";
                    break;
                case 71:
                    name = "bẩy mươi mốt";
                    break;
                case 81:
                    name = "tám mươi mốt";
                    break;
                case 91:
                    name = "chín mươi mốt";
                    break;
                default:
                    if (digit != null && digit.StartsWith("0") == true && "0".Equals(digit.Substring(1)) == false)
                    {
                        name = " linh " + ones(digit.Substring(1));
                    }
                    else
                    {
                        if (digt > 0)
                        {
                            name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                        }
                    }
                    break;
            }
            name = name == null ? "" : name;
            return name;
        }

        private String ones(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = "";
            switch (digt)
            {
                case 1:
                    name = "một";
                    break;
                case 2:
                    name = "hai";
                    break;
                case 3:
                    name = "ba";
                    break;
                case 4:
                    name = "bốn";
                    break;
                case 5:
                    name = "năm";
                    break;
                case 6:
                    name = "sáu";
                    break;
                case 7:
                    name = "bẩy";
                    break;
                case 8:
                    name = "tám";
                    break;
                case 9:
                    name = "chín";
                    break;
            }
            return name;
        }

        private String translateCents(String cents)
        {
            String cts = "", digit = "", engOne = "";
            for (int i = 0; i < cents.Length; i++)
            {
                digit = cents[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Không";
                }
                else
                {
                    engOne = ones(digit);
                }
                cts += " " + engOne;
            }
            return cts;
        }
    }
