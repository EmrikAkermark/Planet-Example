using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System;
using UnityEngine;

public class DOPExplosionSource : SystemBase
{
	protected override void OnUpdate()
	{
		float3 Movement = new float3();
		float deltaTime = Time.DeltaTime;

		Entities.ForEach((ref Translation translation, in InputData inputData) =>
		{
			bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
			bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
			bool isUpKeyPressed = Input.GetKey(inputData.upKey);
			bool isDownKeyPressed = Input.GetKey(inputData.downKey);
			bool isInKeyPressed = Input.GetKey(inputData.inKey);
			bool isOutKeyPressed = Input.GetKey(inputData.outKey);

			Movement.x = Convert.ToInt16(isRightKeyPressed);
			Movement.x -= Convert.ToInt16(isLeftKeyPressed);
			Movement.y = Convert.ToInt16(isUpKeyPressed);
			Movement.y -= Convert.ToInt16(isDownKeyPressed);
			Movement.z = Convert.ToInt16(isInKeyPressed);
			Movement.z -= Convert.ToInt16(isOutKeyPressed);

			if (Input.GetKey(inputData.turboKey))
			{
				Movement *= 130;
			}
			else
			{
				Movement *= 50;
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
