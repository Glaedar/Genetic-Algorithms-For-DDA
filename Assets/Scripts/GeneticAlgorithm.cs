using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {

    public List<DNA> dna;
    public List<DNA> lastGenerationDNA;
    
    //number of enemies to be spawned i.e. the population of the GA
    public int populationSize = 20;
    // crossover rate
    public float crossoverRate = 0.70f;
    //mutation rate
    public float mutationRate = 0.05f;
    //number of bits in chromosome
    public int chromosomeLength = 20;
    //number of bits in a gene
    public int geneLength = 5;

    //get details of fittest for testing purposes
    public int fittestGenome;
    public float bestFitnessScore;

    public int generation;
    public WaveSpawner waveSpawner;
    private float testFitness;
    

    public bool busy;

    private int noOfEnemiesThrough;

    public GeneticAlgorithm()
    {
        busy = false;
        dna = new List<DNA>();
        lastGenerationDNA= new List<DNA>();

    }

    void start()
    {
        GameObject waveSpawnController = GameObject.Find("WaveSpawnController");
        waveSpawner = waveSpawnController.GetComponent<WaveSpawner>();
    }
    
    public void Mutate(List<int> bits)
    {
        for (int i = 0; i < bits.Count; i++)
        {
            // check if we want to flip a bit
            if (UnityEngine.Random.value < mutationRate)
            {
                // flip the bit
                bits[i] = bits[i] == 0 ? 1 : 0;
            }
        }
    }
    
    //crossover two parents selected in the epoch
    public void Crossover(List<int> parent1, List<int> parent2, List<int> offspring1, List<int> offspring2)
    {
        
        //if the values are not in the crossover rate or there is only one parent just add them to the list without crossover
        if (UnityEngine.Random.value > crossoverRate || parent1 == parent2)
        {

            offspring1.AddRange(parent1);
            offspring2.AddRange(parent2);
    
            return;
        }
    
        System.Random rnd = new System.Random();
    
        //select a random point in the gene and crossover to create offspring
        int crossoverPoint = rnd.Next(0, chromosomeLength - 1);
    
        for (int i = 0; i < crossoverPoint; i++)
        {
            offspring1.Add(parent1[i]);
            offspring2.Add(parent2[i]);
        }
    
        for (int i = crossoverPoint; i < parent1.Count; i++)
        {
            offspring1.Add(parent2[i]);
            offspring2.Add(parent1[i]);
        }
    }
    
    //select paarents based on fitness but add a random element aswell
    public DNA ParentSelection()
    {
        float slice = UnityEngine.Random.value * testFitness;
        float total = 0;
        int selectedGenome = 0;
    
        for (int i = 0; i < populationSize; i++)
        {
            total += dna[i].fitness;
    
            if (total > slice)
            {
                selectedGenome = i;
                break;
            }
        }
        return dna[selectedGenome];
    }
    
    //put the stats into arrays for the wave spawner to use
    public void DecodeStats()
    {
        for (int i = 0; i < populationSize; i++)
        {
            List<int> stats = Decode(dna[i].bits);
            waveSpawner.EnemyStatsIntoArrays(stats);
        }
    }
    
    //update the fitness scores to new set generated in the wave spawner
    public void UpdateFitnessScores()
    {
        fittestGenome = 0;
        bestFitnessScore = 0;
        testFitness = 0;
        //totalFitnessScore = 0;
    
        for (int i = 0; i < populationSize; i++)
        {
            List<int> stats = Decode(dna[i].bits);
    
            dna[i].fitness = waveSpawner.calculateFitness(stats);
        
            testFitness += dna[i].fitness;
    
            //get the bvest fitness score for testing purposes
            if (dna[i].fitness > bestFitnessScore)
            {
                bestFitnessScore = dna[i].fitness;
                fittestGenome = i;

                // check to see if the enemy has reached the end and stop the GA if it has
                if (dna[i].fitness >= 1)
                {
                    noOfEnemiesThrough++;
                }
              
            }
        }
        Debug.Log("The best fitness for wave " + waveSpawner.waveNumber + " was " + bestFitnessScore);

        if (bestFitnessScore >= 1.0f)
        {

            Debug.Log("Beat the towers in " + waveSpawner.waveNumber + " waves");
            Debug.Log("Number of Enemies through finish line is  " + noOfEnemiesThrough);
            Debug.Break();
        }

    }
    
    // decode the bits into integers and apply them to the stat list
    public List<int> Decode(List<int> bits)
    {
        List<int> stats = new List<int>();
    
        for (int geneIndex = 0; geneIndex < bits.Count; geneIndex += geneLength)
        {
            List<int> gene = new List<int>();
    
            for (int bitIndex = 0; bitIndex < geneLength; bitIndex++)
            {
                gene.Add(bits[geneIndex + bitIndex]);
            }
            
            stats.Add(GeneToInt(gene));
        }
        return stats;
    }
    
    //convert bits to integers
    public int GeneToInt(List<int> gene)
    {
        int value = 0;
        int multiplier = 1;
    
        for (int i = gene.Count; i > 0; i--)
        {
            value += gene[i - 1] * multiplier;
            multiplier *= 2;
        }
        return value;
    }
    
    //create the initial population, decode and send them to the wavespawner
    public void CreateStartPopulation()
    {
        dna.Clear();
    
        for (int i = 0; i < populationSize; i++)
        {
            DNA child = new DNA (chromosomeLength);
            dna.Add(child);
        }

        DecodeStats();
    }
    
    public void Run()
    {
        CreateStartPopulation();
       // busy = true;
    }
    
    public void Epoch()
    {
        if (!busy) return;
        UpdateFitnessScores();
    
        if (!busy)
        {
            lastGenerationDNA.Clear();
            lastGenerationDNA.AddRange(dna);
            return;
        }
    
        int numberOfOffspring = 0;
    
        List<DNA> offspring = new List<DNA>();
        while (numberOfOffspring < populationSize)

        {
            // select 2 parents
            DNA parent1 = ParentSelection();
            DNA parent2 = ParentSelection();
            while (parent1 == parent2)
            {
                parent1 = ParentSelection();
                parent2 = ParentSelection();
            }
            // create new dna for the children to go into.
            DNA offspring1 = new DNA();
            DNA offspring2 = new DNA();
            Crossover(parent1.bits, parent2.bits, offspring1.bits, offspring2.bits);
            Mutate(offspring1.bits);
            Mutate(offspring2.bits);
            offspring.Add(offspring1);
            offspring.Add(offspring2);
    
            numberOfOffspring += 2;
        }
    
        
        lastGenerationDNA.Clear();
        lastGenerationDNA.AddRange(dna);
        // overwrite population with babies
        dna = offspring;
        //decode stats again to add new generation to enemies
        DecodeStats();

       // Debug.Log("number of offspring " + offspring.Count);
        // increment the generation counter
        generation++;
    
    }

}
