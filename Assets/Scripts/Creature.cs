﻿using System.Collections;
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

    public void init(float mVal, float MVal, float oVal, float pVal)
    {
        m = mVal;
        M = MVal;
        o = oVal;
        p = pVal;
    }

    public void Randomize()
    {
        m = Random.Range(-1f, +1f);
        M = Random.Range(-1f, +1f);
        o = Random.Range(-2f, 2f);
        p = Random.Range(0.1f, 2f);
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

    // Overloading + operator
    public static GenomeLeg operator +(GenomeLeg g1, GenomeLeg g2)
    {
        GenomeLeg addition = new GenomeLeg();

        addition.m = g1.m + g2.m;
        addition.M = g1.M + g2.M;
        addition.o = g1.o + g2.o;
        addition.p = g1.p + g2.p;

        return addition;
    }

    // Overloading - operator
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

    // Produces 2 crossover legs
    public static void Crossover(GenomeLeg g1, GenomeLeg g2, out GenomeLeg c1, out GenomeLeg c2)
    {
        //GenomeLeg cross = new GenomeLeg();

        int separationPoint = Random.Range(10, 29) / 10;
        float[] c1Vals = new float[4];
        float[] c2Vals = new float[4];
        float[] g1Vals = new float[4];
        float[] g2Vals = new float[4];

        g1Vals[0] = g1.m;
        g1Vals[1] = g1.M;
        g1Vals[2] = g1.o;
        g1Vals[3] = g1.p;

        g2Vals[0] = g2.m;
        g2Vals[1] = g2.M;
        g2Vals[2] = g2.o;
        g2Vals[3] = g2.p;

        for (int i = 0; i <= separationPoint; i++)
        {
            c1Vals[i] = g1Vals[i];
            c2Vals[i] = g2Vals[i];
        }

        for (int j = separationPoint + 1; j < 4; j++)
        {
            c1Vals[j] = g2Vals[j];
            c2Vals[j] = g1Vals[j];
        }

        c1.m = c1Vals[0];
        c1.M = c1Vals[1];
        c1.o = c1Vals[2];
        c1.p = c1Vals[3];

        c2.m = c2Vals[0];
        c2.M = c2Vals[1];
        c2.o = c2Vals[2];
        c2.p = c2Vals[3];

        //cross = (g1 + g2) / 2;
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

    public void init(float lm, float lM, float lo, float lp, float rm, float rM, float ro, float rp)
    {
        left.init(lm, lM, lo, lp);
        right.init(rm, rM, ro, rp);
    }

    public void Randomize()
    {
        left.Randomize();
        right.Randomize();
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

    // Overriding + operator
    public static Genome operator +(Genome g1, Genome g2)
    {
        Genome addition = new Genome();

        addition.left = g1.left + g2.left;
        addition.right = g1.right + g2.right;

        return addition;
    }

    // Overriding - operator
    public static Genome operator /(Genome g, float val)
    {
        Genome division = new Genome();

        division = g.Clone();

        division.left /= val;
        division.right /= val;

        return division;
    }

    // Produces 2 crossover genomes
    public static void Crossover(Genome g1, Genome g2, out Genome c1, out Genome c2)
    {
        GenomeLeg.Crossover(g1.left, g2.left, out c1.left, out c2.left);
        GenomeLeg.Crossover(g1.right, g2.right, out c1.right, out c2.right);
    }
}

public class Creature : MonoBehaviour {

    public Genome genome;

    public LegController left;
    public LegController right;


    private Vector3 initialPosition;

    bool isUp = true;

    // Use this for initialization
    void Start ()
    {
        initialPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isUp)
        {
            left.position = genome.left.EvaluateAt(Time.time);
            right.position = genome.right.EvaluateAt(Time.time);
        }
    }

    void CheckIfUp()
    {
        float headUpdotUP = Vector2.Dot(transform.up, Vector2.up);

        bool headDown =
        transform.eulerAngles.z > 180 - 45 &&
        transform.eulerAngles.z < 180 + 45;

        if (headDown)
            isUp = false;
    }

    public float GetScore()
    {
        // Walking score
        float walkingScore = transform.position.x - initialPosition.x;

        float headUpdotUP = Vector2.Dot(transform.up, Vector2.up);
        // Balancing score
        bool headUp =
        transform.eulerAngles.z < 0 + 30 ||
        transform.eulerAngles.z > 360 - 30;
        //headUpdotUP > 0 && headUpdotUP < Mathf.Cos(30 * Mathf.Deg2Rad);

        bool headDown = 
        transform.eulerAngles.z > 180 - 45 &&
        transform.eulerAngles.z < 180 + 45;

        // Return 0 if walking score is 0 meaning creature moving to the left
        if (walkingScore < 0)
            return 0;

        return
            walkingScore
            * (headDown ? 0.5f : 1f)
            + (headUp ? 5f : 0f)
            ;
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name.ToLower().Contains("land"))
        {
            isUp = false;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        }
    }
}
