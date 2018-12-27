using System;
using System.Collections.Generic;

namespace KDS.Neat
{
    public class NodeGene : ICloneable
    {
        public NodeGeneType Type;
        public int Id;

        public float Value = 0;

        public NodeGene(NodeGeneType type, int id)
        {
            Type = type;
            Id = id;
        }

        public object Clone()
        {
            return new NodeGene(Type, Id);
        }
    }
}