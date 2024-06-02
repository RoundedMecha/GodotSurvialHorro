using Godot;
using System;

public partial class Character : CharacterBody3D
{
	public float Speed = 5.0f;
	public const float CrouchSpeed = 2.5f;
	public const float JumpVelocity = 4.5f;
	public const float sensitivity = 0.01f;
	public bool crouched;
	
	
	[ExportGroup("Properties")]
	[Export]
	public Node3D head;
	[Export]
	public Camera3D cam;
	[Export]
	public AnimationPlayer ap;
	[Export]
	public ShapeCast3D shapeCast;
	
	public override void _Ready()
	{
		shapeCast.AddException(GetNode<CollisionObject3D>("DefaultCollisionShape")); 
		shapeCast.AddException(GetNode<CollisionObject3D>("CrouchedCollisionShape3D"));
	}
	
	public override void _Input(InputEvent @event) //_Input for when input exists only
	{
		if(@event is InputEventMouseMotion) //Handle Mouse Input
		{
			InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
			head.RotateY(-mouseMotion.Relative.X * sensitivity);
			cam.RotateX(-mouseMotion.Relative.Y * sensitivity);
			
			Vector3 cameraRot = cam.Rotation;
			cameraRot.X = Mathf.Clamp(cameraRot.X,Mathf.DegToRad(-80f),Mathf.DegToRad(80f));
			cam.Rotation = cameraRot;
		}
		else if(@event.IsActionPressed("Crouch")) //Handle Crouching
		{
			if (Input.IsActionPressed("Crouch"))
			{
				if(crouched == false)
				{
				Speed = CrouchSpeed;
				ap.Play("CrouchAnim");
				crouched = true;
				}
				else if(crouched == true)
				{
					if(crouched == true && shapeCast.IsColliding() == false)
					{
					ap.Play("unCrouchAnim");
					crouched = false;
					Speed = 5.0f;
					}
				}
			}	
		}
	}
	


	public override void _PhysicsProcess(double delta) //Called at a fixed rate
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
		Vector3 direction = (head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	
}
