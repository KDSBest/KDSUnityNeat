namespace KDS.Neat
{
    public interface INeatConfiguration
    {
        /// <summary>
        /// Gets the probability pertubing.
        ///
        /// Normally 90% aka 0.9f
        /// </summary>
        float ProbabilityPertubing { get; }

        /// <summary>
        /// Gets the minimum uniform pertubing weight.
        ///
        /// Normally -2
        /// </summary>
        float MinimumUniformPertubingWeight { get; }

        /// <summary>
        /// Gets the maximum uniform pertubing weight.
        ///
        /// Normally 2
        /// </summary>
        float MaximumUniformPertubingWeight { get; }

        /// <summary>
        /// Gets the minimum generated weight.
        /// </summary>
        float MinimumGeneratedWeight { get; }

        /// <summary>
        /// Gets the maximum generated weight.
        /// </summary>
        float MaximumGeneratedWeight { get; }

        /// <summary>
        /// Gets the excess coeff.
        /// </summary>
        float ExcessCoeff { get; }

        /// <summary>
        /// Gets the disjoined coeff.
        /// </summary>
        float DisjoinedCoeff { get; }

        /// <summary>
        /// Gets the distance coeff.
        /// </summary>
        float DistanceCoeff { get; }

        float MutationRate { get; }

        float AddConnectionRate { get; }

        float AddNodeRate { get; }

        float SameSpeciesDistance { get; }

        int PopulationSize { get; }

        float Epsilon { get; }

        int MaxNetworkSolvingLoops { get; }
   }
}