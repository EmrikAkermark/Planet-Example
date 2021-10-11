using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DOPRotation : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		return Entities.WithName("RotatingCubes").ForEach(
			(ref Rotation rotation, in Translation transform) =>
			{ rotation.Value = quaternion.LookRotation(float3.zero - transform.Value, new float3(0, 1, 0)); 
			}).Schedule(inputDeps);
	}
}
