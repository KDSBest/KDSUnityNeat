namespace KDS.Neat
{
    public interface ISameSpecialCalculation
    {
        bool IsSameSpecies(INeatConfiguration configuration, Genome genome1, Genome genome2);
    }
}