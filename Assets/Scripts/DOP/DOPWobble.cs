using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DOPWobble : SystemBase
{
	protected override void OnUpdate()
	{
		var offset = math.radians((float)Time.ElapsedTime * 90);
		var factor = math.sin(offset) / 10;

		// Entities.ForEach is the default way of iterating over entities for processing.
		// Notice the ScheduleParallel at the end, this will run over multiple threads.
		Entities.ForEach((ref Translation translation, in ExplosionData ExData) =>
		{
			translation.Value = ExData.OriginalPos + math.normalize(ExData.ExplosionPos - ExData.OriginalPos) * factor * 1000 ;
		}).ScheduleParallel();
	}
}
