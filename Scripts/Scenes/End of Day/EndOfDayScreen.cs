using Godot;
using System;
using InputSystem;
using ServiceSystem;

public partial class EndOfDayScreen : Node2D {
    [Export]
    private AudioStreamPlayer _soundEffectPlayer;

    [Export]
    private Label _profitLabel;

    [Export]
    private Button _customerRarityUpgradeButton;

    [Export]
    private Button _merchandiseRarityUpgradeButton;

    [Export]
    private Button _nextDayButton;

    [Export]
    private Label _customerRarityLevelLabel;

    [Export]
    private Label _merchandiseRarityLevelLabel;

    [Export]
    private Label _customerRarityUpgradeCostLabel;

    [Export]
    private Label _merchandiseRarityUpgradeCostLabel;

    [Export]
    private Label _moneyLabel;

    private UpgradeService _upgradeService;
    private PlayerDataService _playerDataService;
    private int _moneyEarned;
    private int _moneyCounter;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        TransactionService transactionService = serviceLocator.GetService<TransactionService>(ServiceName.Transaction);
        _moneyEarned = transactionService.GetProfitFromDay();
        transactionService.AddProfitFromDayToPlayerMoney();

        _playerDataService = serviceLocator.GetService<PlayerDataService>(ServiceName.PlayerData);
        _upgradeService = serviceLocator.GetService<UpgradeService>(ServiceName.Upgrade);

        _customerRarityUpgradeButton.Pressed += () => { UpgradeRarity(UpgradeService.UpgradeType.CustomerRarity); };
        _merchandiseRarityUpgradeButton.Pressed += () => { UpgradeRarity(UpgradeService.UpgradeType.MerchandiseRarity); };
        _moneyLabel.Text = $"{_playerDataService.GetMoney()}";
        SetUpgradeCost(UpgradeService.UpgradeType.CustomerRarity);
        SetUpgradeCost(UpgradeService.UpgradeType.MerchandiseRarity);

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


    public void UpgradeRarity(UpgradeService.UpgradeType upgradeType) {
        _upgradeService.UpgradeRarity(upgradeType);
        SetUpgradeCost(upgradeType);
        _moneyLabel.Text = $"{_playerDataService.GetMoney()}";
    }

    public void SetUpgradeCost(UpgradeService.UpgradeType upgradeType) {
        switch (upgradeType) {
            case UpgradeService.UpgradeType.CustomerRarity:
                _customerRarityUpgradeCostLabel.Text = $"{_upgradeService.ComputeUpgradeCost(upgradeType)}";
                _customerRarityLevelLabel.Text = $"Customer Rarity Level: {_playerDataService.GetCustomerRarityUpgradeLevel()}";
                break;
            case UpgradeService.UpgradeType.MerchandiseRarity:
                _merchandiseRarityUpgradeCostLabel.Text = $"{_upgradeService.ComputeUpgradeCost(upgradeType)}";
                _merchandiseRarityLevelLabel.Text = $"Merchandise Rarity Level: {_playerDataService.GetMerchandiseRarityUpgradeLevel()}";
                break;
        }
    }
}