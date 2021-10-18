using UnityEngine;
using Unity.Collections;
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

struct NewlyInstanced : IComponentData { }

//public class DOPInitialiser : SystemBase
//{
//	protected override void OnUpdate()
//	{

//		Entities.WithAll<NewCube>().ForEach((int entityInQueryIndex) =>
//		{ }).WithDisposeOnCompletion(positions).ScheduleParallel();

//		EntityManager.SetComponentData(instance, new Translation() { Value = myPosition });
//		EntityManager.(instance, new Rotation() { Value = myRotation });
//		EntityManager.AddComponentData(instance, new ExplosionData { OriginalPos = myPosition, ExplosionPos = transform.position });
//		EntityManager.AddComponentData(instance, new UnitWeight { Value = UnitBaseWeight + UnityEngine.Random.Range(-UnitWeightVariation, UnitWeightVariation) });
//		Enabled = false;
//	}
//}

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

	private Settings explosionSettings;

	private static readonly ProfilerMarker AngleSettingpm = new ProfilerMarker("Setting Angles");
	private static readonly ProfilerMarker Instancingpm = new ProfilerMarker("Instancing");
	private static readonly ProfilerMarker SettingUppm = new ProfilerMarker("Setup");
	//private static readonly ProfilerMarker Instancingpm = new ProfilerMarker("Instancing");

	void Start()
    {
		_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		newEntityConversion = GameObjectConversionUtility.ConvertGameObjectHierarchy(OurObject, settings);
		forceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ForceOrigin, settings);
		//explosionSettings = GetSingleton
		//GETTHISSHIT();
	}

	public void GETTHISSHIT()
	{
		SetUpAngles();
		Debug.Log($"We have {_numberOfBoxes} boxes out");
		Entity force = _entityManager.Instantiate(forceEntity);
		_entityManager.SetComponentData(force, new Translation { Value = transform.position });
	}

	private void SetUpAngles()
	{
		AngleSettingpm.Begin();
		float circumference = PlanetRadius * Mathf.PI * 2;
		float segments = 1;
		while (circumference / segments > 1)
		{
			segments++;
		}
		segments--;
		float segmentAngle = 360 / segments;
		int currentSegment = 0;
		AngleSettingpm.End();
		while (segmentAngle * currentSegment < 360f)
		{
			float currentAngle = segmentAngle * currentSegment;
			float height = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * PlanetRadius;
			float radius = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * PlanetRadius;
			Instancingpm.Begin();
			SetPositions(radius, height);
			Instancingpm.End();
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
			Vector3 myPosition = Quaternion.Euler(0, segmentAngle * i, 0) * Vector3.forward * radius + gameObject.transform.position + heightMod;
			Entity instance = _entityManager.Instantiate(newEntityConversion);
			quaternion myRotation = Quaternion.LookRotation(Vector3.Normalize(transform.position-myPosition), Vector3.up);

			SettingUppm.Begin();
			_entityManager.SetComponentData(instance, new Translation() { Value = myPosition });
			_entityManager.SetComponentData(instance, new Rotation() { Value = myRotation });
			_entityManager.AddComponentData(instance, new ExplosionData { OriginalPos = myPosition, ExplosionPos = transform.position });
			_entityManager.AddComponentData(instance, new UnitWeight { Value = UnitBaseWeight + UnityEngine.Random.Range(-UnitWeightVariation, UnitWeightVariation)});
			if(i%2 == 0)
			{
				_entityManager.AddComponent<NoMoveTag>(instance);
			}
			SettingUppm.End();
			_numberOfBoxes++;
		}
	}


}
