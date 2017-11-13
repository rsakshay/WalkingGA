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
                m += Random.Range(-0.1f, 0.1f);
                m = Mathf.Clamp(m, -1f, +1f);
                break;
            case 1:
                M += Random.Range(-0.1f, 0.1f);
                M = Mathf.Clamp(M, -1f, +1f);
                break;
            case 2:
                o += Random.Range(-0.25f, 0.25f);
                o = Mathf.Clamp(o, -2f, 2f);
                break;
            case 3:
                p += Random.Range(-0.25f, 0.25f);
                p = Mathf.Clamp(p, 0.1f, 2f);
                break;
        }
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
        float walkingScore = Mathf.Abs(transform.position.x - initialPosition.x);

        float headUpdotUP = Vector2.Dot(transform.up, Vector2.up);
        // Balancing score
        bool headUp =
        //transform.eulerAngles.z < 0 + 30 ||
        //transform.eulerAngles.z > 360 - 30;
        headUpdotUP > 0 && headUpdotUP < Mathf.Cos(30 * Mathf.Deg2Rad);

        bool headDown = !headUp;
        return
            walkingScore
            * (headDown ? 0.5f : 1f)
            + (headUp ? 2f : 0f)
            ;
    }
}