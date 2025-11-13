using Godot;
using System;

public partial class EndOfDayScreen : Node2D {
    [Export]
    private AudioStreamPlayer _soundEffectPlayer;

    [Export]
    private Label _profitLabel;

    private int _moneyEarned = 100;
    private int _moneyCounter;

    public override void _Ready() {
        if (!GlobalSettings.MuteVolume) {
            _soundEffectPlayer.Play();
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (_moneyCounter >= _moneyEarned) {
            return;
        }

        _moneyCounter++;
        _profitLabel.Text = $"Profit: {_moneyCounter}";
    }
}