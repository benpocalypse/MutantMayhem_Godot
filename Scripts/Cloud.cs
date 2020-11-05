using Godot;
using System;

public class Cloud : Node2D
{
	float speed = 0;
	
	public override void _Ready()
	{
		Random rnd = new Random();
		
		speed = rnd.Next(1, 10)*(-10);
		
		var sprite = ((Sprite)GetNode("Sprite"));
		sprite.Texture = ((Texture)GD.Load("res://Assets/Backgrounds/cloud" + rnd.Next(1, 4) + ".png"));
	
		this.Translate(new Vector2(1500, rnd.Next(1, 360) + rnd.Next(-100,100)));
	}


	public override void _Process(float delta)
	{
		this.Translate(new Vector2((speed*delta), 0));
		
		if (this.GetPosition().x < -200)
		{
			this.QueueFree();
		}
	}
}
