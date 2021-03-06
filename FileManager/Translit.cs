﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{

        public class Translit
        {
            static Dictionary<string, string> dictionaryChar = new Dictionary<string, string>()
            {
                {"Є", "YE"},
                {"І", "I"},
                {"Ѓ", "G"},
                {"і", "i"},
                {"№", "#"},
                {"є", "ye"},
                {"ѓ", "g"},
                {"А", "A"},
                {"Б", "B"},
                {"В", "V"},
                {"Г", "G"},
                {"Д", "D"},
                {"Е", "E"},
                {"Ё", "YO"},
                {"Ж", "ZH"},
                {"З", "Z"},
                {"И", "I"},
                {"Й", "J"},
                {"К", "K"},
                {"Л", "L"},
                {"М", "M"},
                {"Н", "N"},
                {"О", "O"},
                {"П", "P"},
                {"Р", "R"},
                {"С", "S"},
                {"Т", "T"},
                {"У", "U"},
                {"Ф", "F"},
                {"Х", "X"},
                {"Ц", "C"},
                {"Ч", "CH"},
                {"Ш", "SH"},
                {"Щ", "SHH"},
                {"Ъ", "'"},
                {"Ы", "Y"},
                {"Ь", ""},
                {"Э", "E"},
                {"Ю", "YU"},
                {"Я", "YA"},
                {"а", "a"},
                {"б", "b"},
                {"в", "v"},
                {"г", "g"},
                {"д", "d"},
                {"е", "e"},
                {"ё", "yo"},
                {"ж", "zh"},
                {"з", "z"},
                {"и", "i"},
                {"й", "j"},
                {"к", "k"},
                {"л", "l"},
                {"м", "m"},
                {"н", "n"},
                {"о", "o"},
                {"п", "p"},
                {"р", "r"},
                {"с", "s"},
                {"т", "t"},
                {"у", "u"},
                {"ф", "f"},
                {"х", "x"},
                {"ц", "c"},
                {"ч", "ch"},
                {"ш", "sh"},
                {"щ", "shh"},
                {"ъ", ""},
                {"ы", "y"},
                {"ь", ""},
                {"э", "e"},
                {"ю", "yu"},
                {"я", "ya"},
                {"«", ""},
                {"»", ""},
                {"—", "-"},
                {" ", "_"}
            };
            public static string TranslitFileName(string source)
            {
                var result = "";
                foreach (var ch in source)
                {
                    var ss = "";

                    if (dictionaryChar.TryGetValue(ch.ToString(), out ss))
                    {
                        result += ss;
                    }
                    else result += ch;
                }
                return result;
            }
        }
    }

