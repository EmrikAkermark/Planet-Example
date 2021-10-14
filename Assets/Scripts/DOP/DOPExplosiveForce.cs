using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Profiling;

public class DOPExplosiveForce : SystemBase
{

	private static readonly ProfilerMarker s_ExpForcepm = new ProfilerMarker("HOLY FUCK");

	protected override void OnUpdate()
	{
		s_ExpForcepm.Begin();
			Entities.ForEach((ref Translation translation, ref Rotation rotation, in ExplosionData exData, in UnitWeight unitWeight) =>
			{
				translation.Value = exData.OriginalPos + math.normalize(exData.OriginalPos - exData.ExplosionPos) * (math.max(200 - math.distance(exData.OriginalPos, exData.ExplosionPos), 0) / unitWeight.Value);
				rotation.Value = quaternion.LookRotation(math.normalize(exData.OriginalPos - exData.ExplosionPos), new float3 { x = 0, y = 1, z = 0 });
			}).ScheduleParallel();
		s_ExpForcepm.End();
	}
}
