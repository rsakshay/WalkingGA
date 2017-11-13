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

            CreateCreatures();
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

    void CreateCreatures()
    {
        for (int i = 0; i < variations; i++)
        {
            // Mutate best genome
            Genome genome = bestGenome.Clone();
            genome.Mutate();

            // Instantiate creature
            Vector3 pos = Vector3.zero + i * separationDistance;
            GameObject creatureGO = Instantiate(creaturePrefab, pos, Quaternion.identity);

            Creature creature = creatureGO.GetComponentInChildren<Creature>(true);
            creature.genome = genome;

            creatures.Add(creature);
        }
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
        foreach (Creature creature in creatures)
        {
            float fitnessVal = creature.GetScore();

            if (fitnessVal > bestScore)
            {
                bestScore = fitnessVal;
                bestGenome = creature.genome.Clone();
            }
        }
    }

    void DestroyCreatures()
    {
        foreach (Creature creature in creatures)
            Destroy(creature.transform.parent.gameObject);

        creatures.Clear();
    }
}
