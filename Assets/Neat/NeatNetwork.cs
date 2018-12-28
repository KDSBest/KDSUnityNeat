using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDS.Neat.Defaults;
using KDS.Neat.KillGenomesStrategies;
using UnityEngine.Experimental.PlayerLoop;

namespace KDS.Neat
{
    public class NeatNetwork
    {
        private readonly IInnovationCounter innovationCounter;
        private readonly IRandomizer randomizer;
        public readonly INeatConfiguration Configuration;
        private readonly ISameSpeciesDetectionCalculation distanceFunc;
        private readonly ISpeciesBreedSelectionStrategy speciesBreedSelectionStrategy;
        public int CurrentGeneration = 1;
        public List<Genome> Genomes;

        public List<Species> Species;

        public Genome FittestGenome;

        public NeatNetwork(int inNodes, int outNodes) : this(new DefaultNeatConfiguration(), new InnovationCounter(),
            new DefaultRandomizer(), new DefaultSameSpeciesDetectionCalculation(),
            new SurvivalOfTheFittestBreedSelectionStrategy(), inNodes, outNodes)
        {

        }

        public NeatNetwork(INeatConfiguration configuration, IInnovationCounter innovationCounter, IRandomizer randomizer, ISameSpeciesDetectionCalculation distanceFunc, ISpeciesBreedSelectionStrategy speciesBreedSelectionStrategy, int inNodes, int outNodes)
        {
            this.Configuration = configuration;
            this.innovationCounter = innovationCounter;
            this.randomizer = randomizer;
            this.distanceFunc = distanceFunc;
            this.speciesBreedSelectionStrategy = speciesBreedSelectionStrategy;
            Genomes = new List<Genome>();
            Species = new List<Species>();

            var startingGenome = GenerateStartingGenome(inNodes, outNodes);

            Genomes.Add((Genome)startingGenome.Clone());
            Species.Add(new Species(Genomes[0]));
            BreedNextGeneration();
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

            SortGenomesIntoSpecies(Configuration, distanceFunc);

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
            float completeWeightForSpecies = 0;
            foreach (var s in Species)
            {
                s.CalculateFitness();
                completeWeightForSpecies += s.Fitness;
                nextGenerationGenomes.AddRange(speciesBreedSelectionStrategy.SelectGenomes(s));
            }

            CurrentGeneration++;
            BreedGenomes(randomizer, Configuration, innovationCounter, nextGenerationGenomes, completeWeightForSpecies);

            Genomes = nextGenerationGenomes;
        }

        private void Reset(IRandomizer randomizer)
        {
            foreach (var s in Species)
            {
                s.Reset(randomizer);
            }
        }

        private void BreedGenomes(IRandomizer randomizer, INeatConfiguration configuration, IInnovationCounter innovationCounter, List<Genome> nextGenerationGenomes,
            float completeWeightForSpecies)
        {
            while (nextGenerationGenomes.Count < configuration.PopulationSize)
            {
                Species s = GetRandomSpeciesBasedOnFitness(randomizer, completeWeightForSpecies);

                Genome p1 = GetRandomGenomeBasedOnFitness(randomizer, s);
                Genome p2 = GetRandomGenomeBasedOnFitness(randomizer, s);

                Genome child = p1.Crossover(configuration, randomizer, innovationCounter, p2);
                child.Generation = CurrentGeneration;

                if (randomizer.GetRandomPercentage() <= configuration.MutationRate)
                {
                    child.Mutation(configuration, randomizer);
                }

                if (randomizer.GetRandomPercentage() <= configuration.AddConnectionRate)
                {
                    child.AddConnectionMutation(configuration, randomizer, innovationCounter);
                }

                if (randomizer.GetRandomPercentage() <= configuration.AddNodeRate)
                {
                    child.AddNodeMutation(randomizer, innovationCounter);
                }

                nextGenerationGenomes.Add(child);
            }
        }

        private Genome GetRandomGenomeBasedOnFitness(IRandomizer randomizer, Species s)
        {
            float completeWeight = 0;

            foreach (var g in s.Genomes)
            {
                completeWeight += g.Fitness;
            }

            float r = randomizer.GetRandomPercentage() * completeWeight;
            float countWeight = 0;

            foreach (var g in s.Genomes)
            {
                countWeight += g.Fitness;

                if (countWeight > r)
                {
                    return g;
                }
            }

            return s.Genomes[s.Genomes.Count - 1];
        }

        private Species GetRandomSpeciesBasedOnFitness(IRandomizer randomizer, float completeWeight)
        {
            float r = randomizer.GetRandomPercentage() * completeWeight;
            float countWeight = 0;

            foreach (var s in Species)
            {
                countWeight += s.Fitness;

                if (countWeight >= r)
                {
                    return s;
                }
            }

            return Species[Species.Count - 1];
        }

        private void SortGenomesIntoSpecies(INeatConfiguration configuration, ISameSpeciesDetectionCalculation distanceFunc)
        {
            foreach (var g in Genomes)
            {
                bool foundSpecies = false;
                foreach (var s in Species)
                {
                    if (distanceFunc.IsSameSpecies(configuration, g, s.Mascot))
                    {
                        s.Genomes.Add(g);
                        foundSpecies = true;
                        break;
                    }
                }

                if (!foundSpecies)
                {
                    Species newSpecies = new Species(g);
                    Species.Add(newSpecies);
                }
            }

            Species.RemoveAll(x => x.Genomes.Count == 0);
        }
    }
}
