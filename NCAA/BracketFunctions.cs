using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAA
{
    public class BracketFunctions
    {

        public static DeterminedBracketNode MostProbableBracket(BasicBracketNode championship_node)
        {

            throw new NotImplementedException();
        }

        public static double ProbabilityOfBracket(IDeterminedBracketNode node, Round round)
        {
            if (node.AsTeamNode != null)
                return 1.0;
            else 
            {
                var tnode = (DeterminedBracketNode)node;

                // probability of first team
                var team1_prob = ProbabilityOfBracket(tnode.DeterminedQualifyingNode1, round.Previous());

                // probability of second team
                var team2_prob = ProbabilityOfBracket(tnode.DeterminedQualifyingNode2, round.Previous());

                // Probability of getting this game
                var game_probability = team1_prob*team2_prob;


                // Probability of winner winning, given this game

                return 
            }
        }

        public static double LogProbabilityOfBracket(IDeterminedBracketNode node)
        {
            if (node.AsTeamNode != null)
                return 0.0;
            else
            {
                var tnode = (DeterminedBracketNode)node;
                return LogProbabilityOfBracket(tnode.DeterminedQualifyingNode1) + LogProbabilityOfBracket(tnode.DeterminedQualifyingNode2);
            }
        }

        //public static double ProbabilityOf
    }
}
