using Godot;
using System;

public class Player : Node2D
{
	Generic2dGame game;

	private float rotationInDegrees = 0.0f;
	private float rotationAcceleration = 0.0f;
	private const float rotationAccelerationDelta = 12.0f;
	private const float maxRotationAcceleration = 2f;
	private bool rotatingLeft = false;
	private bool rotatingRight = false;
	private string previousRotationDirection = string.Empty;

	[Signal]
	public delegate void LeftArmHit();

	[Signal]
	public delegate void RightArmHit();

	[Signal]
	public delegate void HeadHit();

	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
	}

	public override void _Process(float delta)
	{
		HandleRotation(delta);
	}

	public void RotateLeft()
	{
		rotatingLeft = true;
	}

	public void RotateRight()
	{
		rotatingRight = true;
	}

	public void StopRotation()
	{
		if (rotatingLeft == true)
		{
			previousRotationDirection = nameof(rotatingLeft);
		}
		else
		{
			previousRotationDirection = nameof(rotatingRight);
		}

		rotatingLeft = false;
		rotatingRight = false;
	}

	private void HandleRotation(float delta)
	{
		if (rotatingLeft == true)
		{
			if (rotationAcceleration < maxRotationAcceleration)
			{
				rotationAcceleration += rotationAccelerationDelta * delta;
			}

			rotationInDegrees -= rotationAcceleration;

			this.SetRotationDegrees(rotationInDegrees);
		}

		if (rotatingRight == true)
		{
			if (rotationAcceleration < maxRotationAcceleration)
			{
				rotationAcceleration += rotationAccelerationDelta * delta;
			}

			rotationInDegrees += rotationAcceleration;

			this.SetRotationDegrees(rotationInDegrees);
		}

		if (rotatingLeft == false && rotatingRight == false &&
			(rotationAcceleration > 0.0f) )
		{
			rotationAcceleration -= rotationAccelerationDelta * delta;

			rotationInDegrees += (previousRotationDirection == nameof(rotatingLeft)) ?
				-rotationAcceleration :
				rotationAcceleration;

			this.SetRotationDegrees(rotationInDegrees);
		}
	}

	private void _on_HeadArea2D_area_entered(Area2D area)
	{

		area.GetParent().GetParent().QueueFree();

		EmitSignal(nameof(HeadHit));
	}

	private void _on_RightArmArea2D_area_entered(Area2D area)
	{
		this.GetNode<AnimatedSprite>("RightArm").Play("default", false);
		area.GetParent().QueueFree();

		EmitSignal(nameof(RightArmHit));
	}

	private void _on_RightArmArea2D_area_exited(object area)
	{
		//this.GetNode<AnimatedSprite>("RightArm").Stop();
		//this.GetNode<AnimatedSprite>("RightArm").SetFrame(0);
	}

	private void _on_LeftArmArea2D_area_entered(Area2D area)
	{
		area.GetParent().QueueFree();
		this.GetNode<AnimatedSprite>("LeftArm").Play("default", false);

		EmitSignal(nameof(LeftArmHit));
	}

	private void _on_LeftArmArea2D_area_exited(object area)
	{
		//this.GetNode<AnimatedSprite>("LeftArm").Stop();
		//this.GetNode<AnimatedSprite>("LeftArm").SetFrame(0);
	}
}
