using System.Collections.Generic;
using System.Linq;

namespace KDS.Neat.KillGenomesStrategies
{
    public class SurvivalOfTheFittestBreedSelectionStrategy : ISpeciesBreedSelectionStrategy, IComparer<Genome>
    {
        public List<Genome> SelectGenomes(Species s)
        {
            s.Genomes.Sort(this);

            return new List<Genome>()
            {
                s.Genomes[0]
            };
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