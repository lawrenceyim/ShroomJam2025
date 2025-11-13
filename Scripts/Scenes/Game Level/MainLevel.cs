using Godot;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class MainLevel : Node, IInputState, ITick {
    [Export]
    private Camera2D _customerViewCamera;

    [Export]
    private CustomerView _customerView;

    [Export]
    private Camera2D _shelfViewCamera;

    [Export]
    private ShelfView _shelfView;

    [Export]
    private HeldMerchandiseDisplay _handCustomerView;

    [Export]
    private HeldMerchandiseDisplay _handShelfView;

    [Export]
    private SettingsMenu _customerSettingsMenu;

    [Export]
    private SettingsMenu _shelfSettingsMenu;

    [Export]
    private Sprite2D _toCustomerArrow;

    [Export]
    private Sprite2D _toShelfArrow;

    [Export]
    private Area2D _toCustomerArrowArea;

    [Export]
    private Area2D _toShelfArrowArea;

    [Export]
    private AudioStreamPlayer _musicPlayer;

    [Export]
    private AudioStreamPlayer _soundEffectPlayer;

    private const string SwitchView = "Space";
    private const string Pause = "Escape";

    private const int SecondsPerDay = 120;
    private readonly TickTimer _dayTimer = new();
    private readonly Color _originalArrowColor = new(1, 1, 1, 1);
    private readonly Color _highlightedArrowColor = new(1, 0, 1, 1);
    private readonly BlinkingComponent _toShelfBlinkingComponent = new();
    private readonly BlinkingComponent _toCustomerBlinkingComponent = new();
    private readonly int _ticksPerSeconds = Engine.PhysicsTicksPerSecond; // Avoid latency from marshalling
    private ServiceLocator _serviceLocator;
    private PackedSceneRepository _packedSceneRepository;
    private InputStateMachine _inputStateMachine;
    private TransactionService _transactionService;
    private MerchandiseService _merchandiseService;
    private Texture2dRepository _texture2dRepository;
    private SceneRepository _sceneRepository;
    private SoundEffectRepository _soundEffectRepository;
    private GameClock _gameClock;
    private ActiveView _activeView = ActiveView.CustomerView;
    private bool _toCustomerArrowHovered = false;
    private bool _toShelfArrowHovered = false;

    public override void _Ready() {
        _serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        PlayerDataService playerDataService = _serviceLocator.GetService<PlayerDataService>(ServiceName.PlayerData);
        _merchandiseService = _serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise);
        _inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
        _gameClock = _serviceLocator.GetService<GameClock>(ServiceName.GameClock);
        _transactionService = _serviceLocator.GetService<TransactionService>(ServiceName.Transaction);
        _soundEffectRepository = repositoryLocator.GetRepository<SoundEffectRepository>(RepositoryName.SoundEffect);

        _transactionService.Initialize(_merchandiseService, playerDataService);
        _merchandiseService.RestockMerchandise();
        _gameClock.AddActiveScene(this, GetInstanceId());
        _inputStateMachine.SetState(this);
        _packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene);
        _texture2dRepository = repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture);
        _sceneRepository = repositoryLocator.GetRepository<SceneRepository>(RepositoryName.Scene);

        _shelfView.Initialize(
            _merchandiseService,
            _texture2dRepository
        );
        _customerView.Initialize(
            _texture2dRepository,
            playerDataService
        );
        _toCustomerBlinkingComponent.Instantiate(_toCustomerArrow, 1, _originalArrowColor, _highlightedArrowColor);
        _toShelfBlinkingComponent.Instantiate(_toShelfArrow, 1, _originalArrowColor, _highlightedArrowColor);
        _toCustomerArrowArea.MouseEntered += () => _toCustomerArrowHovered = true;
        _toShelfArrowArea.MouseEntered += () => _toShelfArrowHovered = true;
        _toCustomerArrowArea.MouseExited += () => _toCustomerArrowHovered = false;
        _toShelfArrowArea.MouseExited += () => _toShelfArrowHovered = false;

        _DisplayHand(false);
        _ChangeAudioVolume();

        _dayTimer.StartFixedTimer(false, SecondsPerDay * _ticksPerSeconds);
        _dayTimer.TimedOut += _EndDay;
        _gameClock.SetPauseState(false);

        _customerSettingsMenu.ResumeDay += Unpause;
        _shelfSettingsMenu.ResumeDay += Unpause;
        _customerSettingsMenu.RestartDay += _RestartDay;
        _shelfSettingsMenu.RestartDay += _RestartDay;
        _customerSettingsMenu.AudioVolumeChange += _ChangeAudioVolume;
        _shelfSettingsMenu.AudioVolumeChange += _ChangeAudioVolume;
        _customerView.PlaySoundEffect += _PlaySoundEffect;
    }

    public override void _ExitTree() {
        _gameClock.RemoveActiveScene(GetInstanceId());
        _toCustomerBlinkingComponent.KillTween();
        _toShelfBlinkingComponent.KillTween();
    }

    public void PhysicsTick(double delta) {
        _dayTimer.PhysicsTick(delta);
        _customerView.PhysicsTick(delta);
        _customerView.UpdateDayTimer(_dayTimer.GetTicksLeft());
    }

    public void ProcessInput(InputEventDto eventDto) {
        switch (eventDto) {
            case KeyDto keyDto:
                ProcessKeyInput(keyDto);
                break;
            case MouseButtonDto mouseButtonDto:
                _ProcessMouseButtonInput(mouseButtonDto);
                break;
        }
    }

    private void ProcessKeyInput(KeyDto dto) {
        if (!dto.Pressed) {
            return;
        }

        switch (dto.Identifier) {
            case Pause:
                PauseGame();
                break;
            case SwitchView:
                _SwitchView();
                break;
        }
    }

    private void PauseGame() {
        _gameClock.SetPauseState(true);

        switch (_activeView) {
            case ActiveView.CustomerView:
                _inputStateMachine.SetState(_customerSettingsMenu);
                _customerSettingsMenu.Display();
                break;
            case ActiveView.ShelfView:
                _inputStateMachine.SetState(_shelfSettingsMenu);
                _shelfSettingsMenu.Display();
                break;
        }
    }

    private void Unpause() {
        _inputStateMachine.SetState(this);
        _gameClock.SetPauseState(false);
        _customerSettingsMenu.Hide();
        _shelfSettingsMenu.Hide();
    }

    private void _ProcessMouseButtonInput(MouseButtonDto dto) {
        if (!dto.Pressed) {
            return;
        }

        switch (_activeView) {
            case ActiveView.CustomerView:
                if (_toShelfArrowHovered) {
                    _SwitchView();
                    return;
                }

                if (_IsTransactionValid()) {
                    _SellHeldMerchandise();
                }

                break;
            case ActiveView.ShelfView:
                if (_toCustomerArrowHovered) {
                    _SwitchView();
                    return;
                }

                Vector2I? hoveredDvdSlot = _shelfView.GetHoveredSlot();
                if (hoveredDvdSlot != null) {
                    _SwapMerchandise(hoveredDvdSlot.Value);
                }

                break;
        }
    }

    private void _SwitchView() {
        if (_customerViewCamera.IsCurrent()) {
            _shelfViewCamera.MakeCurrent();
            _activeView = ActiveView.ShelfView;
        } else if (_shelfViewCamera.IsCurrent()) {
            _customerViewCamera.MakeCurrent();
            _activeView = ActiveView.CustomerView;
        }
    }

    private void _SellHeldMerchandise() {
        CustomerSaleDto saleDto = _customerView.GetCustomerSale();
        Merchandise heldMerchandise = _merchandiseService.GetHeldMerchandise();
        int profit = _transactionService.SellMerchandise(saleDto);
        _customerView.MerchandiseSold(profit);
        _DisplayHand(false);
        _customerView.SetHoldingItem(false);
        _merchandiseService.SetMerchandiseCount(_merchandiseService.GetMerchandiseCount() - 1);

        if (saleDto.customerMood == CustomerMood.Happy
            && saleDto.colorWanted == heldMerchandise.Color
            && saleDto.merchandiseTypeWanted == heldMerchandise.Type) {
            _PlaySoundEffect(SoundEffectId.GreatSaleMade);
        } else {
            _PlaySoundEffect(SoundEffectId.SaleMade);
        }

        if (_merchandiseService.GetMerchandiseCount() == 0) {
            _EndDay();
        }
    }

    private void _SetShelfMerchandiseTexture(Vector2I position, Texture2D texture) {
        _shelfView.SetMerchandiseTexture(position, texture);
    }

    private void _SwapMerchandise(Vector2I position) {
        Merchandise heldMerchandise = _merchandiseService.GetHeldMerchandise();
        Merchandise merchandiseInSlot = _merchandiseService.GetMerchandiseFromShelf(position);
        _merchandiseService.SetHeldMerchandise(merchandiseInSlot);
        _merchandiseService.SetShelfMerchandise(heldMerchandise, position);
        _shelfView.RefreshShelfMerchandiseTexture(position);
        _RefreshHandDisplay();
        _customerView.SetHoldingItem(_merchandiseService.GetHeldMerchandise() is not null);

        if (_merchandiseService.GetHeldMerchandise() is not null) {
            _PlaySoundEffect(SoundEffectId.MerchandisePickedUp);
        } else {
            _PlaySoundEffect(SoundEffectId.MerchandisePutDown);
        }
    }

    private void _EndDay() {
        GetTree().ChangeSceneToPacked(_sceneRepository.GetPackedScene(SceneId.EndOfDay));
    }

    private void _DisplayHand(bool visible) {
        _handCustomerView.Visible = visible;
        _handShelfView.Visible = visible;
    }

    private void _RefreshHandDisplay() {
        Merchandise held = _merchandiseService.GetHeldMerchandise();

        if (held is null) {
            _DisplayHand(false);
            return;
        }

        _DisplayHand(true);
        Texture2dId textureId = MerchandiseUtil.GetMerchandiseTextureId(held.Color, held.Type, held.Tier);
        _handCustomerView.SetMerchandise(_texture2dRepository.GetTexture(textureId));
        _handShelfView.SetMerchandise(_texture2dRepository.GetTexture(textureId));
    }

    private void _RestartDay() {
        PackedScene mainLevelPackedScene = _sceneRepository.GetPackedScene(SceneId.MainLevel);
        GetTree().ChangeSceneToPacked(mainLevelPackedScene);
    }

    private bool _IsTransactionValid() {
        return _customerView.IsMerchandiseSellSlotHovered()
               && _customerView.IsCustomerReadyToPurchase()
               && _merchandiseService.GetHeldMerchandise() is not null;
    }

    private void _PlaySoundEffect(SoundEffectId soundEffectId) {
        if (GlobalSettings.MuteVolume) {
            return;
        }

        AudioStream soundEffect = _soundEffectRepository.GetSoundEffect(soundEffectId);
        _soundEffectPlayer.Stream = soundEffect;
        _soundEffectPlayer.Play();
    }

    private void _ChangeAudioVolume() {
        if (GlobalSettings.MuteVolume) {
            _musicPlayer.Stop();
            _soundEffectPlayer.Stop();
        } else {
            _musicPlayer.Play();
        }
    }

    private enum ActiveView {
        CustomerView,
        ShelfView
    }
}