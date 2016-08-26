using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAA
{

    public enum Round { First, Second, Third, Sweet16, Elite8, Final4, Championship, Champion }

    public static class RoundExtensions
    {
        public static Round Previous(this Round input)
        {
            return (Round)(((int)input) - 1);
        }

        public static Round Next(this Round input)
        {
            return (Round)(((int)input) + 1);
        }
    }

    public interface IBracketNode
    {
        IEnumerable<Prediction> PossibleTeams { get; }
        int NumPossibleTeams { get; }
        TeamBracketNode AsTeamNode { get; }
    }
    public class BasicBracketNode : IBracketNode
    {
        public IBracketNode QualifyingNode1 { get; set; }
        public IBracketNode QualifyingNode2 { get; set; }

        public IEnumerable<Prediction> PossibleTeams { get { return this.QualifyingNode1.PossibleTeams.Concat(this.QualifyingNode2.PossibleTeams); } }
        public int NumPossibleTeams { get { return this.QualifyingNode1.NumPossibleTeams + this.QualifyingNode2.NumPossibleTeams; } }

        public TeamBracketNode AsTeamNode { get { return null; } }

        public BasicBracketNode(IBracketNode qn1, IBracketNode qn2)
        {
            this.QualifyingNode1 = qn1;
            this.QualifyingNode2 = qn2;
        }
    }

    public interface IDeterminedBracketNode : IBracketNode
    {
        string WinningTeam { get; }
    }

    public class DeterminedBracketNode : IDeterminedBracketNode
    {
        public string WinningTeam { get; set; }
        public string LosingTeam { get { return this.DeterminedQualifyingNode1.WinningTeam == this.WinningTeam ? this.DeterminedQualifyingNode2.WinningTeam : this.DeterminedQualifyingNode1.WinningTeam; } }
        public IDeterminedBracketNode DeterminedQualifyingNode1 { get; set; }
        public IDeterminedBracketNode DeterminedQualifyingNode2 { get; set; }

        public DeterminedBracketNode(string team, DeterminedBracketNode qn1, DeterminedBracketNode qn2)
        {
            this.WinningTeam = team;
            this.DeterminedQualifyingNode1 = qn1;
            this.DeterminedQualifyingNode2 = qn2;
        }

        public IEnumerable<Prediction> PossibleTeams
        {
            get { return this.DeterminedQualifyingNode1.PossibleTeams.Concat(this.DeterminedQualifyingNode2.PossibleTeams); }
        }

        public int NumPossibleTeams
        {
            get { return this.DeterminedQualifyingNode1.NumPossibleTeams + this.DeterminedQualifyingNode2.NumPossibleTeams; }
        }
    }

    public class TeamBracketNode : IBracketNode, IDeterminedBracketNode
    {
        public Prediction Prediction { get; set; }


        public IEnumerable<Prediction> PossibleTeams
        {
            get { yield return this.Prediction; } 
        }

        public int NumPossibleTeams
        {
            get { return 1; }
        }

        public string WinningTeam
        {
            get { return this.Prediction.Team; }
        }

        public TeamBracketNode AsTeamNode { get { return this; } }
    }
    
    public class Bracket
    {
        public BasicBracketNode ChampionshipNode { get; set; }

        public static BasicBracketNode BuildChampionshipNode(List<Prediction> predictions)
        {

            var south_tourney = BuildRegion("South", predictions);
            var east_tourney = BuildRegion("East", predictions);
            var west_tourney = BuildRegion("West", predictions);
            var midwest_tourney = BuildRegion("Midwest", predictions);

            var south_west_semi = new BasicBracketNode(south_tourney, west_tourney);
            var east_midwest_semi = new BasicBracketNode(east_tourney, midwest_tourney);
            var final = new BasicBracketNode(south_west_semi, east_midwest_semi);
            return final;
        }

        public static BasicBracketNode BuildRegion(string region, List<Prediction> predictions)
        {
            List<BasicBracketNode> second_round = new List<BasicBracketNode>(8);

            second_round.Add(BuildSecondRound(1, 16, region, predictions));
            second_round.Add(BuildSecondRound(8, 9, region, predictions));
            second_round.Add(BuildSecondRound(5, 12, region, predictions));
            second_round.Add(BuildSecondRound(4, 13, region, predictions));
            second_round.Add(BuildSecondRound(6, 11, region, predictions));
            second_round.Add(BuildSecondRound(3, 14, region, predictions));
            second_round.Add(BuildSecondRound(7, 10, region, predictions));
            second_round.Add(BuildSecondRound(2, 15, region, predictions));

            List<BasicBracketNode> third_round = new List<BasicBracketNode>(4);
            for (int ii = 0; ii < 4; ++ii)
            {
                third_round.Add(new BasicBracketNode(second_round[2 * ii], second_round[2 * ii + 1]));
            }

            List<BasicBracketNode> sweet_16 = new List<BasicBracketNode>(2);
            sweet_16.Add(new BasicBracketNode(third_round[0], third_round[1]));
            sweet_16.Add(new BasicBracketNode(third_round[2], third_round[3]));

            var region_championship_game = new BasicBracketNode(sweet_16[0], sweet_16[1]);

            return region_championship_game;
            
        }

        public static BasicBracketNode BuildSecondRound(int seed1, int seed2, string region, List<Prediction> prediction)
        {
            var seed1_teams = prediction.Where(x => x.Region == region && x.Seed == seed1).ToList();
            var seed2_teams = prediction.Where(x => x.Region == region && x.Seed == seed2).ToList();

            IBracketNode node1 = NodeFromTeams(seed1_teams);
            IBracketNode node2 = NodeFromTeams(seed2_teams);

            return new BasicBracketNode(node1, node2);

        }

        public static IBracketNode NodeFromTeams(List<Prediction> seed1_teams)
        {
            IBracketNode node1;
            if (seed1_teams.Count == 1)
            {
                node1 = new TeamBracketNode() { Prediction = seed1_teams[0] };
            }
            else
            {
                node1 = new BasicBracketNode(new TeamBracketNode() { Prediction = seed1_teams[0] }, new TeamBracketNode() { Prediction = seed1_teams[1] });
            }
            return node1;
        }
    }
}
