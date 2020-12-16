using Godot;
using System;

public class DirectAttackEnemy : IEnemy
{
	private float rotationSpeed = 0.0f;

	// TODO - This won't be true on levels where we're not flying.
	private Vector2 playerPosition = new Vector2(640, 360);
	private Vector2 positionDifference;
	private float speed = 0.25f;

	public override void _Ready()
	{
		EntranceSide = ((Side)Rnd.Next(0, 4));
		rotationSpeed = ((float)Rnd.Next(-50,50))*0.1f;

		switch (EntranceSide)
		{
			case Side.Top:
				StartingX = Rnd.Next(0, Generic2dGame.ScreenWidth);
				StartingY = -80;
				break;

			case Side.Left:;
				StartingX = -80;
				StartingY = Rnd.Next(0, Generic2dGame.ScreenHeight);
				break;

			case Side.Right:
				StartingX = Generic2dGame.ScreenWidth + 80;
				StartingY = Rnd.Next(0, Generic2dGame.ScreenHeight);
				break;

			case Side.Bottom:
				StartingX = Rnd.Next(0, Generic2dGame.ScreenWidth);
				StartingY = Generic2dGame.ScreenHeight + 80;
				break;
		}

		if (Rnd.NextDouble() < 0.8)
		{
			((Sprite)this.GetNode("Sprite")).Texture = ((Texture)GD.Load("res://Assets/Enemies/Bomb1.png"));
			Health = 1;
		}
		else
		{
			((Sprite)this.GetNode("Sprite")).Texture = ((Texture)GD.Load("res://Assets/Enemies/Potion1.png"));
			Health = 2;
		}

		Position = new Vector2(StartingX, StartingY);
		positionDifference = playerPosition - this.Position;

		var sprite = this.GetNode("Sprite");
		Area = ((Area2D)sprite.GetNode("Area2D"));

		this.GetParent().GetNode("Player").Connect("Hit", this, nameof(Hit));
	}

	public override void _Process(float delta)
	{
		base.Translate(positionDifference * speed * delta);
		base.Rotate(rotationSpeed*delta);

		if (HitCoolDownTimer > 0.0f)
		{
			HitCoolDownTimer -= delta;
		}
		else
		{
			if ((Area.GetOverlappingAreas().Count) > 0 && (DamageToTake > 0))
			{
				Hit(DamageToTake, NodeGuid.ToString());
			}
		}

		if (HitAnimationTimer > 0.0f)
		{
			var sprite = this.GetNode<Sprite>("Sprite");
			sprite.SelfModulate = new Color(0.5f - HitAnimationTimer, 0.5f - HitAnimationTimer, 0.5f - HitAnimationTimer);

			HitAnimationTimer -= delta;
		}
		else
		{
			HitAnimationTimer = 0.0f;
			var sprite = this.GetNode<Sprite>("Sprite");
			sprite.SelfModulate = new Color(1, 1, 1);
		}
	}

	public void Hit(int damage, string guid)
	{
		if (guid == NodeGuid.ToString())
		{
			if (HitCoolDownTimer <= 0.0f)
			{
				Health -= damage;

				if (Health > 0)
				{
					HitAnimationTimer = 0.2f;
					HitCoolDownTimer = 0.5f;
					DamageToTake = damage;
					this.GetNode<AudioStreamPlayer2D>("HitSound").Play();
				}
				else
				{
					var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
					Node2D explosionInstance = (Node2D)explosion.Instance();
					var position = this.GlobalPosition;
					explosionInstance.Position = position;
					this.GetParent().AddChild(explosionInstance);

					var coin = (PackedScene)ResourceLoader.Load("res://Components/Coin.tscn");
					Coin coinInstance = (Coin)coin.Instance();
					coinInstance.Position = position;

					var hud = this.GetParent().GetNode("HUD");

					if (Rnd.NextDouble() < 0.90)
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
}
