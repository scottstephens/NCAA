using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;

namespace NCAA
{
    class Program
    {
        static string check_mark = "√";

        static void Main(string[] args)
        {
            var scraped_file = @"../../../data/538_ncaa_2014_predictions_scraped.html";
            var data = Parse.ScrapedData(scraped_file);


        }

        
    }
}
