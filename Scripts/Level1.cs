using Godot;
using System;

public class Level1 : Node
{
	private ulong time = 0;
	private ulong difficulty = 200;
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

		PopulateClouds(prePopulate: true);
	}

	public override void _Process(float delta)
	{
		time++;

		if (time % 300 == 0)
		{
			PopulateClouds(prePopulate: false);
		}

		if (time % difficulty == 0)
		{
			if (Rnd.NextDouble() < 0.85)
			{
				AddDirectAttackEnemy();
			}
			else
			{
				AddCircularAttackEnemey();
			}

			if (difficulty > 70)
			{
				difficulty -= 5;
			}
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

	private void Player_Died()
	{
		game.PlayerScore = ((HUD)GetNode("HUD")).GetCoins();
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

	private void AddDirectAttackEnemy()
	{
		var dae = (PackedScene)ResourceLoader.Load("res://Components/DirectAttackEnemy.tscn");
		DirectAttackEnemy daeInstance = (DirectAttackEnemy)dae.Instance();
		AddChild(daeInstance);
	}

	private void AddCircularAttackEnemey()
	{
		var cae = (PackedScene)ResourceLoader.Load("res://Components/CircularAttackEnemey.tscn");
		CircularAttackEnemey caeInstance = (CircularAttackEnemey)cae.Instance();
		AddChild(caeInstance);
	}
}
