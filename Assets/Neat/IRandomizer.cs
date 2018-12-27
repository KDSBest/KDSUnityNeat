namespace KDS.Neat
{
    public interface IRandomizer
    {
        /// <summary>
        /// float Value from 0 to 1
        /// </summary>
        /// <returns></returns>
        float GetRandomPercentage();

        /// <summary>
        /// Gets random number from 0 to excluding max
        /// </summary>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        int GetRandom(int max);

        /// <summary>
        /// Gets the random from minimum to maximum (excluding max)
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        float GetRandom(float min, float max);

        /// <summary>
        /// Gets the random bool.
        /// </summary>
        /// <returns></returns>
        bool GetRandomBool();
    }
}