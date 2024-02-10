using Godot;
using System;
using System.Buffers;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

public partial class Broom : CharacterBody3D
{

	[Export] public Main manager;

	public bool isActive = false;

	 public Godot.Vector3 direction;
	[Export] float min_flight_speed = 0;
	[Export] float max_flight_speed = 30;
	[Export] float turn_speed = 0.03f;
	[Export] float pitch_speed = 0.01f;
	[Export] float level_speed = 3.0f;
	[Export] float throttle_delta = 30f;
	[Export] float acceleration = 10f;

	[Export] public float forward_speed = 0;
	[Export] float target_speed = 0;
	[Export] float impulseForce = 6;
	bool grounded = false;
	Godot.Vector3 velocity = Vector3.Zero;
	float turn_input = 0;
	float pitch_input = 0;
	
	Marker3D cameraCenter;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(isActive) {

				
			/**body rotation is in regular process because it lags in physicsprocess and is more a animation anyway
			maybe rotate extra collisions separately for invisible lag that may occur**/
			if (direction != Godot.Vector3.Zero)
			{
				Godot.Vector3 bodyRotation = GetNode<MeshInstance3D>("player/collision/body").Rotation;
				bodyRotation.Y = Mathf.LerpAngle(bodyRotation.Y, Mathf.Atan2(-direction.X, -direction.Z), (float)delta * forward_speed);
				
				if(direction.Z < 0) {
					bodyRotation.X = Mathf.LerpAngle(bodyRotation.X, Mathf.Atan2(direction.Y, -direction.Z), (float)delta * forward_speed);
				} else {
					bodyRotation.X = Mathf.LerpAngle(bodyRotation.X, Mathf.Atan2(direction.Y, direction.Z), (float)delta * forward_speed);
				}

				GetNode<MeshInstance3D>("player/collision/body").Rotation = bodyRotation;
				bodyRotation.X += Mathf.DegToRad(90);
				GetNode<MeshInstance3D>("Body").Rotation = bodyRotation;
			}
			

			// 			float normalizedAngle = Mathf.Abs(Mathf.RadToDeg(cameraCenter.Rotation.Y % (Mathf.Pi*2)));

			// ///Set the player's direction according to the camera's rotation around X axis
			// if(normalizedAngle >= 0  && normalizedAngle <= 90 || normalizedAngle >= 270 && normalizedAngle <= 360) {
			// 	direction = direction.Rotated(Godot.Vector3.Right, cameraCenter.Rotation.X).Normalized();
			// } else {
			// 	direction = direction.Rotated(Godot.Vector3.Left, cameraCenter.Rotation.X).Normalized();	
			// }

		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if(isActive) {
			cameraCenter = GetNode<Marker3D>("player/camera_center");

			getInput(delta);
			forward_speed = (float)Mathf.Lerp((double)forward_speed, (double)target_speed, (double)acceleration * delta);
			

			//keyboard controls
			//Velocity = -Transform.Basis.Z * forward_speed;

			// RotateObjectLocal(new Vector3(1, 0, 0), pitch_input * pitch_speed * forward_speed / 10f); 
			// RotateObjectLocal(new Vector3(0, 0, 1), turn_input * turn_speed);
			// MoveAndSlide();

			//Mouse Controls
			Godot.Vector3 velocity = Velocity;

			//set the player's direction according to the camera's rotation around the Y axis
			direction = new Godot.Vector3(0, 0, -1).Rotated(Godot.Vector3.Up, cameraCenter.Rotation.Y).Normalized(); //rotates the input direction with camera rotation

			float normalizedAngle = Mathf.Abs(Mathf.RadToDeg(cameraCenter.Rotation.Y % (Mathf.Pi*2)));

			///Set the player's direction according to the camera's rotation around X axis
			if(normalizedAngle >= 0  && normalizedAngle <= 90 || normalizedAngle >= 270 && normalizedAngle <= 360) {
				direction = direction.Rotated(Godot.Vector3.Right, cameraCenter.Rotation.X).Normalized();
			} else {
				direction = direction.Rotated(Godot.Vector3.Left, cameraCenter.Rotation.X).Normalized();	
			}

			if (direction != Godot.Vector3.Zero)
			{
				velocity.X = direction.X * forward_speed * (float)delta * 60;
				velocity.Z = direction.Z * forward_speed * (float)delta * 60;
				velocity.Y = direction.Y * forward_speed * (float)delta * 60;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, forward_speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, forward_speed);
			}
			Velocity = velocity;
			MoveAndSlide();

		} else { /// makes the broom fly out of inertia when the player exits it
			forward_speed = (float)Mathf.Lerp((double)forward_speed, (double)target_speed, (double)acceleration * delta);
			if (direction != Godot.Vector3.Zero)
			{
				velocity.X = direction.X * forward_speed * (float)delta * 60;
				velocity.Z = direction.Z * forward_speed * (float)delta * 60;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X*200, 0, forward_speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z*200, 0, forward_speed);
			}
			Velocity = velocity;
			MoveAndSlide();

		}

		for(int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision3D collision = GetSlideCollision(i);
			if(collision.GetCollider().GetType() == typeof(RigidBody3D)) {
				RigidBody3D colliderRigidBody = (RigidBody3D)collision.GetCollider();
				colliderRigidBody.ApplyCentralImpulse(-collision.GetNormal() * forward_speed * impulseForce);
			}
		}
		//GD.Print(Velocity);
	}

    public override void _Input(InputEvent @event)
	{
		if(isActive) {
			if(Input.IsActionJustPressed("exit_mode")) {
				manager._SetPlayerMode();
			}
		}
	}


	public void getInput(double delta) {
		if(Input.IsActionPressed("throttle_up")) {
			target_speed = Mathf.Min(forward_speed + throttle_delta * (float)delta, max_flight_speed);
		}
		if(Input.IsActionPressed("throttle_down")) {
			//float limit = grounded ? 0 : min_flight_speed;
			target_speed = Mathf.Max(forward_speed - throttle_delta * (float)delta, 0); 
		}

		turn_input = 0;
		if(forward_speed > 0.5) {
			turn_input -= Input.GetActionStrength("roll_right");
			turn_input += Input.GetActionStrength("roll_left");
		}

		pitch_input = 0;
		if(!grounded) {
			pitch_input -= Input.GetActionStrength("pitch_down");
		}
		if(forward_speed >= min_flight_speed) {
			pitch_input += Input.GetActionStrength("pitch_up");
		}
	}
}
