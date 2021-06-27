using Godot;
using System;

public class IEnemy : Node2D
{
	public enum Side
	{
		Top = 0,
		Left,
		Right,
		Bottom
	};

	public Guid NodeGuid = Guid.NewGuid();
	public Side EntranceSide = Side.Top;
	public int StartingX = 0;
	public int StartingY = 0;
	public Random Rnd = new Random();
	public int Health = 0;
	public int TotalHealth = 0;
	public float HitCoolDownTimer = 0.0f;
	public float HitAnimationTimer = 0.0f;
	public int DamageToTake = 0;
	public Area2D Area;

	public override void _Ready()
	{

	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
