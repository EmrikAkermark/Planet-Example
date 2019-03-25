using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Explosion : MonoBehaviour
{
    public bool RandomSurface;
    public int width;
    public int height;

    public float CubeWeight;
    public float CubeWeightVariation;

    public float ExplosiveForce;
    public float ExplosiveRadius;

    public Transform ExplosionOrigin;

    public GameObject unitPrefab;

    List<CubeClass> cubes = new List<CubeClass>();

void Start()
{
        FillBoard();
        Explode();
}

void FillBoard()
{
    Clear();

    /*
     * loop through width and height
     * based on a random value, instantiate a new unit
     * place the new unit with x and y
     * add the unit to the units list
    */

    for (int x = 0; x < width; x++)
    {
        for (var y = 0; y < height; y++)
        {
                CubeClass cube = new CubeClass();
                Vector3 pos = new Vector3(x - width * .5f + .5f, y - height * .5f + .5f, -.5f);
                float weight = CubeWeight + Random.Range(-CubeWeightVariation, CubeWeightVariation);
                cube.Cube = Instantiate(unitPrefab, gameObject.transform, true);
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
        float energy = (-dist * dist)/ExplosiveForce + ExplosiveRadius;
        if(energy < 0)
        {
            return 0f;
        }
        else
        {
            return energy / cubeWeight;
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
        FillBoard();
        Explode();
    }
}