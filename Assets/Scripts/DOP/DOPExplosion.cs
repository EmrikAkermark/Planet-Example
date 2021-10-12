using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public class DOPExplosion : MonoBehaviour
{
	public GameObject OurObject;
	public int NumberOfObjects;
	public float PlanetRadius = 100f;

	private EntityManager _entityManager;
	private Entity newEntityConversion;
	private GameObjectConversionSettings settings;
	private int _numberOfBoxes = 0;


	void Start()
    {
		_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		newEntityConversion = GameObjectConversionUtility.ConvertGameObjectHierarchy(OurObject, settings);

		SetUpAngles();
		Debug.Log($"We have {_numberOfBoxes} boxes out");
		//for(var i = 0; i < NumberOfObjects; i++)
		//{
		//	Entity instance = _entityManager.Instantiate(newEntityConversion);
		//	Vector3 myPosition = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
		//	_entityManager.SetComponentData(instance, new Translation() { Value = myPosition });
		//	_entityManager.SetComponentData(instance, new Rotation() { Value = transform.rotation });
		//}
    }

	private void SetUpAngles()
	{
		float circumference = PlanetRadius * Mathf.PI * 2;
		float segments = 1;
		while (circumference / segments > 1)
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
	}

	private void SetPositions(float radius, float newHeight)
	{
		Vector3 heightMod = new Vector3(0, newHeight, 0);

		float circumference = radius * Mathf.PI * 2;
		float segments = 1;
		while (circumference / segments > 1)
		{
			segments++;
		}
		segments--;
		float segmentAngle = 360 / segments;

		for (int i = 0; i < segments; i++)
		{
			float weight = 10 + UnityEngine.Random.Range(-3, 3);
			Vector3 myPosition = Quaternion.Euler(0, segmentAngle * i, 0) * Vector3.forward * radius + gameObject.transform.position + heightMod;
			Entity instance = _entityManager.Instantiate(newEntityConversion);
			quaternion myRotation = Quaternion.LookRotation(transform.position - myPosition, Vector3.up);
			_entityManager.SetComponentData(instance, new Translation() { Value = myPosition });
			_entityManager.SetComponentData(instance, new Rotation() { Value = myRotation });
			// I just want my entity to remember its start location
			//_entityManager.AddComponentData(instance, new float3() { x = myPosition.x, y = myPosition.y, z = myPosition.z });
			//_entityManager.SetComponentData(instance, new float3() { x = myPosition.x, y = myPosition.y, z = myPosition.z });
			_numberOfBoxes++;
		}
	}


}
