using Godot;
using System;

public class Titlescreen : Node
{
	private Generic2dGame game;

	private float scaleFactor = 1f;
	private bool startGameNext = false;

	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
	}

	public override void _Process(float delta)
	{
		var foreground = (Sprite)GetNode("Foreground");
		scaleFactor += 0.1f;
		var factor = ((float)Math.Sin(((double)scaleFactor)));

		foreground.ApplyScale(new Vector2(1 + (factor/200), 1 + (factor/200)));

		if (Input.IsActionJustPressed("ui_accept"))
		{
			_on_StartGameButton_pressed();
		}

		if (Input.IsActionJustReleased("ui_right"))
		{
			_on_CreditsButton_pressed();
		}
	}
	
	private void _on_QuitGameButton_pressed()
	{
		GetTree().Quit();
	}

	private void _on_StartGameButton_pressed()
	{
		startGameNext = true;
		this.GetNode<AudioStreamPlayer2D>("ButtonSound").Play();
	}

	private void _on_ButtonSound_finished()
	{
		var game = (Generic2dGame)GetNode("/root/Generic2dGame");

		if (startGameNext == true)
		{
			game.GotoScene(Generic2dGame.Scenes.Level1);
		}
		else
		{
			game.GotoScene(Generic2dGame.Scenes.CreditsScreen);
		}
	}

	private void _on_CreditsButton_pressed()
	{
		this.GetNode<AudioStreamPlayer2D>("ButtonSound").Play();
	}
}
