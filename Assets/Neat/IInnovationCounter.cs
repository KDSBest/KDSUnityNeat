namespace KDS.Neat
{
    public interface IInnovationCounter
    {
        int GetConnectionInnovation();
        int GetNodeGeneInnovation();
        int GetGenomeInnovation();
    }
}