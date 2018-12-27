using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KDS.Neat
{
    public class Genome : ICloneable, IComparer<NodeGene>
    {
        public float Fitness;
        public Dictionary<int, ConnectionGene> Connections;
        public Dictionary<int, NodeGene> Nodes;
        public int Generation = 1;
        public int Id = 0;

        public Genome()
        {
            this.Connections = new Dictionary<int, ConnectionGene>();
            this.Nodes = new Dictionary<int, NodeGene>();
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

            // TODO: Use Dictionary as cache
            foreach (var con in Connections.Values)
            {
                if (con.InNode == node1.Id && con.OutNode == node2.Id)
                {
                    return;
                }

                if (con.OutNode == node1.Id && con.InNode == node2.Id)
                {
                    return;
                }
            }

            float weight = randomizer.GetRandom(configuration.MinimumGeneratedWeight, configuration.MaximumGeneratedWeight);
            int innvoationNumber = innovationCounter.GetConnectionInnovation();
            this.Connections.Add(innvoationNumber,
                new ConnectionGene(reverse ? node2.Id : node1.Id, reverse ? node1.Id : node2.Id, weight, true,
                    innvoationNumber));
        }

        public void AddNodeMutation(IRandomizer randomizer, IInnovationCounter innovationCounter)
        {
            var keys = Connections.Keys.ToList();
            int connectionIndex = keys[randomizer.GetRandom(Connections.Count)];
            ConnectionGene con = Connections[connectionIndex];

            NodeGene inNode = Nodes[con.InNode];
            NodeGene outNode = Nodes[con.OutNode];

            con.Disable();

            NodeGene newNode = new NodeGene(NodeGeneType.Hidden, innovationCounter.GetNodeGeneInnovation());
            Nodes.Add(newNode.Id, newNode);

            ConnectionGene inToNew =
                new ConnectionGene(inNode.Id, newNode.Id, 1, true, innovationCounter.GetConnectionInnovation());
            ConnectionGene newToOut = new ConnectionGene(newNode.Id, outNode.Id, con.Weight, true,
                innovationCounter.GetConnectionInnovation());
            Connections.Add(inToNew.Innovation, inToNew);
            Connections.Add(newToOut.Innovation, newToOut);
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

            foreach (var parent1Connection in parent1.Connections.Values)
            {
                if (parent2.Connections.ContainsKey(parent1Connection.Innovation))
                {
                    if (randomizer.GetRandomBool())
                    {
                        child.Connections.Add(parent1Connection.Innovation, (ConnectionGene)parent1Connection.Clone());
                    }
                    else // Disjoined or Excessed
                    {
                        child.Connections.Add(parent1Connection.Innovation,
                            (ConnectionGene)parent2.Connections[parent1Connection.Innovation].Clone());
                    }
                }
                else
                {
                    child.Connections.Add(parent1Connection.Innovation, (ConnectionGene)parent1Connection.Clone());
                }
            }

            if (aboutTheSame)
            {
                foreach (var parent2Connection in parent2.Connections.Values)
                {
                    if (!parent1.Connections.ContainsKey(parent2Connection.Innovation)) // Disjoined or Excessed
                    {
                        child.Connections.Add(parent2Connection.Innovation, (ConnectionGene)parent2Connection.Clone());
                    }
                }
            }

            return child;
        }

        public void Mutation(INeatConfiguration configuration, IRandomizer randomizer)
        {
            foreach (var con in Connections.Values)
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
            foreach (var keyVal in this.Connections)
            {
                clone.Connections.Add(keyVal.Key, keyVal.Value);
            }

            clone.Id = Id;
            return clone;
        }

        public List<NodeGene> CalculateValues(INeatConfiguration configuration, Dictionary<int, float> inputValues)
        {
            foreach (var nodeKeyValue in Nodes)
            {
                nodeKeyValue.Value.Value = 0;
            }

            foreach (var inputValue in inputValues)
            {
                Nodes[inputValue.Key].Value = inputValue.Value;
            }

            for (int i = 0; i < configuration.MaxNetworkSolvingLoops; i++)
            {
                foreach (var con in Connections.Values)
                {
                    if (!con.Expressed)
                    {
                        continue;
                    }

                    var inNode = Nodes[con.InNode];
                    var outNode = Nodes[con.OutNode];
                    outNode.Value += inNode.Value * con.Weight;
                }
            }

            var nodes = Nodes.Values.Where(x => x.Type == NodeGeneType.Output).ToList();
            nodes.Sort(this);

            return nodes;
        }

        public int Compare(NodeGene x, NodeGene y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }
}