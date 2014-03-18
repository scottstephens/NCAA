using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAA
{

    public enum SubSeed { None,A, B }

    public class Prediction
    {
        public string Region { get; set; }
        public int Seed { get; set; }
        public SubSeed SubSeed { get; set; }
        public string Team { get; set; }

        public double RoundTwo { get; set; }
        public double RoundThree { get; set; }
        public double Sweet16 { get; set; }
        public double Elite8 { get; set; }
        public double Final4 { get; set; }
        public double ChampionshipGame { get; set; }
        public double Champion { get; set; }

        public double getProbability(Round round)
        {
            switch (round)
            {
                case Round.First:
                    return 1.0;
                case Round.Second:
                    return this.RoundTwo;
                case Round.Third:
                    return this.RoundThree;
                case Round.Sweet16:
                    return this.Sweet16;
                case Round.Elite8:
                    return this.Elite8;
                case Round.Final4:
                    return this.Final4;
                case Round.Championship:
                    return this.ChampionshipGame;
                case Round.Champion:
                    return this.Champion;
                default:
                    throw new Exception("Bad Round value: " + round.ToString());
            }
        }
    }
}
