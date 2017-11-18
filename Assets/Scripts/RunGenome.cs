using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunGenome : MonoBehaviour {
    public int generation;

    public float left_m;
    public float left_M;
    public float left_o;
    public float left_p;

    public float right_m;
    public float right_M;
    public float right_o;
    public float right_p;

    public GameObject creaturePrefab;

    public Text genText;
    public Text mText;
    public Text MText;
    public Text oText;
    public Text pText;


    Creature creature;

    Genome genome = new Genome();

    // Use this for initialization
    void Start () {
        StartCoroutine(Run());
    }

    // Update is called once per frame
    void Update () {
		
	}

    public IEnumerator Run()
    {
        genome.init(left_m, left_M, left_o, left_p, right_m, right_M, right_o, right_p);

        genText.text = "Generation: " + (generation);
        mText.text = "left m: " + genome.left.m + "  right m: " + genome.right.m;
        MText.text = "left M: " + genome.left.M + "  right M: " + genome.right.M;
        oText.text = "left o: " + genome.left.o + "  right o: " + genome.right.o;
        pText.text = "left p: " + genome.left.p + "  right p: " + genome.right.p;

        Vector3 pos = (Vector3.down * 35f);
        GameObject creatureGO = Instantiate(creaturePrefab, pos, Quaternion.identity);

        creature = creatureGO.GetComponentInChildren<Creature>(true);
        creature.genome = genome;

        yield return new WaitForSeconds(0.5f);

        creature.enabled = true;
    }
}
