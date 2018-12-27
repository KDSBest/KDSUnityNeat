namespace KDS.Neat
{
    public class InnovationCounter : IInnovationCounter
    {
        private int currentConnectionInnovation = 0;
        private int currentNodeGeneInnovation = 0;
        private int currentGenomeInnovation = 0;

        public int GetConnectionInnovation()
        {
            currentConnectionInnovation++;

            return currentConnectionInnovation;
        }

        public int GetNodeGeneInnovation()
        {
            currentNodeGeneInnovation++;

            return currentNodeGeneInnovation;
        }

        public int GetGenomeInnovation()
        {
            currentGenomeInnovation++;

            return currentGenomeInnovation;
        }
    }
}