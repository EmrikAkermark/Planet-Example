using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DOPExplosionSource : SystemBase
{
	protected override void OnUpdate()
	{
		float3 ExplosionStartPos = new float3 { x = -150, y = 0, z = 0 };
		float3 ExplosionEndPos = new float3 { x = 150, y = 0, z = 0 };

		var factor = math.abs(math.sin((float)Time.ElapsedTime * 0.3f));

		Entities.ForEach((ref ExplosionData ExData) =>
		{
			ExData.ExplosionPos = math.lerp(ExplosionStartPos, ExplosionEndPos, factor);
		}).ScheduleParallel();
	}
}
