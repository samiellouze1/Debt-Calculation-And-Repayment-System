using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace Debt_Calculation_And_Repayment_System
{
    public class DataReader
    {
        [DebuggerNonUserCode]
        public static string GetString(object input)
        {
            try
            {
                return (input.ToString());
            }
            catch { return ""; }
        }
        [DebuggerNonUserCode]
        public static string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        [DebuggerNonUserCode]
        public static Guid GetGuid(object input)
        {
            try
            {
                return Guid.Parse(GetString(input));
            }
            catch { return Guid.NewGuid(); }
        }
        [DebuggerNonUserCode]
        public static string GetString(object input, bool dbCheck)
        {
            try
            {
                string str = GetString(input);
                return str;
                //return str.Replace("'", "''").Replace("\\", "").Replace("/*", "").Replace("*/", "");
            }
            catch { return ""; }
        }
        [DebuggerNonUserCode]
        public static bool GetBoolean(object input)
        {
            try { return (Convert.ToBoolean(input)); }
            catch { return false; }
        }
        [DebuggerNonUserCode]
        public static bool? GetBooleanNullable(object input)
        {
            try
            {
                return (Convert.ToBoolean(input));
            }
            catch
            {
                return null;
            }
        }
        [DebuggerNonUserCode]
        public static Int32 GetInt32(object input)
        {
            try { return Int32.Parse(input.ToString()); }
            catch { return 0; }
        }
        [DebuggerNonUserCode]
        public static Int32 GetInt32(object input, int defaultValue)
        {
            try { return Int32.Parse(input.ToString()); }
            catch { return defaultValue; }
        }
        [DebuggerNonUserCode]
        public static Int64 GetInt64(object input)
        {
            try { return Int64.Parse(input.ToString()); }
            catch { return 0; }
        }
        [DebuggerNonUserCode]
        public static Double GetDouble(object input)
        {
            try { return Convert.ToDouble(input.ToString()); }
            catch { return 0; }
        }
        [DebuggerNonUserCode]
        public static DateTime GetDateTime(object input)
        {
            try { return (DateTime.Parse(input.ToString())); }
            catch
            {
                return GetDateTimeNOW();
            }
        }
        //[DebuggerNonUserCode]
        public static DateTime GetDateTimeWithFormat(object input, string format)
        {
            try
            {
                return DateTime.ParseExact(input.ToString(), format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return GetDateTimeNOW();
            }
        }
        //[DebuggerNonUserCode]
        public static DateTime? GetDateTimeNullable(object input)
        {
            try { return (DateTime.Parse(input.ToString())); }
            catch
            {
                return null;
            }
        }
        [DebuggerNonUserCode]
        public static DateTime? GetDateTimeNullableWithFormat(object input, string format)
        {
            try
            {
                return DateTime.ParseExact(input.ToString(), format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetDateTimeNOW()
        {
            return (DateTime.Now);
        }
        [DebuggerNonUserCode]
        public static Decimal GetDecimal(object input)
        {
            try { return Decimal.Parse(input.ToString()); }
            catch { return 0; }
        }
        [DebuggerNonUserCode]
        public static float GetFloat(object input)
        {
            try { return float.Parse(input.ToString()); }
            catch { return 0; }
        }

        public static bool IsEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }


        public static string GetArg(string query, string key)
        {
            Match x = Regex.Match(query, key + "=([^&#]*)");
            return x.Groups[1].Value;
        }

        public static string GetTCKimlikPerdele(string TcKimlikNo, int baslangicNoktasi = 4, int alinacakAdet = 3)
        {
            string result = "";

            if (TcKimlikNo != null)
            {
                string baslangic = "";
                string bitis = "";
                baslangic = baslangic.PadLeft(baslangicNoktasi, '*');
                result = TcKimlikNo.Substring(baslangicNoktasi, alinacakAdet);
                bitis = baslangic.PadLeft(TcKimlikNo.Length - (baslangic.Length + alinacakAdet), '*');
                result = baslangic + result + bitis;
                return result;
            }
            else
            {
                return result;
            }

        }
        public static string GetIsimSoyIsimPerdele(string Ad, string Soyad, int alinacakAdet = 3)
        {
            if (Ad.Length < 3)
            {
                return "";
            }
            var adBolunmus = Ad.Split(' ');
            string result = "";
            foreach (var element in adBolunmus)
            {
                if (!string.IsNullOrEmpty(element))
                {
                    if (element.Length >= alinacakAdet)
                    {
                        result = result + element.Substring(0, alinacakAdet) + "*** ";
                    }
                }
            }
            string soyad;
            if (Soyad.Length > 2)
            {
                soyad = Soyad.Substring(Soyad.Length - (alinacakAdet), alinacakAdet);
            }
            else
            {
                soyad = Soyad;
            }

            return result + "***" + soyad;
        }

        public static long GetIDinPostResult(string input)
        {
            long result;
            string resultString = Regex.Match(input, @"\d+").Value;
            result = DataReader.GetInt64(resultString);
            return result;
        }


    }
}
