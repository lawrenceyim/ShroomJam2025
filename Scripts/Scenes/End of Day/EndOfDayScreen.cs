using Godot;
using System;
using InputSystem;
using ServiceSystem;

public partial class EndOfDayScreen : Node2D {
    [Export]
    private AudioStreamPlayer _soundEffectPlayer;

    [Export]
    private Label _profitLabel;

    private int _moneyEarned;
    private int _moneyCounter;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        TransactionService transactionService = serviceLocator.GetService<TransactionService>(ServiceName.Transaction);
        _moneyEarned = transactionService.GetProfitFromDay();
        transactionService.AddProfitFromDayToPlayerMoney();


        if (!GlobalSettings.MuteVolume) {
            _soundEffectPlayer.Play();
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (_moneyCounter >= _moneyEarned) {
            return;
        }

        _moneyCounter = Math.Min(_moneyCounter + 25, _moneyEarned);
        _profitLabel.Text = $"Profit: {_moneyCounter}";
    }
}