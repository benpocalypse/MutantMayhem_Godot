using Godot;
using System;

public class CreditsScreen : Node2D
{
	public override void _Ready()
	{

	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustReleased("ui_left") || Input.IsActionJustPressed("ui_accept") )
		{
			_on_BackButton_pressed();
		}
	}

	private void _on_BackButton_pressed()
	{
		this.GetNode<AudioStreamPlayer2D>("ButtonSound").Play();
	}
	
	private void _on_AudioStreamPlayer2D_finished()
	{
		var game = (Generic2dGame)GetNode("/root/Generic2dGame");
		game.GotoScene(Generic2dGame.Scenes.Titlescreen);
	}
}
