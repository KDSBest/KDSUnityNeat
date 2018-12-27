using System;

namespace KDS.Neat.Defaults
{
    public class DefaultRandomizer : IRandomizer
    {
        private Random random = new Random();

        /// <summary>
        /// float Value from 0 to 1
        /// </summary>
        /// <returns></returns>
        public float GetRandomPercentage()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Gets random number from 0 to excluding max
        /// </summary>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public int GetRandom(int max)
        {
            return random.Next(max);
        }

        /// <summary>
        /// Gets the random from minimum to maximum (excluding max)
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public float GetRandom(float min, float max)
        {
            float distance = max - min;
            return (float)random.NextDouble() * distance + min;
        }

        /// <summary>
        /// Gets the random bool.
        /// </summary>
        /// <returns></returns>
        public bool GetRandomBool()
        {
            return GetRandom(2) == 1;
        }
    }
}