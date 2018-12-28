using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.Neat.Solver
{
    public class GenomeSolver : IComparer<NodeGene>
    {
        public int Compare(NodeGene x, NodeGene y)
        {
            return x.Id.CompareTo(y.Id);
        }

        public List<NodeGene> SimpleLoopSolver(Genome genome, int networkSolvingLoops,
            Dictionary<int, float> inputValues)
        {
            var nodes = SetupSolver(genome, inputValues);

            for (int i = 0; i < networkSolvingLoops; i++)
            {
                foreach (var con in genome.GetConnections().Values)
                {
                    if (!con.Expressed)
                    {
                        continue;
                    }

                    var inNode = genome.Nodes[con.InNode];
                    var outNode = genome.Nodes[con.OutNode];
                    outNode.Value += inNode.Value * con.Weight;
                }
            }

            return nodes;
        }

        /// <summary>
        /// Traverses solver, this requires the network to not have any circular references!
        /// If there are any circular references maximum node depth will prevent an stack overflow
        /// </summary>
        /// <param name="genome">The genome.</param>
        /// <param name="maximumNodeDepth">The maximum node depth.</param>
        /// <param name="inputValues">The input values.</param>
        /// <returns></returns>
        public List<NodeGene> TraverseSolver(Genome genome, int maximumNodeDepth, Dictionary<int, float> inputValues)
        {
            var nodes = SetupSolver(genome, inputValues);

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Value = GetValue(genome, nodes[i], maximumNodeDepth, 0);
            }

            return nodes;   
        }

        private List<NodeGene> SetupSolver(Genome genome, Dictionary<int, float> inputValues)
        {
            foreach (var nodeKeyValue in genome.Nodes)
            {
                nodeKeyValue.Value.Value = 0;
            }

            foreach (var inputValue in inputValues)
            {
                genome.Nodes[inputValue.Key].Value = inputValue.Value;
            }

            var nodes = genome.Nodes.Values.Where(x => x.Type == NodeGeneType.Output).ToList();
            nodes.Sort(this);
            return nodes;
        }

        private float GetValue(Genome genome, NodeGene node, int maximumNodeDepth, int depth)
        {
            if (node.Type == NodeGeneType.Input)
            {
                return node.Value;
            }

            if (depth > maximumNodeDepth)
            {
                return node.Value;
            }

            node.Value = 0;

            foreach (var con in genome.GetConnections().Values)
            {
                if (!con.Expressed)
                {
                    continue;
                }

                if (con.OutNode != node.Id)
                {
                    continue;
                }

                node.Value += GetValue(genome, genome.Nodes[con.InNode], maximumNodeDepth, depth + 1) * con.Weight;
            }

            return node.Value;
        }
    }
}
