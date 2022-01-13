using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

struct ExplosionData : IComponentData 
{
	public float3 OriginalPos;
	public float3 ExplosionPos;
}

struct UnitWeight : IComponentData
{
	public float Value;
}

public class DOPExplosion : SystemBase
{
	float PlanetRadius;
	float3 StartPosition;
	NativeList<float3> PositionList;

	Random RandomData;

	EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

	protected override void OnCreate()
	{
		endSimulationEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnStartRunning()
	{
		base.OnStartRunning();

		RandomData = new Random(1);
	}

	protected override void OnUpdate()
	{
		//Setting up settings and commandbuffer
		var ecb = endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
		var settings = GetSingleton<Settings>();

		//Setting up global values
		StartPosition = settings.StartingPosition;
		PlanetRadius = settings.PlanetRadius;
		PositionList = new NativeList<float3>(0, Allocator.Persistent);

		//Creates every position on the sphere
		SetUpAngles();

		//Creating the cube, and adding some components before instancing
		Entity Cube = settings.Cube;
		//Todo, implement random variation in weight
		EntityManager.AddComponentData(Cube, new UnitWeight { Value = settings.UnitBaseWeight });
		EntityManager.AddComponentData(Cube, new ExplosionData {ExplosionPos = settings.StartingPosition });

		//This array exist to quickly instance all the cubes,
		//which is necessary due to the large amounts of them
		//past sphere-sizes of 10
		NativeArray<Entity> Instances = new NativeArray<Entity>(PositionList.Length, Allocator.Temp);
		EntityManager.Instantiate(Cube, Instances);

		//Ugly workaround due to foreach requiring local variables.
		//However, this does give us a chance to turn the list into an array
		var PositionArray = new NativeArray<float3>(PositionList.Length, Allocator.Persistent);
		for (int i = 0; i < PositionArray.Length; i++)
		{
			PositionArray[i] = PositionList[i];
		};

		//These are not needed anymore
		Instances.Dispose();
		PositionList.Clear();
		PositionList.Dispose();

		NativeArray<float> UnitWeights = new NativeArray<float>(PositionArray.Length, Allocator.Persistent);
		for (int i = 0; i < UnitWeights.Length; i++)
		{
			UnitWeights[i] = RandomData.NextFloat(settings.UnitBaseWeight - settings.UnitWeightVariation, settings.UnitBaseWeight + settings.UnitWeightVariation);
		};

		Entities.WithAll<NewCube>().ForEach((int entityInQueryIndex, ref UnitWeight unitWeight) =>
		{
			unitWeight.Value = UnitWeights[entityInQueryIndex];
		}).WithDisposeOnCompletion(UnitWeights).ScheduleParallel();


		//This sets the position of every instanced cube to a unique point
		//on the sphere from the PositionArray, along with removing the
		//<NewCube> tag so they won't be affacted by the setup of another sphere
		//if one is spawned
		Entities.WithAll<NewCube>().ForEach((int entityInQueryIndex, Entity entity, ref Translation translation, ref ExplosionData explosionData, ref UnitWeight unitWeight) =>
		{
			explosionData.OriginalPos = PositionArray[entityInQueryIndex];
			translation.Value = PositionArray[entityInQueryIndex];
			
			ecb.RemoveComponent<NewCube>(entityInQueryIndex ,entity);
		}).WithDisposeOnCompletion(PositionArray).ScheduleParallel();


		//This entity does not actually do anything, but serves as a visualiser
		//for the explosive point of this sphere
		Entity sphere = EntityManager.Instantiate(settings.ForceSphere);
		EntityManager.SetComponentData(sphere, new Translation { Value = settings.StartingPosition });


		endSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
		Enabled = false;
	}
	private void SetUpAngles()
	{

		float circumference = PlanetRadius * math.PI * 2;
		float segments = 1;
		//Finds out how many cubes will fit around the circumference
		//if a cube is size 1, change the one if the cube size is changed
		while (circumference / segments > 1)
		{
			segments++;
		}
		//The while loop exits when the the amount of cubes would intersect
		//one another, we take one step back for the densest amount of cubes without
		//them intersecting each other
		segments--;

		//Finds the angle in degrees between each cube in the sphere
		//Casting the segment int to a float is VERY important,
		//funky spheres and divide by 0 attempts happens without it
		//(Thank you 1000 times Oliver Lebert)
		float segmentAngle = 360f / (float)segments;
		int currentSegment = 0;

		//This finds the radous of each "band" around the sphere,
		//as well as its height compared to the equator.
		while (segmentAngle * currentSegment < 360f)
		{
			float currentAngle = segmentAngle * currentSegment;
			float height = math.cos(math.radians(currentAngle)) * PlanetRadius;
			float radius = math.sin(math.radians(currentAngle)) * PlanetRadius;

			//This sets the positions of the cubes in the band
			SetPositions(radius, height);

			currentSegment++;
		}
	}

	private void SetPositions(float radius, float newHeight)
	{
		//Much of this is the same code as SetupAngles,
		//only with a new radius and height for the specific band
		float3 heightMod = new float3 { y = newHeight };
		float circumference = radius * math.PI * 2;
		int segments = 1;
		while (circumference / segments > 1)
		{
			segments++;
		}
		segments--;

		if(segments == 0)
		{
			return;
		}
		//An array to store the positions of the band, before they are
		//inserted in the complete positions list
		NativeArray<float3> Ring = new NativeArray<float3>(segments, Allocator.Temp);

		float segmentAngle = 360f / (float)segments;
		for (int i = 0; i < segments; i++)
		{
			//Using triggettan, or Pythagorean identity, this finds all the
			//points along the radius of the current band where a cube can fit
			Ring[i] = new float3{	x = math.sin(math.radians(i * segmentAngle)),
									z = math.cos(math.radians(i * segmentAngle))
								} * radius + StartPosition + heightMod;
		}
		PositionList.AddRange(Ring);
		Ring.Dispose();
	}
}
