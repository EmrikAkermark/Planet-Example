using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetExplosion : MonoBehaviour
{
    public float PlanetRadius;
    public float ObjectWidth;
    public GameObject unitPrefab;

    public float CubeWeight;
    public float CubeWeightVariation;

    public float ExplosiveForce;
    public float ExplosiveRadius;

    public Transform ExplosionOrigin;


    private int rotationDivisionNumber;
    private float rotationSegment;
    private float circumference;
    List<CubeClass> cubes = new List<CubeClass>();

    void Start()
    {
        GenerateExplosion();
    }


    void CalculateRotationSegment(int divNumber = 1)
    {
        if (circumference / divNumber > ObjectWidth)
        {
            CalculateRotationSegment(divNumber * 2);
        }
        else
        {
            rotationDivisionNumber = divNumber / 2;
            float rotationDivisionNumberFloat = rotationDivisionNumber;
            rotationSegment = 360 / rotationDivisionNumberFloat;
            SetPositions();
        }
    }


    void SetPositions()
    {
        for (int i = 0; i < rotationDivisionNumber; i++)
        {
            //We don't bother filling in the "poles" of the planet, as that would just add a bunch of cubes in the same position
            for (int j = 0; j < rotationDivisionNumber / 2 - 1; j++)
            {
                CubeClass cube = new CubeClass();
                float weight = CubeWeight + Random.Range(-CubeWeightVariation, CubeWeightVariation);
                Vector3 pos = Quaternion.Euler(rotationSegment * (j - (rotationDivisionNumber / 4 - 1)), rotationSegment * i, rotationSegment * (j - (rotationDivisionNumber / 4 - 1))) * Vector3.forward * PlanetRadius + gameObject.transform.position;               
                cube.Cube = Instantiate(unitPrefab, pos, transform.rotation, gameObject.transform);
                cube.StartVector = pos;
                cube.Weight = weight;
                cubes.Add(cube);
            }
        }
    }

    public void Explode()
    {
        /*
         * Generate an explosion
         * put origin at Explosion origin
         * loop through cubes list
         * Figure out new position based on explosion strength, distance from origin, what way to go, and weight
         * Make the cubes face the explosion origin        
         */
        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].Cube.transform.position = cubes[i].StartVector + Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position) * ExplosionEnergy(cubes[i].StartVector, cubes[i].Weight);
            cubes[i].Cube.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position));
        }
    }

    float ExplosionEnergy(Vector3 cubeStart, float cubeWeight)
    {
        float dist = Vector3.Distance(cubeStart, ExplosionOrigin.position);
        float energy = (-dist * dist) / ExplosiveForce + ExplosiveRadius;
        if (energy < 0)
        {
            return 0f;
        }
        else
        {
            return energy / cubeWeight;
        }
    }


    public void EngagePhysics()
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].Cube.transform.position = cubes[i].StartVector;
            cubes[i].Cube.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position));
            Vector3 direction = Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position) * ExplosionEnergy(cubes[i].StartVector, cubes[i].Weight);
            cubes[i].FlingCube(direction);
        }
    }


    void Clear()
    {
        foreach (CubeClass cube in cubes)
        {
            Destroy(cube.Cube);
        }
        cubes.Clear();
    }

    public void GenerateExplosion()
    {
        Clear();
        circumference = PlanetRadius * 2 * Mathf.PI;
        CalculateRotationSegment();
        Explode();
    }
}
