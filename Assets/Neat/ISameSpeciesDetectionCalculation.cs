namespace KDS.Neat
{
    public interface ISameSpeciesDetectionCalculation
    {
        bool IsSameSpecies(INeatConfiguration configuration, Genome genome1, Genome genome2);
    }
}