using Godot;
using System;

public class HUD : Node2D
{
	Random rnd = new Random();

	private int maxHealth = 3;
	private int currentHealth = 3;
	private int coins = 0;

	[Signal]
	public delegate void PlayerDied();

	public override void _Ready()
	{
		InitializeHud();
	}

/*
	public override void _Process(float delta)
	{

	}
*/

	private void InitializeHud()//int _currentHealth, int _maxHealth, int _coins)
	{
		for (int i = 0; i < currentHealth; i++)
		{
			var heart = (PackedScene)ResourceLoader.Load("res://Components/Heart.tscn");
			Heart heartInstance = (Heart)heart.Instance();
			heartInstance.Translate(new Vector2((i+1)*65, 50));
			var sprite = ((Sprite)heartInstance.GetNode("Sprite"));
			sprite.Texture = ((Texture)GD.Load("res://Assets/Misc/HUD/HeartFull" + rnd.Next(1, 4) + ".png"));
			heartInstance.Name = "Heart" + (i + 1).ToString();
			AddChild(heartInstance);
		}
	}

	public void SetMaxHealth(int _maxHealth)
	{
		maxHealth = _maxHealth;
	}

	public void SetCurrentHealth(int health)
	{
		currentHealth = health;
	}

	public void SubtractOneHealth()
	{
		if (currentHealth > 1)
		{
			GetNode("Heart" + currentHealth.ToString()).QueueFree();

			var heart = (PackedScene)ResourceLoader.Load("res://Components/Heart.tscn");
			Heart heartInstance = (Heart)heart.Instance();
			heartInstance.Translate(new Vector2(currentHealth*65, 50));
			var sprite = ((Sprite)heartInstance.GetNode("Sprite"));
			sprite.Texture = ((Texture)GD.Load("res://Assets/Misc/HUD/HeartEmpty" + rnd.Next(1, 4) + ".png"));
			heartInstance.Name = "Heart" + (currentHealth).ToString();
			AddChild(heartInstance);

			currentHealth--;
		}
		else
		{
			EmitSignal(nameof(PlayerDied));
		}
	}

	public void AddOneHealth()
	{
		if (currentHealth < maxHealth)
		{
			currentHealth++;
		}
	}
}
