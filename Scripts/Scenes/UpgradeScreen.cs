using Godot;
using ServiceSystem;

// Didn't have time to experiment with the input system for this class.
// Just quick glue code to glue together logic
public partial class UpgradeScreen : Node2D {
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

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _playerDataService = serviceLocator.GetService<PlayerDataService>(ServiceName.PlayerData);
        _upgradeService = serviceLocator.GetService<UpgradeService>(ServiceName.Upgrade);

        _customerRarityUpgradeButton.Pressed += () => { UpgradeRarity(UpgradeService.UpgradeType.CustomerRarity); };
        _merchandiseRarityUpgradeButton.Pressed += () => { UpgradeRarity(UpgradeService.UpgradeType.MerchandiseRarity); };
        _moneyLabel.Text = $"{_playerDataService.GetMoney()}";
        SetUpgradeCost(UpgradeService.UpgradeType.CustomerRarity);
        SetUpgradeCost(UpgradeService.UpgradeType.MerchandiseRarity);
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