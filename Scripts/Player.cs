using Godot;
using System;

public class Player : Node2D
{
	private Generic2dGame game;

	Random rnd = new Random();
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
		var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
		Node2D explosionInstance = (Node2D)explosion.Instance();
		var position = ((Node2D)area.GetParent().GetParent()).GlobalPosition;
		explosionInstance.SetPosition(position);
		this.GetParent().AddChild(explosionInstance);

		this.GetNode<AudioStreamPlayer2D>("DamageSound").Play();

		EmitSignal(nameof(HeadHit));
		area.GetParent().GetParent().QueueFree();
	}

	private void _on_RightArmArea2D_area_entered(Area2D area)
	{
		this.GetNode<AnimatedSprite>("RightArm").Play("default", false);

		var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
		Node2D explosionInstance = (Node2D)explosion.Instance();
		var position = ((Node2D)area.GetParent().GetParent()).GlobalPosition;
		explosionInstance.SetPosition(position);
		this.GetParent().AddChild(explosionInstance);

		var coin = (PackedScene)ResourceLoader.Load("res://Components/Coin.tscn");

		Coin coinInstance = (Coin)coin.Instance();
		coinInstance.SetPosition(position);
		var hud = area.GetParent().GetParent().GetParent().GetNode("HUD");

		if (rnd.NextDouble() < 0.90)
		{
			coinInstance.SetValue(1);
			((HUD)hud).AddCoin(1);
		}
		else
		{
			coinInstance.SetValue(5);
			((HUD)hud).AddCoin(5);
		}

		this.GetParent().AddChild(coinInstance);

		EmitSignal(nameof(RightArmHit));
		area.GetParent().GetParent().QueueFree();
	}

	private void _on_RightArmArea2D_area_exited(object area)
	{
		//this.GetNode<AnimatedSprite>("RightArm").Stop();
		//this.GetNode<AnimatedSprite>("RightArm").SetFrame(0);
	}

	private void _on_LeftArmArea2D_area_entered(Area2D area)
	{
		this.GetNode<AnimatedSprite>("LeftArm").Play("default", false);

		var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
		Node2D explosionInstance = (Node2D)explosion.Instance();
		var position = ((Node2D)area.GetParent().GetParent()).GlobalPosition;
		explosionInstance.SetPosition(position);
		this.GetParent().AddChild(explosionInstance);

		var coin = (PackedScene)ResourceLoader.Load("res://Components/Coin.tscn");

		Coin coinInstance = (Coin)coin.Instance();
		coinInstance.SetPosition(position);
		var hud = area.GetParent().GetParent().GetParent().GetNode("HUD");

		if (rnd.NextDouble() < 0.90)
		{
			coinInstance.SetValue(1);
			((HUD)hud).AddCoin(1);
		}
		else
		{
			coinInstance.SetValue(5);
			((HUD)hud).AddCoin(5);
		}

		this.GetParent().AddChild(coinInstance);

		EmitSignal(nameof(LeftArmHit));
		area.GetParent().GetParent().QueueFree();
	}

	private void _on_LeftArmArea2D_area_exited(object area)
	{
		//this.GetNode<AnimatedSprite>("LeftArm").Stop();
		//this.GetNode<AnimatedSprite>("LeftArm").SetFrame(0);
	}

	private void _on_LeftArm_animation_finished()
	{
		this.GetNode<AnimatedSprite>("LeftArm").Stop();
		this.GetNode<AnimatedSprite>("LeftArm").SetFrame(0);
	}

	private void _on_RightArm_animation_finished()
	{
		this.GetNode<AnimatedSprite>("RightArm").Stop();
		this.GetNode<AnimatedSprite>("RightArm").SetFrame(0);
	}
}
