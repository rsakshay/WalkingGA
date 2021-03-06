﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Simulation : MonoBehaviour {

    public int generations = 100;
    public int variations = 100;
    public float simulationTime = 15f;
    [Range(1, 100)]
    public float mutationChance = 5;
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
    private List<float> scores = new List<float>();
    
    struct Parent
    {
        public Genome parentGenome;
        public float parentScore;
    }

    //private List<Parent> betterParents = new List<Parent>();
    //private List<Parent> worseParents = new List<Parent>();
    private List<Parent> parents = new List<Parent>();
    //private List<Parent> allParents = new List<Parent>();


    // Use this for initialization
    void Start () {
        bestGenome = new Genome();
        bestGenome.init();
        StartCoroutine(Sim());
        UpdateText(0);
	}

    public IEnumerator Sim()
    {
        for (int i = 0; i < generations; i++)
        {
            CreateCreatures(i);

            yield return new WaitForSeconds(0.5f);

            StartSim();

            yield return new WaitForSeconds(simulationTime);

            StopSim();
            EvaluateScore();

            DestroyCreatures();

            yield return new WaitForSeconds(1);

            UpdateText(i + 1);
        }

        WriteToFile();
    }

    void WriteToFile()
    {
        using (StreamWriter sw = new StreamWriter("scores.txt"))
        {
            foreach (float val in scores)
            {
                sw.WriteLine(val);
            }
        }
        
        using (StreamWriter sw = new StreamWriter("bestGenome.txt"))
        {
            sw.WriteLine(mText.text);
            sw.WriteLine(MText.text);
            sw.WriteLine(oText.text);
            sw.WriteLine(pText.text);
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
                Parent p1 = SelectParent(true);
                Parent p2 = SelectParent(false);

                // Crossover
                Genome.Crossover(p1.parentGenome, p2.parentGenome, out genome1, out genome2);
            }

            // Mutate
            if (Random.Range(0, 100) < mutationChance)
                genome1.Mutate();

            if (Random.Range(0, 100) < mutationChance)
                genome2.Mutate();

            // Instantiate creature
            InstantiateCreature(i, genome1);
            InstantiateCreature(i + 1, genome2);
        }

        //worseParents.Clear();
        //betterParents.Clear();
        parents.Clear();

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

            //if (fitnessVal > bestScore)
            //{
            //    //bestScore = fitnessVal;
            //    //bestGenome = creature.genome.Clone();

            //    //totalScore += fitnessVal;
            //    //totalGenome += creature.genome.Clone();
            //    //numBetter++;

            //    betterParents.Add(currentCreature);
            //}
            //else
            //    worseParents.Add(currentCreature);
            parents.Add(currentCreature);
            //allParents.Add(currentCreature);

            //if (fitnessVal > bestScore)
            //    betterParents.Add(currentCreature);
        }

        bestScore = CalculateMaxBestScore();
        scores.Add(bestScore);

        //if (numBetter > 0)
        //{
        //    bestScore = totalScore / numBetter;
        //    bestGenome = totalGenome / numBetter;
        //}
    }

    //Parent SelectParent()
    //{
    //    const int rangeExpander = 10;

    //    // Get random index for worseParents list
    //    int worseIndex = -1;
    //    if (worseParents.Count > 0)
    //        worseIndex = Random.Range(0, worseParents.Count * rangeExpander) / rangeExpander;

    //    // Get random index for betterParents list
    //    int betterIndex = -1;
    //    if (betterParents.Count > 0)
    //        betterIndex = Random.Range(0, betterParents.Count * rangeExpander) / rangeExpander;

    //    // return a random worseParent if no elements in betterParents
    //    if (betterIndex == -1)
    //    {
    //        Parent returnParent = worseParents[worseIndex];
    //        worseParents.RemoveAt(worseIndex);
    //        return returnParent;
    //    }

    //    // return a random betterParent if no elements in worseParents
    //    if (worseIndex == -1)
    //    {
    //        Parent returnParent = betterParents[betterIndex];
    //        betterParents.RemoveAt(betterIndex);
    //        return returnParent;
    //    }


    //    int choice = Random.Range(0, 10 * rangeExpander) / rangeExpander;

    //    // If both have valid count then check by choice
    //    if (choice < 1)
    //    {
    //        // Choose parent from worseParents List
    //        Parent returnParent = worseParents[worseIndex];
    //        worseParents.RemoveAt(worseIndex);
    //        return returnParent;
    //    }
    //    else
    //    {
    //        // Choose parent from betterParents List
    //        Parent returnParent = betterParents[betterIndex];
    //        betterParents.RemoveAt(betterIndex);
    //        return returnParent;
    //    }
    //}

    /// <summary>
    /// Selects a parent with maximum score from either the first half of the list or the second.
    /// </summary>
    /// <param name="firstHalf">boolean to check first half of the list or second half. True will check only the first half</param>
    /// <returns>Returns a parent with the highest score in the first/seconmd half of the list</returns>
    Parent SelectParent(bool firstHalf)
    {
        //if (betterParents.Count == 0)
        //    return SelectBestParent(allParents);

        int startIndex = 0;
        int endIndex = 0;

        if (firstHalf)
        {
            endIndex = parents.Count / 2;
        }
        else
        {
            startIndex = parents.Count / 2;
            endIndex = parents.Count;
        }

        int bestIndex = startIndex;
        float score = parents[bestIndex].parentScore;

        for (int i = startIndex + 1; i < endIndex; i++)
        {
            if (parents[i].parentScore > score)
            {
                score = parents[i].parentScore;
                bestIndex = i;
            }
        }

        Parent returnParent = parents[bestIndex];
        parents.RemoveAt(bestIndex);
        return returnParent;
    }

    Parent SelectBestParent(List<Parent> parentList)
    {
        int bestIndex = 0;
        float score = parentList[0].parentScore;

        for (int i = 1; i < parentList.Count; i++)
        {
            if (parentList[i].parentScore > score)
            {
                score = parentList[i].parentScore;
                bestIndex = i;
            }
        }

        Parent returnParent = parentList[bestIndex];
        parentList.RemoveAt(bestIndex);
        return returnParent;
    }

    /// <summary>
    /// Destroys all creatures
    /// </summary>
    void DestroyCreatures()
    {
        foreach (Creature creature in creatures)
            Destroy(creature.transform.parent.gameObject);

        creatures.Clear();
    }

    /// <summary>
    /// Get the maximum score in better parents
    /// </summary>
    //float CalculateMaxBestScore()
    //{
    //    if (betterParents.Count > 0)
    //    {
    //        int bestIndex = 0;
    //        float score = betterParents[0].parentScore;
    //        for (int i = 1; i < betterParents.Count; i++)
    //        {
    //            if (betterParents[i].parentScore > score)
    //            {
    //                score = betterParents[i].parentScore;
    //                bestIndex = i;
    //            }
    //        }

    //        bestGenome = betterParents[bestIndex].parentGenome;
    //        return score;
    //    }

    //    if (worseParents.Count > 0)
    //    {
    //        int bestIndex = 0;
    //        float score = worseParents[0].parentScore;
    //        for (int i = 1; i < worseParents.Count; i++)
    //        {
    //            if (worseParents[i].parentScore > score)
    //            {
    //                score = worseParents[i].parentScore;
    //                bestIndex = i;
    //            }
    //        }

    //        bestGenome = worseParents[bestIndex].parentGenome;
    //        return score;
    //    }

    //    return 0;
    //}

    /// <summary>
    /// Get the maximum score in parents
    /// </summary>
    float CalculateMaxBestScore()
    {
        if (parents.Count == 0)
            return bestScore;

        int bestIndex = 0;
        float score = parents[0].parentScore;
        for (int i = 1; i < parents.Count; i++)
        {
            if (parents[i].parentScore > score)
            {
                score = parents[i].parentScore;
                bestIndex = i;
            }
        }

        bestGenome = parents[bestIndex].parentGenome;
        return score;
    }
}
