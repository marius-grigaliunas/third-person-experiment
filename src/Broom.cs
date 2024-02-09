using Godot;
using System;
using System.Buffers;

public partial class Broom : CharacterBody3D
{

	[Export] public Main manager;

	public bool isActive = false;

	[Export] float min_flight_speed = 2;
	[Export] float max_flight_speed = 50;
	[Export] float turn_speed = 0.75f;
	[Export] float pitch_speed = 0.5f;
	[Export] float level_speed = 3.0f;
	[Export] float throttle_delta = 30f;
	[Export] float acceleration = 10f;

	[Export] float forward_speed = 0;
	[Export] float target_speed = 0;
	bool grounded = false;
	Godot.Vector3 velocity = Vector3.Zero;
	float turn_input = 0;
	float pitch_input = 0;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
		if(isActive) {
			getInput(delta);
			GD.Print("mobe");
			forward_speed = (float)Mathf.Lerp((double)forward_speed, (double)target_speed, (double)acceleration * delta);
			Velocity = -Transform.Basis.Z * forward_speed;
			GD.Print(Velocity);
			MoveAndSlide();
		}

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
			float limit = grounded ? 0 : min_flight_speed;
			target_speed = Mathf.Max(forward_speed - throttle_delta * (float)delta, limit); 
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
