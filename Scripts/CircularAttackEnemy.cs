using Godot;
using System;

public class CircularAttackEnemy : IEnemy
{
	private Vector2 playerPosition = new Vector2(640, 360);
	private Vector2 oldPosition = new Vector2(0, 0);
	private float radius = Generic2dGame.ScreenWidth/2;
	private float shrinkSpeed = 20f;
	private float speed = 20f;
	private float currentAngle = 0.0f;
	public override void _Ready()
	{
		EntranceSide = ((Side)Rnd.Next(0, 4));

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

		Health = 4;

		var sprite = this.GetNode("Sprite");
		Area = ((Area2D)sprite.GetNode("Area2D"));

		currentAngle = Rnd.Next(0, 360);

		oldPosition = PointOnCircle(radius, currentAngle, playerPosition);
		Position = oldPosition;
		LookAt(PointOnCircle(radius, currentAngle-10, playerPosition));

		this.GetParent().GetNode("Player").Connect("Hit", this, nameof(Hit));
	}


	public override void _Process(float delta)
	{
		var newPoint = PointOnCircle(radius, currentAngle, playerPosition);
		LookAt(PointOnCircle(radius, currentAngle-10, playerPosition));
		Translate((newPoint - oldPosition));

		currentAngle += speed*delta;
		radius -= shrinkSpeed*delta;

		shrinkSpeed += delta;
		speed += delta;

		oldPosition = newPoint;

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
			var sprite = this.GetNode<AnimatedSprite>("Sprite");
			sprite.SelfModulate = new Color(0.5f - HitAnimationTimer, 0.5f - HitAnimationTimer, 0.5f - HitAnimationTimer);

			HitAnimationTimer -= delta;
		}
		else
		{
			HitAnimationTimer = 0.0f;
			var sprite = this.GetNode<AnimatedSprite>("Sprite");
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

	private Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 center)
	{
		// Convert from degrees to radians via multiplication by PI/180
		float dx = (float)(radius * Math.Cos(angleInDegrees * Math.PI / 180F)) + center.x;
		float dy = (float)(radius * Math.Sin(angleInDegrees * Math.PI / 180F)) + center.y;

		return new Vector2(dx, dy);
	}
}
