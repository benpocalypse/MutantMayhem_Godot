using Godot;
using System;

public class Boss1 : IEnemy
{
    private enum MovementPhase
    {
        JustEntered,
        MovingInCircle,
        Hit
    };

    // FIXME - Have the position of the player be dynamic/calculable.
    private Vector2 playerPosition = new Vector2(640, 360);
    private Vector2 oldPosition = new Vector2(0, 0);
    private Vector2 positionDifference = new Vector2(0, 0);
    private float currentRadius = 250;
    private const float originalRadius = 250;
    private const float radiusLimit = 20;
    private float waveSpeed = 1f;
    private float moveSpeed = 0.55f;
    private float currentAngle = 270.0f;

    private MovementPhase Phase = MovementPhase.JustEntered;

    public override void _Ready()
    {
        this.Position = new Vector2(Generic2dGame.ScreenWidth / 2, -80);

        Health = 10;

        var sprite = this.GetNode("Sprite");
        Area = ((Area2D)sprite.GetNode("Area2D"));

        //currentAngle = Rnd.Next(0, 360);

        //oldPosition = PointOnCircle(radius, currentAngle, playerPosition);
        //Position = oldPosition;
        //LookAt(PointOnCircle(radius, currentAngle - 10, playerPosition));

        positionDifference = playerPosition - this.Position;

        this.GetParent().GetNode("Player").Connect("Hit", this, nameof(Hit));
    }


    public override void _Process(float delta)
    {
        switch (Phase)
        {
            case MovementPhase.JustEntered:
                if (this.Position.y < 120)
                {
                    this.Translate(positionDifference * moveSpeed * delta);
                    moveSpeed -= 0.005f;
                }
                else
                {
                    Phase = MovementPhase.MovingInCircle;
                    oldPosition = this.Position;
                    moveSpeed = 20.0f;
                }
                break;

            case MovementPhase.MovingInCircle:
                var newPoint = PointOnCircle(currentRadius, currentAngle, playerPosition);
                //LookAt(PointOnCircle(radius, currentAngle - 10, playerPosition));
                Translate((newPoint - oldPosition));

                currentAngle += moveSpeed * delta;

                if (currentRadius >= originalRadius - radiusLimit)
                {
                    currentRadius -= waveSpeed * delta;
                }
                else
                {
                    if (currentRadius <= originalRadius + radiusLimit)
                    {
                        currentRadius += waveSpeed * delta;
                    }
                }

                //shrinkSpeed += delta;
                //moveSpeed += delta;

                oldPosition = newPoint;
                break;

            case MovementPhase.Hit:

                break;
        }


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

                // FIXME - Implement this.
                //Phase = MovementPhase.Hit;

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
