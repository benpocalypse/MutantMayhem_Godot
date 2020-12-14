using Godot;
using System;

public class Bomb : Node2D
{
	private enum Side
	{
		Top = 0,
		Left,
		Right,
		Bottom
	};

	private Side entranceSide = Side.Top;
	private float rotationSpeed = 0.0f;
	private int startingX = 0;
	private int startingY = 0;
	private Random rnd = new Random();
	private Vector2 playerPosition = new Vector2(640, 360);
	private Vector2 positionDifference;
	float speed = 0.25f;

	public override void _Ready()
	{
		entranceSide = ((Side)rnd.Next(0, 4));
		rotationSpeed = ((float)rnd.Next(-50,50))*0.1f;

		switch (entranceSide)
		{
			case Side.Top:
				startingX = rnd.Next(0, Generic2dGame.ScreenWidth);
				startingY = -80;
				break;

			case Side.Left:;
				startingX = -80;
				startingY = rnd.Next(0, Generic2dGame.ScreenHeight);
				break;

			case Side.Right:
				startingX = Generic2dGame.ScreenWidth + 80;
				startingY = rnd.Next(0, Generic2dGame.ScreenHeight);
				break;

			case Side.Bottom:
				startingX = rnd.Next(0, Generic2dGame.ScreenWidth);
				startingY = Generic2dGame.ScreenHeight + 80;
				break;
		}
		
		if (rnd.NextDouble() < 0.6)
		{
			((Sprite)this.GetNode("Sprite")).Texture = ((Texture)GD.Load("res://Assets/Enemies/Bomb1.png"));
		}
		else
		{
			((Sprite)this.GetNode("Sprite")).Texture = ((Texture)GD.Load("res://Assets/Enemies/Potion1.png"));
		}

		this.SetPosition(new Vector2(startingX, startingY));
		positionDifference = playerPosition - this.GetPosition();
	}


	public override void _Process(float delta)
	{
		this.Translate(positionDifference * speed * delta);
		this.Rotate(rotationSpeed*delta);
	}
}
