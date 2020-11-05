using Godot;
using System;

public class Level1 : Node
{
	public ulong time = 0;
	Generic2dGame game;

	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
		
		GetNode("LevelControls").Connect("On_LeftButton_pressed", this, nameof(On_LeftButton_pressed));
		GetNode("LevelControls").Connect("On_LeftButton_released", this, nameof(On_LeftButton_released));
		GetNode("LevelControls").Connect("On_RightButton_pressed", this, nameof(On_RightButton_pressed));
		GetNode("LevelControls").Connect("On_RightButton_released", this, nameof(On_RightButton_released));
		
		/* FIXME - Pre-populate a bunch of clouds when the level starts
		
		var cloud = (PackedScene)ResourceLoader.Load("res://Components/Cloud.tscn");
		Node2D cloudInstance = (Node2D)cloud.Instance();
		AddChild(cloudInstance);
		
		*/
	}

	public override void _Process(float delta)
	{
		time++;
		
		if (time % 250 == 0)
		{
			var cloud = (PackedScene)ResourceLoader.Load("res://Components/Cloud.tscn");
			Node2D cloudInstance = (Node2D)cloud.Instance();
			AddChild(cloudInstance);
		}
	}

	private void On_LeftButton_pressed()
	{
		((Player)GetNode("Player")).RotateLeft();
	}

	private void On_LeftButton_released()
	{
		((Player)GetNode("Player")).StopRotation();
	}
	
	private void On_RightButton_pressed()
	{
		((Player)GetNode("Player")).RotateRight();
	}
	
	private void On_RightButton_released()
	{
		((Player)GetNode("Player")).StopRotation();
	}
}
