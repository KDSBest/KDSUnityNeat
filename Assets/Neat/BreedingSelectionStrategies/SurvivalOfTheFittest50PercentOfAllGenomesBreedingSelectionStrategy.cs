using System.Collections.Generic;
using System.Linq;

namespace KDS.Neat.BreedingSelectionStrategies
{
    public class SurvivalOfTheFittest50PercentOfAllGenomesBreedingSelectionStrategy : IBreedingSelectionStrategy, IComparer<Genome>
    {
        public Genome[] SelectParents(IRandomizer randomizer, List<Genome> genomes)
        {
            Genome p1 = GetRandomGenomeBasedOnFitness(randomizer, genomes);
            Genome p2 = GetRandomGenomeBasedOnFitness(randomizer, genomes);
            return new Genome[]
            {
                p1,
                p2
            };
        }

        public void PrepareSelectParents(INeatConfiguration configuration, ISameSpeciesDetectionCalculation distanceFunc, List<Genome> nextGenerationGenomes)
        {
        }

        private Genome GetRandomGenomeBasedOnFitness(IRandomizer randomizer, List<Genome> genomes)
        {
            float completeWeight = 0;

            foreach (var g in genomes)
            {
                completeWeight += g.Fitness;
            }

            float r = randomizer.GetRandomPercentage() * completeWeight;
            float countWeight = 0;

            foreach (var g in genomes)
            {
                countWeight += g.Fitness;

                if (countWeight > r)
                {
                    return g;
                }
            }

            return genomes[genomes.Count - 1];
        }

        public List<Genome> SelectGenomes(List<Species> specieses)
        {
            var selectedGenomes = new List<Genome>();
            foreach (var s in specieses)
            {
                selectedGenomes.AddRange(s.Genomes);
            }

            int survivor = selectedGenomes.Count / 2;
            if (survivor == 0)
            {
                survivor++;
            }

            selectedGenomes.Sort(this);
            if (selectedGenomes.Count > survivor)
            {
                int index = selectedGenomes.Count - survivor;
                selectedGenomes.RemoveRange(index, selectedGenomes.Count - index);
            }

            return selectedGenomes;
        }

        public int Compare(Genome x, Genome y)
        {
            if (x.Fitness > y.Fitness)
            {
                return -1;
            }

            if (y.Fitness > x.Fitness)
            {
                return 1;
            }

            return 0;
        }
    }
}