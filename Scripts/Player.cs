using Godot;
using System;

public class Player : Node2D
{
	public int PlayerHealth = 3;
	private float rotationInDegrees = 0.0f;
	private float rotationAcceleration = 0.0f;
	private const float rotationAccelerationDelta = 12.0f;
	private const float maxRotationAcceleration = 2f;
	private bool rotatingLeft = false;
	private bool rotatingRight = false;
	private string previousRotationDirection = string.Empty;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		HandleRotation(delta);
	}

	private void _on_HeadArea2D_area_entered(Area2D area)
	{
		// FIXME - have this trigger an explosion or something like that.
		area.QueueFree();
		
		if (PlayerHealth > 0)
		{
			PlayerHealth -= 1;
		}
		else
		{
			// FIXME - He dead.
		}
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
}



