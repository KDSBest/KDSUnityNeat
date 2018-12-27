using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using KDS.Neat;
using KDS.Neat.Defaults;
using KDS.Neat.KillGenomesStrategies;
using UnityEngine;

namespace KDS
{
    public class PingPongLogicBehaviour : MonoBehaviour
    {
        public List<Simulation> simulations = new List<Simulation>();
        public GameObject BallPrefab;

        /// <summary>
        /// In Nodes
        ///
        /// Ball x
        /// Ball y
        /// Ball Direction x
        /// Ball Direction y
        /// My yMin
        /// My yMax
        ///
        /// Out Nodes
        ///     Go Up
        ///     Go Down
        /// </summary>
        private NeatNetwork neat = new NeatNetwork(6, 2);

        public void Start()
        {
            int rounds = 100;
            Task.Run(() =>
            {
                while (rounds > 0)
                {
                    neat.Evaluate(SimulateNetwork);
                    Debug.Log("Highest Fitness: " + neat.FittestGenome.Fitness);
                    Debug.Log("Highest Generation: " + neat.FittestGenome.Generation);
                    Debug.Log("Species: " + neat.Species.Count);
                    neat.BreedNextGeneration();
                    rounds--;
                }
            });
        }

        private void SimulateNetwork(List<Genome> genomes)
        {
            try
            {
                List<Simulation> simulations = new List<Simulation>();

                foreach (var g in genomes)
                {
                    simulations.Add(new Simulation()
                    {
                        Genome = g
                    });
                }

                this.simulations = simulations;

                while (simulations.Count(x => x.Done) != simulations.Count)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        // Update is called once per frame
        public void FixedUpdate()
        {
            try
            {
                if (simulations.Count != neat.Configuration.PopulationSize)
                {
                    return;
                }
                for (int i = 0; i < simulations.Count; i++)
                {
                    if (simulations[i] == null || simulations[i].Done)
                    {
                        continue;
                    }

                    if (simulations[i].PingPongSimulation == null && !simulations[i].Done)
                    {
                        simulations[i].PingPongSimulation = GameObject.Instantiate(BallPrefab);
                        var simulationBehaviour = simulations[i].PingPongSimulation.GetComponent<SimulationBehaviour>();
                        simulationBehaviour.Genome = simulations[i].Genome;
                        simulationBehaviour.Configuration = this.neat.Configuration;
                    }
                    else
                    {
                        var simulationBehaviour = simulations[i].PingPongSimulation.GetComponent<SimulationBehaviour>();
                        simulations[i].Genome.Fitness = simulationBehaviour.Fitness;
                        simulations[i].Done = simulationBehaviour.Done;
                        if (simulations[i].Done)
                        {
                            GameObject.Destroy(simulationBehaviour.Player);
                            GameObject.Destroy(simulations[i].PingPongSimulation);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
