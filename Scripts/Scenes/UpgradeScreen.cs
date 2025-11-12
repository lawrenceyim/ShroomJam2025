using Godot;

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
}