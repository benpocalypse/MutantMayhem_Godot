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
	}
	
	private Side entranceSide = Side.Top;
	private float rotationSpeed = 0.0f;
	private int startingX = 0;
	private int startingY = 0;
	private Random rnd = new Random();
	
	private Generic2dGame;
	
	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
		
		entranceSide = ((Side)rnd.Next(0, 4));
		rotationSpeed = ((float)rnd.Next(-5,5));
		
		switch (entranceSide)
		{
			case Side.Top:
				startingX = rnd.Next(0, game.ScreenWidth);
				startingY = -80;
				break;
				
			case Side.Left:;
				startingX = -80;
				startingY = rnd.Next(0, game.ScreenHeight);
				break;
				
			case Side.Right:
				startingX = game.ScreenWidth + 80;
				startingY = rnd.Next(0, game.ScreenHeight);
				break;
		
			case Side.Bottom:
				startingX = rnd.Next(0, game.ScreenWidth);
				startingY = game.ScreenHeight + 80;
				break;
		}
	}


	public override void _Process(float delta)
	{
		  
	}
}
