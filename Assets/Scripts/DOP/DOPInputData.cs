using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
public struct InputData : IComponentData
{

	public KeyCode upKey;
	public KeyCode downKey;
	public KeyCode rightKey;
	public KeyCode leftKey;
	public KeyCode inKey;
	public KeyCode outKey;
	public KeyCode turboKey;


}
