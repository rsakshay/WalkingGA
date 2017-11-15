using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct GenomeLeg
{
    public float m;
    public float M;
    public float o;
    public float p;

    public void init()
    {
        m = 0;
        M = 0;
        o = 0;
        p = 0.1f;
    }

    public float EvaluateAt(float time)
    {
        // https://www.alanzucconi.com/2016/04/20/evolutionary-computation-3/
        return (M - m) / 2 * (1 + Mathf.Sin((time + o) * Mathf.PI * 2 / p)) + m;
    }

    public GenomeLeg Clone()
    {
        GenomeLeg leg = new GenomeLeg();
        leg.m = m;
        leg.M = M;
        leg.o = o;
        leg.p = p;

        return leg;
    }

    public void Mutate()
    {
        int chance = Random.Range(0, 39) / 10;

        switch (chance)
        {
            case 0:
                //m += Random.Range(-0.1f, 0.1f);
                //m = Mathf.Clamp(m, -1f, +1f);
                m = Random.Range(-1f, +1f);
                break;
            case 1:
                //M += Random.Range(-0.1f, 0.1f);
                //M = Mathf.Clamp(M, -1f, +1f);
                M = Random.Range(-1f, +1f);
                break;
            case 2:
                //o += Random.Range(-0.25f, 0.25f);
                //o = Mathf.Clamp(o, -2f, 2f);
                o = Random.Range(-2f, 2f);
                break;
            case 3:
                //p += Random.Range(-0.25f, 0.25f);
                //p = Mathf.Clamp(p, 0.1f, 2f);
                p = Random.Range(0.1f, 2f);
                break;
        }
    }

    public static GenomeLeg operator +(GenomeLeg g1, GenomeLeg g2)
    {
        GenomeLeg addition = new GenomeLeg();

        addition.m = g1.m + g2.m;
        addition.M = g1.M + g2.M;
        addition.o = g1.o + g2.o;
        addition.p = g1.p + g2.p;

        return addition;
    }

    public static GenomeLeg operator /(GenomeLeg g, float val)
    {
        GenomeLeg division = new GenomeLeg();
        division = g.Clone();

        division.m /= val;
        division.M /= val;
        division.o /= val;
        division.p /= val;

        return division;
    }

    public static GenomeLeg Crossover(GenomeLeg g1, GenomeLeg g2)
    {
        GenomeLeg cross = new GenomeLeg();

        //cross.m = g1.m;
        //cross.M = g1.M;

        //cross.o = g2.o;
        //cross.p = g2.p;

        cross = (g1 + g2) / 2;

        return cross;
    }
}

public struct Genome
{
    public GenomeLeg left;
    public GenomeLeg right;

    public void init()
    {
        left.init();
        right.init();
    }

    public Genome Clone()
    {
        Genome genome = new Genome();
        genome.left = left.Clone();
        genome.right = right.Clone();

        return genome;
    }

    public void Mutate()
    {
        int chance = Random.Range(0, 19) / 10;

        switch (chance)
        {
            case 0:
                left.Mutate();
                break;

            case 1:
                right.Mutate();
                break;
        }
    }

    public static Genome operator +(Genome g1, Genome g2)
    {
        Genome addition = new Genome();

        addition.left = g1.left + g2.left;
        addition.right = g1.right + g2.right;

        return addition;
    }

    public static Genome operator /(Genome g, float val)
    {
        Genome division = new Genome();

        division = g.Clone();

        division.left /= val;
        division.right /= val;

        return division;
    }

    public static Genome Crossover(Genome g1, Genome g2)
    {
        Genome cross = new Genome();

        cross.left = GenomeLeg.Crossover(g1.left, g2.left);
        cross.right = GenomeLeg.Crossover(g1.right, g2.right);

        return cross;
    }
}

public class Creature : MonoBehaviour {

    public Genome genome;

    public LegController left;
    public LegController right;


    private Vector3 initialPosition;

    // Use this for initialization
    void Start ()
    {
        initialPosition = transform.position;

        //genome.left.m = 0;
        //genome.left.M = 1;
        //genome.left.o = -5;
        //genome.left.p = Mathf.PI;

        //genome.right.m = 0;
        //genome.right.M = 0;
        //genome.right.o = 5;
        //genome.right.p = Mathf.PI;
    }
	
	// Update is called once per frame
	void Update ()
    {
        left.position = genome.left.EvaluateAt(Time.time);
        right.position = genome.right.EvaluateAt(Time.time);
    }


    public float GetScore()
    {
        // Walking score
        float walkingScore = transform.position.x - initialPosition.x;

        float headUpdotUP = Vector2.Dot(transform.up, Vector2.up);
        // Balancing score
        bool headUp =
        //transform.eulerAngles.z < 0 + 30 ||
        //transform.eulerAngles.z > 360 - 30;
        headUpdotUP > 0 && headUpdotUP < Mathf.Cos(30 * Mathf.Deg2Rad);

        bool headDown = !headUp;

        // Return 0 if walking score is 0 meaning creature moving to the left
        if (walkingScore < 0)
            return 0;

        return
            walkingScore
            * (headDown ? 0.5f : 1f)
            + (headUp ? 2f : 0f)
            ;
    }
}
