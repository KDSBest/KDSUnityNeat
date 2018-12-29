using System;
using System.Collections.Generic;
using System.Linq;

namespace KDS.Neat
{
    public class Species
    {
        public List<Genome> Genomes;

        public Genome Mascot;

        private float fitness = float.NaN;
        public float Fitness
        {
            get
            {
                if (fitness == float.NaN)
                {
                    CalculateFitness();
                }

                return fitness;
            }
        }

        public void CalculateFitness()
        {
            fitness = Genomes.Sum(x => x.Fitness) / Genomes.Count;
        }

        public Species(Genome mascot)
        {
            this.Mascot = mascot;
            Genomes = new List<Genome>();
            Genomes.Add(this.Mascot);
        }

        public void Reset(IRandomizer randomizer)
        {
            fitness = float.NaN;

            int newMascotIndex = randomizer.GetRandom(Genomes.Count);
            this.Mascot = Genomes[newMascotIndex];
            this.Genomes.Clear();
        }

        public static void SortGenomesIntoSpecies(INeatConfiguration configuration, ISameSpeciesDetectionCalculation distanceFunc, List<Genome> genomes, List<Species> specieses)
        {
            foreach (var g in genomes)
            {
                bool foundSpecies = false;
                foreach (var s in specieses)
                {
                    if (distanceFunc.IsSameSpecies(configuration, g, s.Mascot))
                    {
                        s.Genomes.Add(g);
                        foundSpecies = true;
                        break;
                    }
                }

                if (!foundSpecies)
                {
                    Species newSpecies = new Species(g);
                    specieses.Add(newSpecies);
                }
            }

            specieses.RemoveAll(x => x.Genomes.Count == 0);
        }
    }
}