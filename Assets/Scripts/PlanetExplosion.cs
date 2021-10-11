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

    public float[] ExplosiveArray;

    public Transform ExplosionOrigin;

    public int VisualiserSpeed = 1;
    private List<CubeClass> cubes = new List<CubeClass>();
    private Vector3 lastPosition = Vector3.zero;
    private bool physicsEngaged = false;

    void Start()
    {
        GenerateOptimisedExplosion();
    }

    public void GenerateExplosion()
    {
        Clear();
        SetUpAngles();
        Explode();
        StartCoroutine(Visualiser());
    }

    public void GenerateOptimisedExplosion()
    {
        Clear();
        SetUpAngles();
        OptimisedExplosionSetup();
        StartCoroutine(Visualiser());
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
        //if (ExplosionOrigin.position == lastPosition || physicsEngaged)
        //    return;
        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].Cube.transform.position = cubes[i].StartVector + Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position) * ExplosionEnergy(cubes[i].StartVector, cubes[i].Weight);
            cubes[i].Cube.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position));
        }
        lastPosition = ExplosionOrigin.position;
    }
    public void OptimisedExplode()
    {
        /*
         * Generate an explosion
         * put origin at Explosion origin
         * loop through cubes list
         * Figure out new position based on explosion strength, distance from origin, what way to go, and weight
         * Make the cubes face the explosion origin        
         */
        //if (ExplosionOrigin.position == lastPosition || physicsEngaged)
        //    return;
        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].Cube.transform.position = cubes[i].StartVector + Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position) * OptimisedExplosion(cubes[i].StartVector, cubes[i].Weight);
            cubes[i].Cube.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(cubes[i].StartVector - ExplosionOrigin.position));
        }
        lastPosition = ExplosionOrigin.position;
    }

    void OptimisedExplosionSetup()
    {
        float maxDistance = Mathf.Sqrt(ExplosiveForce * ExplosiveRadius);
        ExplosiveArray = new float[Mathf.CeilToInt(maxDistance)];
        for (int i = 0; i < ExplosiveArray.Length; i++)
        {
            ExplosiveArray[i] = -(Mathf.Pow(i, 2)) / ExplosiveForce + ExplosiveRadius;
        }
    }

    float OptimisedExplosion(Vector3 cubeStart, float cubeWeight)
    {
        float dist = Vector3.Distance(cubeStart, ExplosionOrigin.position);
        int distInt = Mathf.RoundToInt(dist);
        if (distInt >= ExplosiveArray.Length)
        {
            return 0f;
        }
        else
        {
            return ExplosiveArray[distInt] / cubeWeight;
        }

    }

    float ExplosionEnergy(Vector3 cubeStart, float cubeWeight)
    {
        if (ExplosiveForce == 0)
            return 0f;
        float dist = Vector3.Distance(cubeStart, ExplosionOrigin.position);
        float energy = -(Mathf.Pow(dist, 2)) / ExplosiveForce + ExplosiveRadius;
        if (energy < 0f)
        {
            return energy / cubeWeight;
        }
        else
        {
            return energy / cubeWeight;
        }
    }

    private void SetUpAngles()
    {
        float circumference = PlanetRadius * Mathf.PI * 2;
        float segments = 1;
        while (circumference / segments > ObjectWidth)
        {
            segments++;
        }
        segments--;
        float segmentAngle = 360 / segments;
        int currentSegment = 0;
        while (segmentAngle * currentSegment < 360f)
        {
            float currentAngle = segmentAngle * currentSegment;
            float height = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * PlanetRadius;
            float radius = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * PlanetRadius;
            SetPositions(radius, height);
            currentSegment++;
        }
		Debug.Log($"There are currently {cubes.Count} number of boxes");
    }

    private void SetPositions(float radius, float newHeight)
    {
        Vector3 heightMod = new Vector3(0, newHeight, 0);

        float circumference = radius * Mathf.PI * 2;
        float segments = 1;
        while (circumference / segments > ObjectWidth)
        {
            segments++;
        }
        segments--;
        float segmentAngle = 360 / segments;

        for (int i = 0; i < segments; i++)
        {
            CubeClass cube = new CubeClass();
            float weight = CubeWeight + Random.Range(-CubeWeightVariation, CubeWeightVariation);
            Vector3 pos = Quaternion.Euler(0, segmentAngle * i, 0) * Vector3.forward * radius + gameObject.transform.position + heightMod;
            cube.Cube = Instantiate(unitPrefab, pos, transform.rotation, gameObject.transform);
            cube.StartVector = pos;
            cube.Weight = weight;
            cubes.Add(cube);
        }
    }

    private IEnumerator Visualiser()
    {
        int numberOfCubes = cubes.Count;
        for (int i = 0; i < numberOfCubes; i++)
        {
            cubes[i].Cube.SetActive(false);
        }
        int activatedCubes = 0;
        int NextBatchOfCubes = VisualiserSpeed;
        while(activatedCubes < numberOfCubes)
        {
            for (int i = activatedCubes; i < NextBatchOfCubes; i++)
            {
                cubes[i].Cube.SetActive(true);
            }
            yield return null;
            activatedCubes = NextBatchOfCubes;
            NextBatchOfCubes = Mathf.Min(NextBatchOfCubes + VisualiserSpeed, numberOfCubes);
        }
    }

    public void EngagePhysics()
    {
        physicsEngaged = true;
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
        physicsEngaged = false;
    }
}
