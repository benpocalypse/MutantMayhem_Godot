using Godot;
using System;

public class Gameover : Node2D
{
	private Generic2dGame game;

	public override void _Ready()
	{
		game = (Generic2dGame)GetNode("/root/Generic2dGame");
		((RichTextLabel)this.GetNode("ScoreText")).BbcodeText = $"Final Score:  {game.PlayerScore}";
	}
	
	public override void _Process(float delta)
	{
		if (Input.IsActionJustReleased("ui_left") || Input.IsActionJustPressed("ui_accept") )
		{
			_on_Button_button_down();
		}
	}

	private void _on_Button_button_down()
	{
		this.GetNode<AudioStreamPlayer2D>("ButtonSound").Play();
	}

	private void _on_ButtonSound_finished()
	{
		var game = (Generic2dGame)GetNode("/root/Generic2dGame");
		game.GotoScene(Generic2dGame.Scenes.Titlescreen);
	}
}
