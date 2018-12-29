namespace KDS.Neat.Defaults
{
    public class DefaultNeatConfiguration : INeatConfiguration
    {
        /// <summary>
        /// Gets the probability pertubing.
        ///
        /// Normally 90% aka 0.9f
        /// </summary>
        public float ProbabilityPertubing
        {
            get { return 0.9f; }
        }

        /// <summary>
        /// Gets the minimum uniform pertubing weight.
        ///
        /// Normally -2
        /// </summary>
        public float MinimumUniformPertubingWeight
        {
            get { return -2; }
        }

        /// <summary>
        /// Gets the maximum uniform pertubing weight.
        ///
        /// Normally 2
        /// </summary>
        public float MaximumUniformPertubingWeight
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the minimum generated weight.
        /// </summary>
        public float MinimumGeneratedWeight
        {
            get { return -2; }
        }

        /// <summary>
        /// Gets the maximum generated weight.
        /// </summary>
        public float MaximumGeneratedWeight
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the excess coeff.
        /// </summary>
        public float ExcessCoeff
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the disjoined coeff.
        /// </summary>
        public float DisjoinedCoeff
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the distance coeff.
        /// </summary>
        public float DistanceCoeff
        {
            get { return 0.4f; }
        }

        public float MutationRate
        {
            get { return 0.5f; }
        }

        public float AddConnectionRate
        {
            get { return 0.1f; }
        }

        public float AddNodeRate
        {
            get { return 0.1f; }
        }

        public float SameSpeciesDistance
        {
            get { return 3; }
        }

        public int PopulationSize
        {
            get { return 1000; }
        }

        public float Epsilon
        {
            get { return float.Epsilon; }
        }

        public int SameSpeciesNodeIgnoreValue
        {
            get { return 20; }
        }
    }
}