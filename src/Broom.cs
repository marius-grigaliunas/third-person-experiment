using Godot;
using System;
using System.Buffers;

public partial class Broom : CharacterBody3D
{

	[Export] public Main manager;

	public bool isActive = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Input(InputEvent @event)
	{
		if(isActive) {
			// float camSensitivity = manager.thirdPersonCharacter.camSensitivity;
			// float minCamPitch = manager.thirdPersonCharacter.minCamPitch;
			// float maxCamPitch = manager.thirdPersonCharacter.maxCamPitch;

			// Godot.Vector3 camRot = GetNode<Marker3D>("").RotationDegrees;
			// if (@event is InputEventMouseMotion mouseMotion)
			// {
			// 	camRot.Y -= mouseMotion.Relative.X * camSensitivity;
			// 	camRot.X -= mouseMotion.Relative.Y * camSensitivity;
			// }
			// camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch); //prevents camera from going endlessly around the player
			// GetNode<Marker3D>("camera_center").RotationDegrees = camRot;

			if(Input.IsActionJustPressed("exit_mode")) {
				manager._SetPlayerMode();
            }
		}
	}
}
