using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System;
using UnityEngine;

public class DOPExplosionSource : SystemBase
{
	protected override void OnUpdate()
	{
		//float3 ExplosionStartPos = new float3 { x = -150, y = 0, z = 0 };
		//float3 ExplosionEndPos = new float3 { x = 150, y = 0, z = 0 };

		//var factor = math.abs(math.sin((float)Time.ElapsedTime * 0.3f));

		float3 Movement = new float3();
		float deltaTime = Time.DeltaTime;

		Entities.ForEach((ref Translation translation, in InputData inputData) =>
		{
			bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
			bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
			bool isUpKeyPressed = Input.GetKey(inputData.upKey);
			bool isDownKeyPressed = Input.GetKey(inputData.downKey);

			Movement.x = Convert.ToInt16(isRightKeyPressed);
			Movement.x -= Convert.ToInt16(isLeftKeyPressed);
			Movement.y = Convert.ToInt16(isUpKeyPressed);
			Movement.y -= Convert.ToInt16(isDownKeyPressed);

			if(Input.GetKey(inputData.turboKey))
			{
				Movement = Movement * 10;
			}

			Movement *= deltaTime;
			translation.Value += Movement;
		}).Run();

		Entities.ForEach((ref ExplosionData ExData) =>
		{
			ExData.ExplosionPos += Movement;
		}).ScheduleParallel();
	}
}
