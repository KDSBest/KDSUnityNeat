using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KDS.Neat
{
    public class Genome : ICloneable
    {
        public float Fitness = 0.0f;
        private Dictionary<int, ConnectionGene> connections = new Dictionary<int, ConnectionGene>();
        private Dictionary<int, List<int>> connectionsAlreadyExistsCache = new Dictionary<int, List<int>>();

        public Dictionary<int, NodeGene> Nodes = new Dictionary<int, NodeGene>();
        public int Generation = 1;
        public int Id = 0;

        public IReadOnlyDictionary<int, ConnectionGene> GetConnections()
        {
            return this.connections;
        }

        public void AddConnection(ConnectionGene gene)
        {
            if (!this.connections.ContainsKey(gene.Innovation))
            {
                this.connections.Add(gene.Innovation, gene);

                int lowKey = Math.Min(gene.InNode, gene.OutNode);
                int highKey = Math.Max(gene.InNode, gene.OutNode);

                if (!connectionsAlreadyExistsCache.ContainsKey(lowKey))
                {
                    connectionsAlreadyExistsCache.Add(lowKey, new List<int>());
                }

                connectionsAlreadyExistsCache[lowKey].Add(highKey);
            }
        }

        private bool ExistsConnection(int inNode, int outNode)
        {
            int lowKey = Math.Min(inNode, outNode);
            int highKey = Math.Max(inNode, outNode);

            if (!connectionsAlreadyExistsCache.ContainsKey(lowKey))
            {
                return false;
            }

            return connectionsAlreadyExistsCache[lowKey].Contains(highKey);
        }

        public void AddConnectionMutation(INeatConfiguration configuration, IRandomizer randomizer, IInnovationCounter innovationCounter)
        {
            var keys = Nodes.Keys.ToList();
            int index1 = keys[randomizer.GetRandom(Nodes.Count)];
            int index2 = keys[randomizer.GetRandom(Nodes.Count)];

            NodeGene node1 = Nodes[index1];
            NodeGene node2 = Nodes[index2];

            bool reverse = false;
            if (node1.Type == NodeGeneType.Output)
            {
                if (node2.Type == NodeGeneType.Output)
                {
                    return;
                }

                reverse = true;
            }

            if (node2.Type == NodeGeneType.Input)
            {
                if (node1.Type == NodeGeneType.Input)
                {
                    return;
                }

                reverse = true;
            }

            if (ExistsConnection(node1.Id, node2.Id))
            {
                return;
            }

            float weight = randomizer.GetRandom(configuration.MinimumGeneratedWeight, configuration.MaximumGeneratedWeight);
            int innvoationNumber = innovationCounter.GetConnectionInnovation();
            this.AddConnection(new ConnectionGene(reverse ? node2.Id : node1.Id, reverse ? node1.Id : node2.Id, weight, true, innvoationNumber));
        }

        public void AddNodeMutation(IRandomizer randomizer, IInnovationCounter innovationCounter)
        {
            var keys = connections.Keys.ToList();
            int connectionIndex = keys[randomizer.GetRandom(connections.Count)];
            ConnectionGene con = connections[connectionIndex];

            NodeGene inNode = Nodes[con.InNode];
            NodeGene outNode = Nodes[con.OutNode];

            con.Disable();

            NodeGene newNode = new NodeGene(NodeGeneType.Hidden, innovationCounter.GetNodeGeneInnovation());
            Nodes.Add(newNode.Id, newNode);

            ConnectionGene inToNew = new ConnectionGene(inNode.Id, newNode.Id, 1, true, innovationCounter.GetConnectionInnovation());
            ConnectionGene newToOut = new ConnectionGene(newNode.Id, outNode.Id, con.Weight, true, innovationCounter.GetConnectionInnovation());
            AddConnection(inToNew);
            AddConnection(newToOut);
        }

        public Genome Crossover(INeatConfiguration configuration, IRandomizer randomizer, IInnovationCounter innovationCounter, Genome genome2)
        {
            Genome child = new Genome()
            {
                Id = innovationCounter.GetGenomeInnovation()
            };

            Genome parent1 = this;
            Genome parent2 = genome2;
            if (genome2.Fitness > this.Fitness)
            {
                parent1 = genome2;
                parent2 = this;
            }

            bool aboutTheSame = Mathf.Abs(parent1.Fitness - parent2.Fitness) <= configuration.Epsilon;

            foreach (var n in parent1.Nodes.Values)
            {
                child.Nodes.Add(n.Id, (NodeGene)n.Clone());
            }

            if (aboutTheSame)
            {
                foreach (var n in parent2.Nodes.Values)
                {
                    if (!child.Nodes.ContainsKey(n.Id))
                    {
                        child.Nodes.Add(n.Id, (NodeGene)n.Clone());
                    }
                }
            }

            var parent1Connections = parent1.GetConnections();
            var parent2Connections = parent2.GetConnections();

            foreach (var parent1Connection in parent1Connections.Values)
            {
                if (parent2Connections.ContainsKey(parent1Connection.Innovation))
                {
                    if (randomizer.GetRandomBool())
                    {
                        child.AddConnection((ConnectionGene)parent1Connection.Clone());
                    }
                    else // Disjoined or Excessed
                    {
                        child.AddConnection((ConnectionGene)parent2Connections[parent1Connection.Innovation].Clone());
                    }
                }
                else
                {
                    child.AddConnection((ConnectionGene)parent1Connection.Clone());
                }
            }

            if (aboutTheSame)
            {
                foreach (var parent2Connection in parent2Connections.Values)
                {
                    if (!parent1Connections.ContainsKey(parent2Connection.Innovation)) // Disjoined or Excessed
                    {
                        child.AddConnection((ConnectionGene)parent2Connection.Clone());
                    }
                }
            }

            return child;
        }

        public void Mutation(INeatConfiguration configuration, IRandomizer randomizer)
        {
            foreach (var con in connections.Values)
            {
                if (randomizer.GetRandomPercentage() <= configuration.ProbabilityPertubing)
                {
                    float newWeight = con.Weight * randomizer.GetRandom(configuration.MinimumUniformPertubingWeight, configuration.MaximumUniformPertubingWeight);
                    con.Weight = newWeight;
                }
                else
                {
                    float newWeight = randomizer.GetRandom(configuration.MinimumGeneratedWeight, configuration.MaximumGeneratedWeight);
                    con.Weight = newWeight;
                }
            }
        }

        public object Clone()
        {
            Genome clone = new Genome();
            clone.Fitness = this.Fitness;
            clone.Generation = this.Generation;
            foreach (var keyVal in this.Nodes)
            {
                clone.Nodes.Add(keyVal.Key, keyVal.Value);
            }

            foreach (var keyVal in this.connections)
            {
                clone.AddConnection(keyVal.Value);
            }

            clone.Id = Id;
            return clone;
        }
    }
}