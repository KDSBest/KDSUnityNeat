using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using KDS.Neat.BreedingSelectionStrategies;
using KDS.Neat.Defaults;

namespace KDS.Neat
{
    public class NeatNetwork
    {
        private readonly IInnovationCounter innovationCounter;
        private readonly IRandomizer randomizer;
        public readonly INeatConfiguration Configuration;
        private readonly ISameSpeciesDetectionCalculation distanceFunc;
        private readonly IBreedingSelectionStrategy speciesBreedSelectionStrategy;
        public int CurrentGeneration = 1;
        public List<Genome> Genomes;

        public List<Species> Specieses;

        public Genome FittestGenome;

        public NeatNetwork(int inNodes, int outNodes) : this(new DefaultNeatConfiguration(), new InnovationCounter(),
            new DefaultRandomizer(), new DefaultSameSpeciesDetectionCalculation(),
            new SurvivalOfTheFittest50PercentOfAllGenomesBreedingSelectionStrategy(), inNodes, outNodes)
        {

        }

        public NeatNetwork(INeatConfiguration configuration, IInnovationCounter innovationCounter, IRandomizer randomizer, ISameSpeciesDetectionCalculation distanceFunc, IBreedingSelectionStrategy speciesBreedSelectionStrategy, int inNodes, int outNodes)
        {
            this.Configuration = configuration;
            this.innovationCounter = innovationCounter;
            this.randomizer = randomizer;
            this.distanceFunc = distanceFunc;
            this.speciesBreedSelectionStrategy = speciesBreedSelectionStrategy;
            Genomes = new List<Genome>();
            Specieses = new List<Species>();

            var startingGenome = GenerateStartingGenome(inNodes, outNodes);

            Genomes.Add((Genome)startingGenome.Clone());
            Specieses.Add(new Species(Genomes[0]));

            if (Genomes.Count < configuration.PopulationSize)
            {
                BreedNextGeneration();
            }
        }

        private Genome GenerateStartingGenome(int inNodes, int outNodes)
        {
            Genome startingGenome = new Genome()
            {
                Id = innovationCounter.GetGenomeInnovation()
            };
            List<NodeGene> inNodesTemp = new List<NodeGene>();
            for (int i = 0; i < inNodes; i++)
            {
                int id = this.innovationCounter.GetNodeGeneInnovation();
                var newNode = new NodeGene(NodeGeneType.Input, id);
                startingGenome.Nodes.Add(id, newNode);
                inNodesTemp.Add(newNode);
            }

            for (int i = 0; i < outNodes; i++)
            {
                int id = this.innovationCounter.GetNodeGeneInnovation();
                startingGenome.Nodes.Add(id, new NodeGene(NodeGeneType.Output, id));

                for (int inIndex = 0; inIndex < inNodes; inIndex++)
                {
                    int conId = this.innovationCounter.GetConnectionInnovation();
                    startingGenome.AddConnection(new ConnectionGene(inNodesTemp[inIndex].Id, id, 0.5f, true, conId));
                }
            }

            return startingGenome;
        }

        public void Evaluate(Action<List<Genome>> calculateFitnessForGenomes)
        {
            Reset(randomizer);

            Species.SortGenomesIntoSpecies(Configuration, distanceFunc, Genomes, Specieses);

            calculateFitnessForGenomes(Genomes);

            float highestFitness = float.MinValue;
            foreach (var g in Genomes)
            {
                if (g.Fitness > highestFitness)
                {
                    highestFitness = g.Fitness;
                    FittestGenome = g;
                }
            }

        }

        public void BreedNextGeneration()
        {
            List<Genome> nextGenerationGenomes = new List<Genome>();

            nextGenerationGenomes.AddRange(speciesBreedSelectionStrategy.SelectGenomes(Specieses));
            CurrentGeneration++;
            Genomes = BreedGenomes(nextGenerationGenomes);
        }

        private void Reset(IRandomizer randomizer)
        {
            foreach (var s in Specieses)
            {
                s.Reset(randomizer);
            }
        }

        private List<Genome> BreedGenomes(List<Genome> nextGenerationGenomes)
        {
            List<Genome> breededGenomes = new List<Genome>(nextGenerationGenomes);

            this.speciesBreedSelectionStrategy.PrepareSelectParents(Configuration, distanceFunc, nextGenerationGenomes);

            while (breededGenomes.Count < Configuration.PopulationSize)
            {
                var parents = this.speciesBreedSelectionStrategy.SelectParents(randomizer, nextGenerationGenomes);

                var p1 = parents[0];
                var p2 = parents[1];

                Genome child = p1.Crossover(Configuration, randomizer, innovationCounter, p2);
                child.Generation = CurrentGeneration;

                if (randomizer.GetRandomPercentage() <= Configuration.MutationRate)
                {
                    child.Mutation(Configuration, randomizer);
                }

                if (randomizer.GetRandomPercentage() <= Configuration.AddConnectionRate)
                {
                    child.AddConnectionMutation(Configuration, randomizer, innovationCounter);
                }

                if (randomizer.GetRandomPercentage() <= Configuration.AddNodeRate)
                {
                    child.AddNodeMutation(randomizer, innovationCounter);
                }

                breededGenomes.Add(child);
            }

            return breededGenomes;
        }
    }
}
