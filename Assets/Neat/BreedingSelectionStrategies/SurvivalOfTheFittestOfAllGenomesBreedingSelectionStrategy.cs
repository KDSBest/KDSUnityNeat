using System.Collections.Generic;
using System.Linq;

namespace KDS.Neat.BreedingSelectionStrategies
{
    public class SurvivalOfTheFittestOfAllGenomesBreedingSelectionStrategy : IBreedingSelectionStrategy, IComparer<Genome>
    {
        public int Count { get; set; }

        public SurvivalOfTheFittestOfAllGenomesBreedingSelectionStrategy() : this(10)
        {

        }

        public SurvivalOfTheFittestOfAllGenomesBreedingSelectionStrategy(int count)
        {
            this.Count = count;
        }

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

        public void PrepareSelectParents(INeatConfiguration configuration, ISameSpeciesDetectionCalculation distanceFunc, List<Genome> nextGenerationGenomes)
        {
        }

        public List<Genome> SelectGenomes(List<Species> specieses)
        {
            var selectedGenomes = new List<Genome>();
            foreach (var s in specieses)
            {
                selectedGenomes.AddRange(s.Genomes);
            }

            selectedGenomes.Sort(this);

            var result = new List<Genome>();

            for (int i = 0; i < Count && i < selectedGenomes.Count; i++)
            {
                result.Add(selectedGenomes[i]);
            }

            return result;
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