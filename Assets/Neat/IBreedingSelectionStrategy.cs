using System.Collections.Generic;

namespace KDS.Neat
{
    public interface IBreedingSelectionStrategy
    {
        List<Genome> SelectGenomes(List<Species> s);
        Genome[] SelectParents(IRandomizer randomizer, List<Genome> genomes);

        void PrepareSelectParents(INeatConfiguration configuration, ISameSpeciesDetectionCalculation distanceFunc,
            List<Genome> nextGenerationGenomes);
    }
}