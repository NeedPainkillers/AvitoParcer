using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AvitoParcer
{

    static class By
    {
        //public static string itemProp = "itemprop\\s*=\\s*\"name\".*?>(.*?)<";
        //public static string className = "class\s*=\s*\"(?:price|price+\s.*?)\".*?>(.*?)<";
        //public static string parametr = "data-absolute-date\\s*=\\s*\"(.*?)\"";
        //class\s*=\s*\"(?:item item_table|item item_table+\s.*?)\".*?>.*?(?:div class="js-item-date c-2|div class="js-item-date c-2+\s.*?).*?</div>

        public static string ItemProp(string itemProp)
        {
            return "itemprop\\s*=\\s*\"(?:" + itemProp + "|" + itemProp + "+\\s.*?)\".*?>(.*?)<";
        }
        public static string ClassName(string className)
        {
            return "class\\s*=\\s*\"(?:" + className + "|" + className + "+\\s.*?)\".*?>(.*?)<";
        }
        public static string Parametr(string parametr)
        {
            return parametr + "\\s*=\\s*\"(.*?)\"";
        }
        public static string regexForPosts = "class\\s*=\\s*\"(?:item item_table|item item_table+\\s.*?)\".*?>.*?(?:div class=\"js-item-date c-2|div class=\"js-item-date c-2+\\s.*?).*?</div>";
        public static string regexForImageUrls = "style=\"background-image: url\\(//(.*?)\\)\"" ;
    }
    static class Parce
    {
        //private static int FindStartInd(string data, string subStr)
        //{
        //    return (data.IndexOf(subStr) + subStr.Length);
        //}
        //private static int FindStartInd(string data, string subStr, int indexStart)
        //{
        //    return (data.IndexOf(subStr, indexStart) + subStr.Length);
        //}
        //private static int FindEndInd(string data, string subStr, int indexStart)
        //{
        //    return data.IndexOf(subStr, indexStart);
        //}
        //private static string FormSubString(string data, int indexStart, int indexEnd)
        //{
        //    return data.Substring(indexStart, indexEnd - indexStart);
        //}
        //public static string FindSubString(string data, string start, string end)
        //{
        //    int indexStart = FindStartInd(data, start);
        //    return FormSubString(data, indexStart, FindEndInd(data, end, indexStart));
        //}

        //public static GroupCollection FindElements(string data, string regex)
        //{
        //    return Regex.Match(data, regex, RegexOptions.Singleline).Groups;
        //}
        public static GroupCollection FindElement(this String data, string regex)
        {
            return Regex.Matches(data, regex, RegexOptions.Singleline)[0].Groups;
        }
        public static MatchCollection FindElements(this String data, string regex)
        {
            return Regex.Matches(data, regex, RegexOptions.Singleline);
        }
    }



    class AvitoPost
    {
        string name;
        string price;
        string additionalInfo;
        string date;
        string link;
        List<string> imgLinks;
        

        public string Name { get => name; set => name = value; }
        public string Price { get => price; set => price = value; }
        public string AdditionalInfo { get => additionalInfo; set => additionalInfo = value; }
        public string Date { get => date; set => date = value; }
        public List<string> ImgLinks { get => imgLinks; set => imgLinks = value; }
        public string Link { get => link; set => link = value; }

        public static AvitoPost FormObject(string data)
        {
            return new AvitoPost
            {
                Name = data.FindElement(By.ItemProp("name"))[1].Value,
                Price = data.FindElement(By.ItemProp("price"))[1].Value
                      + Parce.FindElement(data, By.ItemProp("priceCurrency"))[0].Value.FindElement(By.Parametr("content"))[1].Value,
                AdditionalInfo = data.FindElement(By.ClassName("specific-params"))[1].Value,
                Date = data.FindElement(By.ClassName("js-item-date"))[0].Value.FindElement(By.Parametr("data-absolute-date"))[1].Value,
                Link = @"https://www.avito.ru" + data.FindElement(By.ClassName("js-item-slider"))[0].Value.FindElement(By.Parametr("href"))[1].Value,
                ImgLinks = (from Match arr in data.FindElements(By.regexForImageUrls)
                            select arr.Groups[1].Value).ToList(),
            };
        }
    }


    

}


//dead zone
//int indexStart = 0, indexEnd = 0;

//string startSubStr = "itemprop=\"name\">";
//string endSubStr = "<";

//indexStart = data.IndexOf(startSubStr) + startSubStr.Length;
//indexEnd = data.IndexOf(endSubStr, indexStart);

//indexStart = FindStartInd(data, "itemprop=\"name\">");
//indexEnd = FindEndInd(data, "<", indexStart);
//item.Name = FormSubString(data, indexStart, indexEnd);