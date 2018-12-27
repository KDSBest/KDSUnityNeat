using System;

namespace KDS.Neat
{
    public class ConnectionGene : ICloneable
    {
        public int InNode;
        public int OutNode;
        public float Weight;
        public bool Expressed;
        public int Innovation;

        public ConnectionGene(int inNode, int outNode, float weight, bool expressed, int innovation)
        {
            InNode = inNode;
            OutNode = outNode;
            Weight = weight;
            Expressed = expressed;
            Innovation = innovation;
        }

        public void Disable()
        {
            Expressed = false;
        }

        public object Clone()
        {
            return new ConnectionGene(InNode, OutNode, Weight, Expressed, Innovation);
        }
    }
}