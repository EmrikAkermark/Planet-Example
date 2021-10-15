using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Profiling;
struct ExplosionData : IComponentData 
{
	public float3 OriginalPos;
	public float3 ExplosionPos;
	
}

struct UnitWeight : IComponentData
{
	public float Value;
}

struct NoMoveTag : IComponentData{ }

public class DOPExplosion : MonoBehaviour
{
	public GameObject OurObject;
	public GameObject ForceOrigin;
	public float UnitBaseWeight = 5f;
	public float UnitWeightVariation = 2f;
	public float PlanetRadius = 100f;

	private EntityManager _entityManager;
	private Entity newEntityConversion;
	private Entity forceEntity;
	private GameObjectConversionSettings settings;
	private int _numberOfBoxes = 0;

	private static readonly ProfilerMarker s_AngleSettingpm = new ProfilerMarker("Setting Angles");
	private static readonly ProfilerMarker s_Instancingpm = new ProfilerMarker("Instancing");

	void Start()
    {
	
    }

	public void GETTHISSHIT()
	{
		_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		newEntityConversion = GameObjectConversionUtility.ConvertGameObjectHierarchy(OurObject, settings);
		forceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ForceOrigin, settings);

		SetUpAngles();
		Debug.Log($"We have {_numberOfBoxes} boxes out");
		Entity force = _entityManager.Instantiate(forceEntity);
		_entityManager.SetComponentData(force, new Translation { Value = transform.position });
		//_entityManager.AddComponent<InputData>(force);
	}

	private void SetUpAngles()
	{
		s_AngleSettingpm.Begin();
		float circumference = PlanetRadius * Mathf.PI * 2;
		float segments = 1;
		while (circumference / segments > 1)
		{
			segments++;
		}
		segments--;
		float segmentAngle = 360 / segments;
		int currentSegment = 0;
		s_AngleSettingpm.End();
		while (segmentAngle * currentSegment < 360f)
		{
			float currentAngle = segmentAngle * currentSegment;
			float height = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * PlanetRadius;
			float radius = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * PlanetRadius;
			s_Instancingpm.Begin();
			SetPositions(radius, height);
			s_Instancingpm.End();
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
			quaternion myRotation = Quaternion.LookRotation(Vector3.Normalize(transform.position-myPosition), Vector3.up);
			_entityManager.SetComponentData(instance, new Translation() { Value = myPosition });
			_entityManager.SetComponentData(instance, new Rotation() { Value = myRotation });
			_entityManager.AddComponentData(instance, new ExplosionData { OriginalPos = myPosition, ExplosionPos = transform.position });
			_entityManager.AddComponentData(instance, new UnitWeight { Value = UnitBaseWeight + UnityEngine.Random.Range(-UnitWeightVariation, UnitWeightVariation)});
			if(i%2 == 0)
			{
				_entityManager.AddComponent<NoMoveTag>(instance);
			}

			_numberOfBoxes++;
		}
	}


}
