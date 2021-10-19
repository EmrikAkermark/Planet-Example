using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;

public class DOPExplosiveForce : SystemBase
{

	private static readonly ProfilerMarker s_ExpForcepm = new ProfilerMarker("Explosive Force");
	Settings settings;
	protected override void OnStartRunning()
	{
		settings = GetSingleton<Settings>();
	}

	protected override void OnUpdate()
	{
		float force = math.max(settings.ExplosiveForce, 1f);

		

		s_ExpForcepm.Begin();
		Entities.ForEach((ref Translation translation, ref Rotation rotation, in ExplosionData exData, in UnitWeight unitWeight) =>
		{
			translation.Value = exData.OriginalPos + math.normalize(exData.OriginalPos - exData.ExplosionPos) * (math.max(force - math.distance(exData.OriginalPos, exData.ExplosionPos), 0) / unitWeight.Value);
			rotation.Value = quaternion.LookRotation(math.normalize(exData.OriginalPos - exData.ExplosionPos), new float3 { x = 0, y = 1, z = 0 });
		}).ScheduleParallel();
		
		//This turns the EXplosion to an IMplosion, try it! Even better, can you make half the cubes explode, and the other implode?
		//Entities.ForEach((ref Translation translation, ref Rotation rotation, in ExplosionData exData, in UnitWeight unitWeight) =>
		//{
		//	translation.Value = exData.OriginalPos + math.normalize(exData.ExplosionPos - exData.OriginalPos) * (math.max(force - math.distance(exData.OriginalPos, exData.ExplosionPos), 0) / unitWeight.Value);
		//	rotation.Value = quaternion.LookRotation(math.normalize(exData.OriginalPos - exData.ExplosionPos), new float3 { x = 0, y = 1, z = 0 });
		//}).ScheduleParallel();
		s_ExpForcepm.End();
	}
}
