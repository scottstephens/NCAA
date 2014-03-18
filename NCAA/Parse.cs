using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;

namespace NCAA
{
    public static class Parse
    {
        public static List<Prediction> ScrapedData(string file_name)
        {
            
            var scraped_contents = File.ReadAllText(file_name);
            var document = new HtmlDocument();
            document.LoadHtml(scraped_contents);
            HtmlNode table = document.DocumentNode.SelectSingleNode("//table[@class='desktop']");
            HtmlNode body = table.SelectSingleNode("tbody");

            var data = new List<Prediction>();
            foreach (HtmlNode row in body.SelectNodes("tr"))
            {
                HtmlNodeCollection cells = row.SelectNodes("td");

                var row_data = new Prediction();
                row_data.Region = cells[0].InnerText.Trim();

                var seed_as_string = cells[1].InnerText.Trim();
                var parsed_seed = ParseSeed(seed_as_string);
                row_data.Seed = parsed_seed.Item1;
                row_data.SubSeed = parsed_seed.Item2;
                row_data.Team = cells[3].InnerText.Trim();
                row_data.RoundTwo = parse_percentage(cells[4]);
                row_data.RoundThree = parse_percentage(cells[5]);
                row_data.Sweet16 = parse_percentage(cells[6]);
                row_data.Elite8 = parse_percentage(cells[7]);
                row_data.Final4 = parse_percentage(cells[8]);
                row_data.ChampionshipGame = parse_percentage(cells[9]);
                row_data.Champion = parse_percentage(cells[10]);

                data.Add(row_data);

            }
            return data;
        }

        static Tuple<int, SubSeed> ParseSeed(string s)
        {
            int main_seed;
            string sub_seed;
            if (s.EndsWith("a") || s.EndsWith("b"))
            {
                main_seed = Int32.Parse(s.Substring(0, s.Length - 1));
                sub_seed = s.Substring(s.Length - 1, 1);
            }
            else
            {
                main_seed = Int32.Parse(s);
                sub_seed = "";
            }
            SubSeed sub_seed_parsed;
            switch (sub_seed)
            {
                case "a":
                    sub_seed_parsed = SubSeed.A;
                    break;
                case "b":
                    sub_seed_parsed = SubSeed.B;
                    break;
                case "":
                    sub_seed_parsed = SubSeed.None;
                    break;
                default:
                    throw new Exception("Unexpected subseed value: " + sub_seed);
            }
            return Tuple.Create(main_seed, sub_seed_parsed);
        }
        static double parse_percentage(HtmlNode percentage_cell)
        {
            if (percentage_cell.Attributes["class"].Value == "round win undefined")
            {
                return 1.0;
            }
            else
            {
                var inner_div = percentage_cell.FirstChild.Attributes["title"].Value;
                return (double)(decimal.Parse(inner_div.TrimEnd(new char[] { '%', ' ' })) / 100M);
            }
        }
    }
}
