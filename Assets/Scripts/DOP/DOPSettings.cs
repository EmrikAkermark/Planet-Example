using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Settings : IComponentData
{
	//public float3[] PlanetPositions;
	public float3 StartingPosition;
	public Entity Cube, ForceSphere;
	public float UnitBaseWeight;
	public float UnitWeightVariation;
	public float PlanetRadius;
	public float ExplosiveForce;
}
