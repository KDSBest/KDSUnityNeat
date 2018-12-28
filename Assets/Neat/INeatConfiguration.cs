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
        ///
        /// A normal Value is 1
        /// </summary>
        float ExcessCoeff { get; }

        /// <summary>
        /// Gets the disjoined coeff.
        ///
        /// A normal Value is 1
        /// </summary>
        float DisjoinedCoeff { get; }

        /// <summary>
        /// Gets the distance coeff.
        ///
        /// A normal Value is 0.4
        /// </summary>
        float DistanceCoeff { get; }

        /// <summary>
        /// Gets the mutation rate.
        ///
        /// A normal Value is 0.9
        /// </summary>
        float MutationRate { get; }

        /// <summary>
        /// Gets the add connection rate.
        ///
        /// A Normal Value is 0.1
        /// </summary>
        float AddConnectionRate { get; }

        /// <summary>
        /// Gets the add node rate.
        ///
        /// A normal Value is 0.1
        /// </summary>
        float AddNodeRate { get; }

        /// <summary>
        /// Gets the same species distance.
        ///
        /// A normal Value is 3-10
        /// </summary>
        float SameSpeciesDistance { get; }

        /// <summary>
        /// Gets the size of the population.
        ///
        /// A normal Value is 150 or as much as your computer can handle
        /// </summary>
        int PopulationSize { get; }

        /// <summary>
        /// Gets the epsilon.
        /// </summary>
        float Epsilon { get; }

        /// <summary>
        /// Gets the same species node ignore value.
        ///
        /// Normally a Value of 12 or 20
        /// </summary>
        int SameSpeciesNodeIgnoreValue { get; }
    }
}