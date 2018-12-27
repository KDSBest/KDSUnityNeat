using System.Collections.Generic;

namespace KDS.Neat
{
    public interface ISpeciesBreedSelectionStrategy
    {
        List<Genome> SelectGenomes(Species s);
    }
}