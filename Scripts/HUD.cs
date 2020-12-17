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
	private float polygonAlpha = 0.0f;
	private bool PlayerDying = false;
	private bool removingHeart = false;
	private string heartToRemove = string.Empty;
	private int heartAnimationCounter = 0;
	private const int heartAnimationTime = 40;
	private bool paused = false;
	private Vector2 moneyBagOriginalScale;

	[Signal]
	public delegate void PlayerDied();

	public override void _Ready()
	{
		InitializeHud();

		Engine.TimeScale = 1.0f;

		moneyBagOriginalScale = ((Sprite)GetNode("MoneyBag")).Scale;
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			_on_TextureButton_button_down();
		}
	
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
			moneyBag.Scale = moneyBagOriginalScale;
		}

		if (heartAnimationCounter > 0)
		{
			GetNode<Node2D>(heartToRemove).GetNode<Sprite>("Sprite").SelfModulate = new Color(1, 1, 1, ((float)heartAnimationCounter/(float)heartAnimationTime));
			GetNode<Node2D>(heartToRemove).Translate(new Vector2(0, 2));
			heartAnimationCounter--;
		}
		else if (removingHeart)
		{
			removingHeart = false;
			GetNode(heartToRemove).CallDeferred("free");
		}

		if (PlayerDying)
		{
			Engine.TimeScale -= 0.01f;
			polygonAlpha += 0.01f;
			this.GetNode<Polygon2D>("BlackPolygon").Visible = true;
			this.GetNode<Polygon2D>("BlackPolygon").Color = new Color(0,0,0, polygonAlpha);
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
			removingHeart = true;
			heartToRemove = "Heart" + currentHealth.ToString();
			heartAnimationCounter  = heartAnimationTime;

			var heart = (PackedScene)ResourceLoader.Load("res://Components/Heart.tscn");
			Heart heartInstance = (Heart)heart.Instance();
			heartInstance.Translate(new Vector2(currentHealth*65, 50));
			var sprite = ((Sprite)heartInstance.GetNode("Sprite"));
			sprite.Texture = ((Texture)GD.Load("res://Assets/Misc/HUD/HeartEmpty" + rnd.Next(1, 4) + ".png"));
			heartInstance.Name = "Heart" + (currentHealth).ToString();
			((Node2D)heartInstance).ZIndex = 1;
			AddChild(heartInstance);

			currentHealth--;
		}
		else
		{
			PlayerDying = true;
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
		coins += amount;
	}

	public int GetCoins()
	{
		return coins;
	}

	private void _on_Area2D_area_entered(object area)
	{
		coinAnimationCounter = 50;
		((RichTextLabel)this.GetNode("MoneyText")).BbcodeText = $"[right]{coins}[/right]";
	}
	private void _on_TextureButton_button_down()
	{
		paused = !paused;
		
		if (paused)
		{
			Engine.TimeScale = 0.0f;
		}
		else
		{
			Engine.TimeScale = 1.0f;
		}
	}
}
