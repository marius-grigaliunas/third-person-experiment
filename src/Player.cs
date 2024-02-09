using System;
using System.Numerics;
using Godot;
using System.Threading.Tasks;

public partial class Player : CharacterBody3D
{
	public Godot.Vector3 direction;
	[Export] public Camera3D camera;
	[Export] public Main manager;
	[Export] public bool isActive = false;
	[Export] float speed = 5.0f;
	[Export] float jumpVelocity = 5f;
	[Export] float gravity = 14f;
	[Export] float pushForce = 5.0f;
	[Export(PropertyHint.Range, "0.1,1.0")] public float camSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] public float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] public float maxCamPitch = 30f;

	private bool canSwitchMode = false;

    public override void _Process(double delta)
	{
		if(isActive) {
			/**body rotation is in regular process because it lags in physicsprocess and is more a animation anyway
			maybe rotate extra collisions separately for invisible lag that may occur**/
			if (direction != Godot.Vector3.Zero)
			{
				Godot.Vector3 bodyRotation = GetNode<MeshInstance3D>("collision/body").Rotation;
				bodyRotation.Y = Mathf.LerpAngle(bodyRotation.Y,Mathf.Atan2(-direction.X, -direction.Z), (float)delta * speed);
				GetNode<MeshInstance3D>("collision/body").Rotation = bodyRotation;
			}


		}

		
	}
	public override void _PhysicsProcess(double delta)
	{
		if (isActive) {
			Godot.Vector3 velocity = Velocity;

			if (!IsOnFloor())
				velocity.Y -= gravity * (float)delta; //characterbodys don't have physic simulations by default like rigidbody
		
			if (Input.IsActionJustPressed("move_jump") && IsOnFloor())
				velocity.Y = jumpVelocity;

			Godot.Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
			direction = new Godot.Vector3(inputDir.X, 0, inputDir.Y).Rotated(Godot.Vector3.Up, GetNode<Marker3D>("camera_center").Rotation.Y).Normalized(); //rotates the input direction with camera rotation
			if (direction != Godot.Vector3.Zero)
			{
				velocity.X = direction.X * speed * (float)delta * 60;
				velocity.Z = direction.Z * speed * (float)delta * 60;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
			}
			Velocity = velocity;
			MoveAndSlide();

			for(int i = 0; i < GetSlideCollisionCount(); i++) {
				KinematicCollision3D collision = GetSlideCollision(i);
				if(collision.GetCollider().GetType() == typeof(RigidBody3D)) {
					RigidBody3D colliderRigidBody = (RigidBody3D)collision.GetCollider();
					colliderRigidBody.ApplyCentralImpulse(-collision.GetNormal() * pushForce);
				}
			}
		}
	}
	public override void _Input(InputEvent @event)
	{
		
			Godot.Vector3 camRot = GetNode<Marker3D>("camera_center").RotationDegrees;
			if (@event is InputEventMouseMotion mouseMotion)
			{
				camRot.Y -= mouseMotion.Relative.X * camSensitivity;
				camRot.X -= mouseMotion.Relative.Y * camSensitivity;
			}
			camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch); //prevents camera from going endlessly around the player
			GetNode<Marker3D>("camera_center").RotationDegrees = camRot;

		if(isActive) {
			if(Input.IsActionJustPressed("enter_mode") && canSwitchMode) {
				manager._SetBroomMode();
			}
		}
	}

	private void _on_volume_check_body_entered(Node3D body) {
		if(body.GetType() == typeof(Broom)) {
			canSwitchMode = true;
        }
	}

	private void _on_volume_check_body_exited(Node3D body) {
		if(body.GetType() == typeof(Broom)) {
            canSwitchMode = false;
        }
	}

	public void setChild(Node3D parent) {
		Node3D playerNode = GetNode<Node3D>("player");
		parent.AddChild(playerNode);
	}
}
