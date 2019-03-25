
using UnityEngine;

public class CubeClass
{
    public GameObject Cube;
    public Vector3 StartVector;
    public float Weight;

    private Rigidbody cubeRB;

    public void FlingCube(Vector3 direction)
    {
        cubeRB = Cube.GetComponent<Rigidbody>();
        cubeRB.isKinematic = false;
        cubeRB.mass = Weight;
        cubeRB.velocity = direction;
    }
}
