using System.Collections.Generic;
using System.Linq;

namespace KDS.Neat.KillGenomesStrategies
{
    public class SurvivalOfTheFittest50PercentBreedSelectionStrategy : ISpeciesBreedSelectionStrategy, IComparer<Genome>
    {
        public List<Genome> SelectGenomes(Species s)
        {
            s.Genomes.Sort(this);

            int c = s.Genomes.Count / 2;

            if (c == 0)
            {
                c++;
            }

            var selectedGenomes = new List<Genome>(c);

            for (int i = 0; i < c; i++)
            {
                selectedGenomes.Add(s.Genomes[i]);
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