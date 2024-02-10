using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

public partial class Main : WorldEnvironment
{
	[Export]public Player thirdPersonCharacter;
	[Export]public Broom broom;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		thirdPersonCharacter.isActive = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("escape")) {
			GetTree().Quit();
		}


	}

	public void _SetBroomMode() {
		thirdPersonCharacter.isActive = false;
		broom.isActive = true;
		GD.Print("Broom mode");
		RemoveChild(thirdPersonCharacter);
		broom.AddChild(thirdPersonCharacter);
		thirdPersonCharacter.Position = Godot.Vector3.Zero;
		thirdPersonCharacter.GetNode<MeshInstance3D>("collision/body").Rotation = Godot.Vector3.Zero;
		thirdPersonCharacter.CollisionLayer = 32;
	}

	public void _SetPlayerMode() {
        thirdPersonCharacter.isActive = true;
        broom.isActive = false;

		GD.Print("Third person mode");
		broom.RemoveChild(thirdPersonCharacter);
		AddChild(thirdPersonCharacter);
		thirdPersonCharacter.Position = broom.Position + Godot.Vector3.One;
    }
}

