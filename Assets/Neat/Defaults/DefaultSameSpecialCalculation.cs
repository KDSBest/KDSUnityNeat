using System;
using System.Linq;

namespace KDS.Neat.Defaults
{
    public class DefaultSameSpecialCalculation : ISameSpecialCalculation
    {
        public bool IsSameSpecies(INeatConfiguration configuration, Genome genome1, Genome genome2)
        {
            int excessGenes = 0;
            int disjoniedGenes = 0;
            float distanceGenes = 0;
            int matchingGenes = 0;
            int numberOfNodes = Math.Max(genome1.Connections.Count, genome2.Connections.Count);
            if (numberOfNodes < 20)
            {
                numberOfNodes = 1;
            }
            int highestInnovation1 = genome1.Connections.Keys.Max();
            int highestInnovation2 = genome2.Connections.Keys.Max();
            int highestInnovation = Math.Max(highestInnovation1, highestInnovation2);

            for (int i = 1; i <= highestInnovation; i++)
            {
                bool hasGene1 = genome1.Connections.ContainsKey(i);
                bool hasGene2 = genome2.Connections.ContainsKey(i);

                if (!hasGene1 && !hasGene2)
                {
                    continue;
                }
                else if (!hasGene1)
                {
                    if (i > highestInnovation1)
                    {
                        excessGenes++;
                    }
                    else
                    {
                        disjoniedGenes++;
                    }
                }
                else if (!hasGene2)
                {
                    if (i > highestInnovation2)
                    {
                        excessGenes++;
                    }
                    else
                    {
                        disjoniedGenes++;
                    }
                }
                else // both Genes true
                {
                    matchingGenes++;
                    distanceGenes += Math.Abs(genome1.Connections[i].Weight - genome2.Connections[i].Weight);
                }
            }

            return (excessGenes * configuration.ExcessCoeff / numberOfNodes +
                   disjoniedGenes * configuration.DisjoinedCoeff / numberOfNodes +
                   configuration.DistanceCoeff * distanceGenes / matchingGenes) < configuration.SameSpeciesDistance;
        }
    }
}