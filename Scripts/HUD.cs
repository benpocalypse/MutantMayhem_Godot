using Godot;
using System;

public class HUD : Node2D
{
	Random rnd = new Random();

	private int maxHealth = 3;
	private int currentHealth = 3;
	private int coins = 0;
	private int coinAnimationCounter = 0;
	private float scaleFactor = 1.0f;
	private Vector2 moneyBagOriginalScale;

	[Signal]
	public delegate void PlayerDied();

	public override void _Ready()
	{
		InitializeHud();

		moneyBagOriginalScale = ((Sprite)GetNode("MoneyBag")).Scale;
	}

	public override void _Process(float delta)
	{
		if (coinAnimationCounter > 0)
		{
			var moneyBag = (Sprite)GetNode("MoneyBag");
			scaleFactor += 0.2f;
			var factor = ((float)Math.Sin(((double)scaleFactor)));

			moneyBag.ApplyScale(new Vector2(1 + (factor/30), 1 + (factor/30)));

			coinAnimationCounter--;
		}
		else
		{
			var moneyBag = (Sprite)GetNode("MoneyBag");
			scaleFactor = 1.0f;
			moneyBag.SetScale(moneyBagOriginalScale);
		}
	}

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
			GetNode("Heart" + currentHealth.ToString()).CallDeferred("free");

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

	public void AddCoin(int amount)
	{
		coinAnimationCounter = 50;
		coins += amount;
	}

	public int GetCoins()
	{
		return coins;
	}

	private void _on_Area2D_area_entered(object area)
	{
		coinAnimationCounter = 0;
		((RichTextLabel)this.GetNode("MoneyText")).BbcodeText = $"[right]{coins}[/right]";
	}
}
