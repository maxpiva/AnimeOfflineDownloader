using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADBaseLibrary.Subtitles
{
    public static class Languages
    {
        public static string TranslateToOriginalLanguage(string orig)
        {
            string str = orig.ToLower(CultureInfo.InvariantCulture);
            if (str.Contains("spanish") && str.Contains("latin"))
                return "Español (Latino)";
            if (str.Contains("spanish") && (str.Contains("españa") || str.Contains("spain")))
                return "Español (España)";
            if (str.Contains("spanish"))
                return "Español";
            if (str.Contains("french"))
                return "Français";
            if (str.Contains("german"))
                return "Deutsch";
            if (str.Contains("italian"))
                return "Italiano";
            if (str.Contains("portuguese") && (str.Contains("brasil") || str.Contains("brazil")))
                return "Português (Brasil)";
            if (str.Contains("portuguese") && str.Contains("portugal"))
                return "Português (Portugal)";
            if (str.Contains("portuguese"))
                return "Português";
            if (str.Contains("russian"))
                return "Русский";
            if (str.Contains("arabic"))
                return "العربية";
            if (str.Contains("chinese"))
                return "中文";
            if (str.Contains("japanese"))
                return "日本語";
            if (str.Contains("hindi"))
                return "हिन्दी";
            if (str.Contains("korean"))
                return "한국어";
            if (str.Contains("thai"))
                return "ไทย";
            return orig;
        }

        public static string CodeFromLanguage(string str)
        {
            str = str.ToLower(CultureInfo.InvariantCulture);
            if (str.Contains("eng"))
                return "eng";
            if (str.Contains("spa"))
                return "spa";
            if (str.Contains("fre"))
                return "fra";
            if (str.Contains("ger"))
                return "deu";
            if (str.Contains("ita"))
                return "ita";
            if (str.Contains("por"))
                return "por";
            if (str.Contains("rus"))
                return "rus";
            if (str.Contains("ara"))
                return "ara";
            if (str.Contains("chi"))
                return "zho";
            if (str.Contains("hindi"))
                return "hin";
            if (str.Contains("jap"))
                return "jpn";
            if (str.Contains("kor"))
                return "kor";
            if (str.Contains("tha"))
                return "tha";
            return "unk";
        }

        public static string CodeFromOriginalLanguage(string language)
        {
            string str = language.ToLower(CultureInfo.InvariantCulture);

            if (str.Contains("english"))
                return "eng";
            if (str.Contains("espa"))
                return "spa";
            if (str.Contains("por"))
                return "por";
            if (str.Contains("fra"))
                return "fra";
            if (str.Contains("deu"))
                return "deu";
            if (str.Contains("ara") || language.Contains("العربية"))
                return "ara";
            if (str.Contains("ita"))
                return "ita";
            if (str.Contains("rus") || language.Contains("Русский") || language.Contains("pусский"))
                return "rus";
            if (str.Contains("chi") || language.Contains("中文") || language.Contains("汉语") || language.Contains("漢語"))
                return "zho";
            if (str.Contains("hin") || language.Contains("हिन्दी") || language.Contains("हिंदी"))
                return "hin";
            if (str.Contains("jap") || language.Contains("日本語") || language.Contains("にほんご"))
                return "jpn";
            if (str.Contains("kor") || language.Contains("한국어") || language.Contains("조선어"))
                return "kor";
            if (str.Contains("tha") || language.Contains("ไทย"))
                return "tha";
            return "unk";
        }


    }
}
