using System.Collections.Generic;
using System.Linq;

namespace KDS.Neat.BreedingSelectionStrategies
{
    public class SurvivalOfTheFittestBreedingSelectionStrategy : IBreedingSelectionStrategy, IComparer<Genome>
    {
        private List<Species> specieses = new List<Species>();
        private float completeWeightForSpecies = 0.0f;

        public Genome[] SelectParents(IRandomizer randomizer, List<Genome> genomes)
        {
            Species s1 = GetRandomSpeciesBasedOnFitness(randomizer, specieses, completeWeightForSpecies);
            Species s2 = GetRandomSpeciesBasedOnFitness(randomizer, specieses, completeWeightForSpecies);

            Genome p1 = GetRandomGenomeBasedOnFitness(randomizer, s1);
            Genome p2 = GetRandomGenomeBasedOnFitness(randomizer, s2);
            return new Genome[]
            {
                p1,
                p2
            };
        }

        public void PrepareSelectParents(INeatConfiguration configuration, ISameSpeciesDetectionCalculation distanceFunc, List<Genome> nextGenerationGenomes)
        {
            List<Species> specieses = new List<Species>();
            Species.SortGenomesIntoSpecies(configuration, distanceFunc, nextGenerationGenomes, specieses);

            float completeWeightForSpecies = 0;
            foreach (var s in specieses)
            {
                s.CalculateFitness();
                completeWeightForSpecies += s.Fitness;
            }
        }

        private Genome GetRandomGenomeBasedOnFitness(IRandomizer randomizer, Species s)
        {
            float completeWeight = 0;

            foreach (var g in s.Genomes)
            {
                completeWeight += g.Fitness;
            }

            float r = randomizer.GetRandomPercentage() * completeWeight;
            float countWeight = 0;

            foreach (var g in s.Genomes)
            {
                countWeight += g.Fitness;

                if (countWeight > r)
                {
                    return g;
                }
            }

            return s.Genomes[s.Genomes.Count - 1];
        }

        private Species GetRandomSpeciesBasedOnFitness(IRandomizer randomizer, List<Species> specieses, float completeWeight)
        {
            float r = randomizer.GetRandomPercentage() * completeWeight;
            float countWeight = 0;

            foreach (var s in specieses)
            {
                countWeight += s.Fitness;

                if (countWeight >= r)
                {
                    return s;
                }
            }

            return specieses[specieses.Count - 1];
        }

        public List<Genome> SelectGenomes(List<Species> specieses)
        {
            var selectedGenomes = new List<Genome>();
            foreach (var s in specieses)
            {
                s.Genomes.Sort(this);

                selectedGenomes.Add(s.Genomes[0]);
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