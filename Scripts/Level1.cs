using Godot;
using System;

public class Level1 : Node
{
	private ulong time = 0;
	private ulong addSpeed = 3;
	private float ticks = 0.0f;
	private float totalTicksForLevel = 90.0f;
	private int numEnemies = 0;
	private bool fightingBoss = false;
	private Random Rnd = new Random();

	private Generic2dGame game;

	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");

		GetNode("LevelControls").Connect("On_LeftButton_pressed", this, nameof(On_LeftButton_pressed));
		GetNode("LevelControls").Connect("On_LeftButton_released", this, nameof(On_LeftButton_released));
		GetNode("LevelControls").Connect("On_RightButton_pressed", this, nameof(On_RightButton_pressed));
		GetNode("LevelControls").Connect("On_RightButton_released", this, nameof(On_RightButton_released));

		GetNode("Player").Connect("HeadHit", this, nameof(Player_HeadHit));

		GetNode("HUD").Connect("PlayerDied", this, nameof(Player_Died));
		GetNode("HUD").Connect("LevelComplete", this, nameof(LevelComplete));
		
		// FIXME - This doesn't work, it plays forever.
		GetNode("WinMusic").Connect("finished", this, nameof(_on_WinMusic_finished));

		game.RestorePersistedData();
		game.FirstTimePlaying = false;

		PopulateClouds(prePopulate: true);

		LevelComplete();
	}

	public override void _Process(float delta)
	{
		ticks += delta;

		if (ticks > 1.0f)
		{
			time++;
			ticks = 0.0f;

			this.GetNode<HUD>("HUD").SetProgress((float)time/totalTicksForLevel);
		}

		if ( ((time % 3) == 0) && (ticks == 0.0f) )
		{
			PopulateClouds(prePopulate: false);
		}

		if (((time % addSpeed) == 0) && (ticks == 0.0f) && !fightingBoss)
		{
			if (numEnemies < 10)
			{
				AddDirectAttackEnemy(1);
			}
			else if ((numEnemies >= 10) && (numEnemies < 11))
			{
				if (Rnd.NextDouble() < 0.8)
				{
					AddDirectAttackEnemy(1);
				}
				else
				{
					AddDirectAttackEnemy(2);
				}
				addSpeed -= 1;
			}
			else if (numEnemies < 20)
			{
				if (Rnd.NextDouble() < 0.8)
				{
					AddDirectAttackEnemy(1);
				}
				else
				{
					AddDirectAttackEnemy(2);
				}
			}
			else if ((numEnemies >= 20) && (numEnemies < 21))
			{
				if (Rnd.NextDouble() < 0.6)
				{
					AddDirectAttackEnemy(1);
				}
				else
				{
					if (Rnd.NextDouble() < 0.6)
					{
						AddDirectAttackEnemy(2);
					}
					else
					{
						AddCircularAttackEnemy();
					}
				}
				addSpeed -= 1;
			}
			else
			{
				if (Rnd.NextDouble() < 0.6)
				{
					AddDirectAttackEnemy(1);
				}
				else
				{
					if (Rnd.NextDouble() < 0.8)
					{
						AddDirectAttackEnemy(2);
					}
					else
					{
						AddCircularAttackEnemy();
					}
				}
			}

			numEnemies++;
		}
		else // We're fighting the boss!
		{
			//var boss = this.GetNode<Boss1>("Boss1");
			//this.GetNode<HUD>("HUD").SetBossHealthBarPercent((float)((float)boss.Health / (float)boss.TotalHealth));
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

	private void Player_HeadHit()
	{
		((HUD)GetNode("HUD")).SubtractOneHealth();
	}

	private void Boss_Defeated()
	{
		this.GetNode<AudioStreamPlayer2D>("Boss1Music").Stop();
		this.GetNode<AudioStreamPlayer2D>("WinMusic").Play();
		this.GetNode<Timer>("WinMusicTimer").Start();
	}

	private void Player_Died()
	{
		game.PlayerScore = ((HUD)GetNode("HUD")).GetCoins();

		var explosion = (PackedScene)ResourceLoader.Load("res://Components/Explosion.tscn");
		Node2D explosionInstance = (Node2D)explosion.Instance();
		var position = this.GetNode<Node2D>("Player").GlobalPosition;
		explosionInstance.Position = position;
		this.AddChild(explosionInstance);

		this.GetNode<AudioStreamPlayer2D>("PlayerDiedSound").Play();
	}

	private void LevelComplete()
	{
		game.PlayerScore = ((HUD)GetNode("HUD")).GetCoins();

		if (game.PlayerScore > game.HighestScore)
		{
			game.HighestScore = game.PlayerScore;
		}

		game.PlayerHealth = this.GetNode<HUD>("HUD").GetHealth();

		game.StorePersistedData();

		((AudioStreamPlayer2D)GetNode("Level1Music")).Playing = false;
		((AudioStreamPlayer2D)GetNode("Boss1Music")).Playing = true;

		this.GetNode<HUD>("HUD").ShowBossHealthBar();

		var boss = (PackedScene)ResourceLoader.Load("res://Components/Boss1.tscn");
		Boss1 bossInstance = (Boss1)boss.Instance();
		AddChild(bossInstance);

		this.GetNode("Boss1").Connect("BossDefeated", this, nameof(Boss_Defeated));

		fightingBoss = true;
	}

	// FIXME - Tie this to a timer, and not just on the sound completing.
	private void _on_PlayerDiedSound_finished()
	{
		GotoGameover();
	}
	
	private void _on_WinMusic_finished()
	{
		// FIXME - have this go to the credits screen, instead of game over.
		GotoGameover();
	}

	private void _on_WinMusicTimer_timeout()
	{
		GotoGameover();
	}

	private void GotoGameover()
	{
		game.PlayerScore = GetNode<HUD>("HUD").GetCoins();

		if (game.PlayerScore > game.HighestScore)
		{
			game.HighestScore = game.PlayerScore;
		}

		game.StorePersistedData();

		game.GotoScene(Generic2dGame.Scenes.Gameover);
	}

	private void PopulateClouds(bool prePopulate)
	{
		if (prePopulate == false)
		{
			var cloud = (PackedScene)ResourceLoader.Load("res://Components/Cloud.tscn");
			Cloud cloudInstance = (Cloud)cloud.Instance();
			AddChild(cloudInstance);
		}
		else
		{
			for (int i = 0; i < 20; i++)
			{
				var cloud = (PackedScene)ResourceLoader.Load("res://Components/Cloud.tscn");
				Cloud cloudInstance = (Cloud)cloud.Instance();
				cloudInstance.RandomizePosition();
				AddChild(cloudInstance);
			}
		}
	}

	private void AddDirectAttackEnemy(int variety)
	{
		var dae = (PackedScene)ResourceLoader.Load("res://Components/DirectAttackEnemy.tscn");
		DirectAttackEnemy daeInstance = (DirectAttackEnemy)dae.Instance();
		daeInstance.SetVariety(variety);
		AddChild(daeInstance);
	}

	private void AddCircularAttackEnemy()
	{
		var cae = (PackedScene)ResourceLoader.Load("res://Components/CircularAttackEnemy.tscn");
		CircularAttackEnemy caeInstance = (CircularAttackEnemy)cae.Instance();
		AddChild(caeInstance);
	}
}
