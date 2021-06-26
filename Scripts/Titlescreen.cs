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
			_on_StartButton_pressed();
		}

		if (Input.IsActionJustReleased("ui_right"))
		{
			
		}
	}

	private void _on_QuitButton_pressed()
	{
		this.GetNode<AudioStreamPlayer2D>("ButtonSound").Play();
		GetTree().Quit();
	}

	private void _on_StartButton_pressed()
	{
		startGameNext = true;
		this.GetNode<AudioStreamPlayer2D>("ButtonSound").Play();
	}

	private void _on_ButtonSound_finished()
	{
		if (startGameNext == true)
		{
			game.GotoScene(Generic2dGame.Scenes.CutsceneIntro);
		}
	}
}
