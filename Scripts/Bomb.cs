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

	public Guid NodeGuid = Guid.NewGuid();

	private Side entranceSide = Side.Top;
	private float rotationSpeed = 0.0f;
	private int startingX = 0;
	private int startingY = 0;
	private Random rnd = new Random();
	private Vector2 playerPosition = new Vector2(640, 360);
	private Vector2 positionDifference;
	private float speed = 0.25f;
	private int health = 0;
	private float hitCoolDownTimer = 0.0f;
	private int damageToTake = 0;
	private Area2D area;

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

		if (rnd.NextDouble() < 0.8)
		{
			((Sprite)this.GetNode("Sprite")).Texture = ((Texture)GD.Load("res://Assets/Enemies/Bomb1.png"));
			health = 1;
		}
		else
		{
			((Sprite)this.GetNode("Sprite")).Texture = ((Texture)GD.Load("res://Assets/Enemies/Potion1.png"));
			health = 2;
		}

		this.SetPosition(new Vector2(startingX, startingY));
		positionDifference = playerPosition - this.GetPosition();

		var sprite = this.GetNode("Sprite");
		area = ((Area2D)sprite.GetNode("Area2D"));

		this.GetParent().GetNode("Player").Connect("ArmHit", this, nameof(Hit));
		this.GetParent().GetNode("Player").Connect("HeadHit", this, nameof(HeadHit));
	}

	public override void _Process(float delta)
	{
		this.Translate(positionDifference * speed * delta);
		this.Rotate(rotationSpeed*delta);

		if (hitCoolDownTimer > 0.0f)
		{
			hitCoolDownTimer -= delta;
		}
		else
		{
			if ((area.GetOverlappingAreas().Count) > 0 && (damageToTake > 0))
			{
				Hit(damageToTake, NodeGuid.ToString());
			}
		}
	}

	public void Hit(int damage, string guid)
	{
		if (guid == NodeGuid.ToString())
		{
			if (hitCoolDownTimer <= 0.0f)
			{
				health -= damage;

				if (health > 0)
				{
					var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
					Node2D explosionInstance = (Node2D)explosion.Instance();
					var position = this.GlobalPosition;
					explosionInstance.SetPosition(position);
					this.GetParent().AddChild(explosionInstance);

					hitCoolDownTimer = 0.5f;
					damageToTake = damage;
				}
				else
				{
					var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
					Node2D explosionInstance = (Node2D)explosion.Instance();
					var position = this.GlobalPosition;
					explosionInstance.SetPosition(position);
					this.GetParent().AddChild(explosionInstance);

					var coin = (PackedScene)ResourceLoader.Load("res://Components/Coin.tscn");
					Coin coinInstance = (Coin)coin.Instance();
					coinInstance.SetPosition(position);

					var hud = this.GetParent().GetNode("HUD");

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

					CallDeferred("free");
				}
			}
		}
	}

	private void HeadHit()
	{
		var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
		Node2D explosionInstance = (Node2D)explosion.Instance();
		var position = this.GlobalPosition;
		explosionInstance.SetPosition(position);
		this.GetParent().AddChild(explosionInstance);

		this.GetNode<Sprite>("Sprite").Visible = false;

		CallDeferred("free");
	}
}
