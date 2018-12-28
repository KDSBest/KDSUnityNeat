using System;
using System.Linq;

namespace KDS.Neat.Defaults
{
    public class DefaultSameSpeciesDetectionCalculation : ISameSpeciesDetectionCalculation
    {
        public bool IsSameSpecies(INeatConfiguration configuration, Genome genome1, Genome genome2)
        {
            int excessGenes = 0;
            int disjoniedGenes = 0;
            float distanceGenes = 0;
            int matchingGenes = 0;
            var genome1Connections = genome1.GetConnections();
            var genome2Connections = genome2.GetConnections();
            int numberOfNodes = Math.Max(genome1Connections.Count, genome2Connections.Count);
            if (numberOfNodes < configuration.SameSpeciesNodeIgnoreValue)
            {
                numberOfNodes = 1;
            }

            int highestInnovation1 = genome1Connections.Keys.Max();
            int highestInnovation2 = genome2Connections.Keys.Max();
            int highestInnovation = Math.Max(highestInnovation1, highestInnovation2);

            for (int i = 1; i <= highestInnovation; i++)
            {
                bool hasGene1 = genome1Connections.ContainsKey(i);
                bool hasGene2 = genome2Connections.ContainsKey(i);

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
                else // both Genomes have a genes
                {
                    matchingGenes++;
                    distanceGenes += Math.Abs(genome1Connections[i].Weight - genome2Connections[i].Weight);
                }
            }

            return (excessGenes * configuration.ExcessCoeff / numberOfNodes +
                   disjoniedGenes * configuration.DisjoinedCoeff / numberOfNodes +
                   configuration.DistanceCoeff * distanceGenes / matchingGenes) < configuration.SameSpeciesDistance;
        }
    }
}