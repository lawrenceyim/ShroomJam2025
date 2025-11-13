using Godot;
using System;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class EndOfDayScreen : Node2D {
    [Export]
    private AudioStreamPlayer _soundEffectPlayer;

    [Export]
    private Label _profitLabel;

    [Export]
    private Label _balanceLabel;

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
    private Label _currentDayLabel;

    private UpgradeService _upgradeService;
    private PlayerDataService _playerDataService;
    private SceneRepository _sceneRepository;
    private SoundEffectRepository _soundEffectRepository;
    private int _moneyEarned;
    private int _moneyCounter;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        TransactionService transactionService = serviceLocator.GetService<TransactionService>(ServiceName.Transaction);
        RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        _sceneRepository = repositoryLocator.GetRepository<SceneRepository>(RepositoryName.Scene);
        _soundEffectRepository = repositoryLocator.GetRepository<SoundEffectRepository>(RepositoryName.SoundEffect);
        _moneyEarned = transactionService.GetProfitFromDay();
        transactionService.AddProfitFromDayToPlayerMoney();

        _playerDataService = serviceLocator.GetService<PlayerDataService>(ServiceName.PlayerData);
        _upgradeService = serviceLocator.GetService<UpgradeService>(ServiceName.Upgrade);

        _customerRarityUpgradeButton.Pressed += () => { _UpgradeRarity(UpgradeService.UpgradeType.CustomerRarity); };
        _merchandiseRarityUpgradeButton.Pressed += () => { _UpgradeRarity(UpgradeService.UpgradeType.MerchandiseRarity); };
        _nextDayButton.Pressed += _NextDay;
        _UpdateUpgradeCostAndLevel();

        _PlaySoundEffect(SoundEffectId.EndOfDay);
        _UpdateBalance();
        _currentDayLabel.Text = $"{_playerDataService.GetDay()}";
    }

    public override void _PhysicsProcess(double delta) {
        if (_moneyCounter >= _moneyEarned) {
            return;
        }

        _moneyCounter = Math.Min(_moneyCounter + 25, _moneyEarned);
        _profitLabel.Text = $"Profit: {_moneyCounter}";
    }

    private void _UpgradeRarity(UpgradeService.UpgradeType upgradeType) {
        if (!_upgradeService.CanUpgrade(upgradeType)) {
            return;
        }

        _upgradeService.UpgradeRarity(upgradeType);
        _UpdateUpgradeCostAndLevel();
        _UpdateBalance();

        _PlaySoundEffect(SoundEffectId.SaleMade);
    }

    private void _UpdateBalance() {
        _balanceLabel.Text = $"{_playerDataService.GetMoney()}";
    }

    private void _UpdateUpgradeCostAndLevel() {
        int customerCost = _upgradeService.ComputeUpgradeCost(UpgradeService.UpgradeType.CustomerRarity);
        if (!_upgradeService.CanUpgrade(UpgradeService.UpgradeType.CustomerRarity)) {
            _customerRarityUpgradeCostLabel.SelfModulate = new Color(255, 0, 0, 1);
        }

        _customerRarityUpgradeCostLabel.Text = $"{customerCost}";
        _customerRarityLevelLabel.Text = _upgradeService.IsMaxLevel(UpgradeService.UpgradeType.CustomerRarity)
            ? "MAX"
            : $"{_playerDataService.GetCustomerRarityUpgradeLevel()}";

        int merchandiseCost = _upgradeService.ComputeUpgradeCost(UpgradeService.UpgradeType.MerchandiseRarity);
        if (!_upgradeService.CanUpgrade(UpgradeService.UpgradeType.MerchandiseRarity)) {
            _merchandiseRarityUpgradeCostLabel.SelfModulate = new Color(255, 0, 0, 1);
        }

        _merchandiseRarityUpgradeCostLabel.Text = $"{merchandiseCost}";
        _merchandiseRarityLevelLabel.Text = _upgradeService.IsMaxLevel(UpgradeService.UpgradeType.MerchandiseRarity)
            ? "MAX"
            : $"{_playerDataService.GetMerchandiseRarityUpgradeLevel()}";
    }

    private void _NextDay() {
        int day = _playerDataService.GetDay();
        if (day == 7) {
            GetTree().ChangeSceneToPacked(_sceneRepository.GetPackedScene(SceneId.GameOver));
        }

        _playerDataService.SetDay(day + 1);
        GetTree().ChangeSceneToPacked(_sceneRepository.GetPackedScene(SceneId.MainLevel));
    }

    private void _PlaySoundEffect(SoundEffectId soundEffectId) {
        if (GlobalSettings.MuteVolume) {
            return;
        }

        AudioStream soundEffect = _soundEffectRepository.GetSoundEffect(soundEffectId);
        _soundEffectPlayer.Stream = soundEffect;
        _soundEffectPlayer.Play();
    }
}