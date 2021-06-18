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

    [Signal]
	public delegate void BossDefeated();

    // FIXME - Have the position of the player be dynamic/calculable.
    private Vector2 playerPosition = new Vector2(640, 360);
    private Vector2 oldPosition = new Vector2(0, 0);
    private Vector2 positionDifference = new Vector2(0, 0);
    private float currentRadius = 270;
    private const float originalRadius = 270;
    private const float minimumRadiusLimit = 20;
    private const float shrinkSpeed = 5.0f;
    private bool movingClockwise = true;
    private float waveSpeed = 1f;
    private float moveSpeed = 0.55f;
    private float currentAngle = 270.0f;
    private float shootProjectileLimit = 5.0f;
    private float shootProjectileTimer = 0f;

    private MovementPhase Phase = MovementPhase.JustEntered;

    public override void _Ready()
    {
        this.Position = new Vector2(Generic2dGame.ScreenWidth / 2, -80);

        Health = 5;
        TotalHealth = 5;

        var sprite = this.GetNode("Sprite");
        Area = ((Area2D)sprite.GetNode("Area2D"));

        positionDifference = playerPosition - this.Position;

        this.GetParent().GetNode("Player").Connect("Hit", this, nameof(Hit));
    }


    public override void _Process(float delta)
    {
        switch (Phase)
        {
            // Slide down from the top.
            case MovementPhase.JustEntered:
                if (this.Position.y < 100)
                {
                    this.Translate(positionDifference * moveSpeed * delta);
                    moveSpeed -= 0.005f;
                }
                else
                {
                    oldPosition = this.Position;
                    moveSpeed = 30.0f;
                    Phase = MovementPhase.MovingInCircle;
                }
                break;

            // Move in increasingly smaller circles.
            case MovementPhase.MovingInCircle:
                var newPoint = PointOnCircle(currentRadius, currentAngle, playerPosition);

                // FIXME - get this to work
                //LookAt(PointOnCircle(currentRadius, currentAngle - 10, playerPosition));

                Translate((newPoint - oldPosition));

                // Continue to move in a circle.
                currentAngle += movingClockwise ?
                    moveSpeed * delta :
                    moveSpeed * delta * -1;

                // And shrink the circle based on how long we've been in this phase.
                if (currentRadius > minimumRadiusLimit)
                {
                    currentRadius -= shrinkSpeed * delta;
                }

                // Count up and then shoot fly's at the player!
                if (shootProjectileTimer < shootProjectileLimit)
                {
                    shootProjectileTimer += delta;
                }
                else
                {
                    shootProjectileTimer = 0.0f;

                    var dae = (PackedScene)ResourceLoader.Load("res://Components/DirectAttackEnemy.tscn");
                    DirectAttackEnemy daeInstance = (DirectAttackEnemy)dae.Instance();
                    daeInstance.SetVariety(3);
                    daeInstance.SetStartingPosition(newPoint);
                    AddChild(daeInstance);
                }

                oldPosition = newPoint;
                break;

            // Back up and change direction.
            case MovementPhase.Hit:
                currentRadius = originalRadius;

                newPoint = PointOnCircle(currentRadius, currentAngle, playerPosition);
                Translate((newPoint - oldPosition));

                oldPosition = newPoint;
                movingClockwise = !movingClockwise;

                shootProjectileTimer = 0.0f;

                Phase = MovementPhase.MovingInCircle;
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
                Phase = MovementPhase.Hit;
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

                var hud = this.GetParent().GetNode<HUD>("HUD");
                hud.SetBossHealthBarPercent(((float)Health) / ((float)TotalHealth));

                Phase = MovementPhase.Hit;

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

                    if (Rnd.NextDouble() < 0.90)
                    {
                        coinInstance.SetValue(1);
                        hud.AddCoin(1);
                    }
                    else
                    {
                        coinInstance.SetValue(5);
                        hud.AddCoin(5);
                    }

                    this.GetParent().AddChild(coinInstance);

                    EmitSignal(nameof(BossDefeated));

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
