using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Simulation : MonoBehaviour {

    public int generations = 100;
    public int variations = 100;
    public float simulationTime = 15f;
    public GameObject creaturePrefab;
    public Vector3 separationDistance = new Vector3(50, 0, 0);
    public Text genText;
    public Text scoreText;
    public Text mText;
    public Text MText;
    public Text oText;
    public Text pText;

    private Genome bestGenome;
    private float bestScore = 0;
    private List<Creature> creatures = new List<Creature>();
    
    struct Parent
    {
        public Genome parentGenome;
        public float parentScore;
    }

    private List<Parent> betterParents = new List<Parent>();
    private List<Parent> worseParents = new List<Parent>();


    // Use this for initialization
    void Start () {
        bestGenome = new Genome();
        bestGenome.init();
        StartCoroutine(Sim());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator Sim()
    {
        for (int i = 0; i < generations; i++)
        {
            UpdateText(i);

            CreateCreatures(i);

            yield return new WaitForSeconds(0.5f);

            StartSim();

            yield return new WaitForSeconds(simulationTime);

            StopSim();
            EvaluateScore();

            DestroyCreatures();

            yield return new WaitForSeconds(1);
        }
    }

    void UpdateText(int generation)
    {
        genText.text = "Generation: " + (generation + 1);
        scoreText.text = "Best Score: " + bestScore;
        mText.text = "left m: " + bestGenome.left.m + "  right m: " + bestGenome.right.m;
        MText.text = "left M: " + bestGenome.left.M + "  right M: " + bestGenome.right.M;
        oText.text = "left o: " + bestGenome.left.o + "  right o: " + bestGenome.right.o;
        pText.text = "left p: " + bestGenome.left.p + "  right p: " + bestGenome.right.p;
    }

    void CreateCreatures(int currentGeneration)
    {
        for (int i = 0; i < variations; i+=2)
        {
            Genome genome1 = new Genome();
            genome1.init();

            Genome genome2 = new Genome();
            genome2.init();

            if (currentGeneration == 0)
            {
                // Randomize the genome for 1st generation
                genome1.Randomize();
                genome2.Randomize();
            }
            else
            {
                Parent p1 = SelectParent();
                Parent p2 = SelectParent();

                // Crossover
                Genome.Crossover(p1.parentGenome, p2.parentGenome, out genome1, out genome2);

                // Mutate
                if (Random.Range(0, 100) < 5)
                    genome1.Mutate();

                if (Random.Range(0, 100) < 5)
                    genome2.Mutate();
            }

            // Instantiate creature
            InstantiateCreature(i, genome1);
            InstantiateCreature(i + 1, genome2);
        }

        worseParents.Clear();
        betterParents.Clear();

    }

    void InstantiateCreature(int sepDistMultiplier, Genome genome)
    {
        Vector3 pos = (Vector3.down * 35f) + sepDistMultiplier * separationDistance;
        GameObject creatureGO = Instantiate(creaturePrefab, pos, Quaternion.identity);

        Creature creature = creatureGO.GetComponentInChildren<Creature>(true);
        creature.genome = genome;

        creatures.Add(creature);
    }

    void StartSim()
    {
        foreach(Creature creature in creatures)
        {
            creature.enabled = true;
        }
    }

    void StopSim()
    {
        foreach (Creature creature in creatures)
        {
            creature.enabled = false;
        }
    }

    void EvaluateScore()
    {
        //float totalScore = 0;
        //Genome totalGenome = new Genome();
        //int numBetter = 0;

        foreach (Creature creature in creatures)
        {
            float fitnessVal = creature.GetScore();

            Parent currentCreature = new Parent();
            currentCreature.parentGenome = creature.genome.Clone();
            currentCreature.parentScore = fitnessVal;

            if (fitnessVal > bestScore)
            {
                //bestScore = fitnessVal;
                //bestGenome = creature.genome.Clone();

                //totalScore += fitnessVal;
                //totalGenome += creature.genome.Clone();
                //numBetter++;

                betterParents.Add(currentCreature);
            }
            else
                worseParents.Add(currentCreature);
        }

        bestScore = CalculateMaxBestScore();

        //if (numBetter > 0)
        //{
        //    bestScore = totalScore / numBetter;
        //    bestGenome = totalGenome / numBetter;
        //}
    }

    Parent SelectParent()
    {
        const int rangeExpander = 10;

        // Get random index for worseParents list
        int worseIndex = -1;
        if (worseParents.Count > 0)
            worseIndex = Random.Range(0, worseParents.Count * rangeExpander) / rangeExpander;

        // Get random index for betterParents list
        int betterIndex = -1;
        if (betterParents.Count > 0)
            betterIndex = Random.Range(0, betterParents.Count * rangeExpander) / rangeExpander;

        // return a random worseParent if no elements in betterParents
        if (betterIndex == -1)
        {
            Parent returnParent = worseParents[worseIndex];
            worseParents.RemoveAt(worseIndex);
            return returnParent;
        }

        // return a random betterParent if no elements in worseParents
        if (worseIndex == -1)
        {
            Parent returnParent = betterParents[betterIndex];
            betterParents.RemoveAt(betterIndex);
            return returnParent;
        }


        int choice = Random.Range(0, 10 * rangeExpander) / rangeExpander;

        // If both have valid count then check by choice
        if (choice < 1)
        {
            // Choose parent from worseParents List
            Parent returnParent = worseParents[worseIndex];
            worseParents.RemoveAt(worseIndex);
            return returnParent;
        }
        else
        {
            // Choose parent from betterParents List
            Parent returnParent = betterParents[betterIndex];
            betterParents.RemoveAt(betterIndex);
            return returnParent;
        }
    }

    void DestroyCreatures()
    {
        foreach (Creature creature in creatures)
            Destroy(creature.transform.parent.gameObject);

        creatures.Clear();
    }

    /// <summary>
    /// Get the minimum score in better parents
    /// </summary>
    float CalculateMinBestScore()
    {
        float score = betterParents[0].parentScore;
        for(int i = 1; i < betterParents.Count; i++)
        {
            if (betterParents[i].parentScore < score)
                score = betterParents[i].parentScore;
        }

        return score;
    }

    /// <summary>
    /// Get the maximum score in better parents
    /// </summary>
    float CalculateMaxBestScore()
    {
        if (betterParents.Count > 0)
        {
            int bestIndex = 0;
            float score = betterParents[0].parentScore;
            for (int i = 1; i < betterParents.Count; i++)
            {
                if (betterParents[i].parentScore > score)
                {
                    score = betterParents[i].parentScore;
                    bestIndex = i;
                }
            }

            bestGenome = betterParents[bestIndex].parentGenome;
            return score;
        }
        
        if (worseParents.Count > 0)
        {
            int bestIndex = 0;
            float score = worseParents[0].parentScore;
            for (int i = 1; i < worseParents.Count; i++)
            {
                if (worseParents[i].parentScore > score)
                {
                    score = worseParents[i].parentScore;
                    bestIndex = i;
                }
            }

            bestGenome = worseParents[bestIndex].parentGenome;
            return score;
        }

        return 0;
    }
}
