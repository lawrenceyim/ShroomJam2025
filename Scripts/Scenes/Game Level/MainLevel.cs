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


    private const string SwitchView = "Space";

    private const int SecondsPerDay = 120;
    private readonly TickTimer _dayTimer = new TickTimer();
    private readonly int _ticksPerSeconds = Engine.PhysicsTicksPerSecond; // Avoid latency from marshalling
    private ServiceLocator _serviceLocator;
    private PackedSceneRepository _packedSceneRepository;
    private InputStateMachine _inputStateMachine;
    private TransactionService _transactionService;
    private MerchandiseService _merchandiseService;
    private Texture2dRepository _texture2dRepository;
    private GameClock _gameClock;
    private ActiveView _activeView = ActiveView.CustomerView;

    public override void _Ready() {
        _serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _merchandiseService = _serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise);
        _inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
        _gameClock = _serviceLocator.GetService<GameClock>(ServiceName.GameClock);
        _transactionService = _serviceLocator.GetService<TransactionService>(ServiceName.Transaction);

        PlayerDataSerivce playerDataService = _serviceLocator.GetService<PlayerDataSerivce>(ServiceName.PlayerData);
        _transactionService.Initialize(_merchandiseService, playerDataService);

        _merchandiseService.RestockMerchandise();
        _gameClock.AddActiveScene(this, GetInstanceId());
        _inputStateMachine.SetState(this);
        RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        _packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene);
        _texture2dRepository = repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture);

        _shelfView.Initialize(
            _merchandiseService,
            _texture2dRepository
        );
        _customerView.Initialize(
            _texture2dRepository,
            playerDataService
        );

        _DisplayHand(false);

        _dayTimer.StartFixedTimer(false, SecondsPerDay * _ticksPerSeconds);
        _dayTimer.TimedOut += _EndDay;
    }

    public override void _ExitTree() {
        _gameClock.RemoveActiveScene(GetInstanceId());
    }

    public void PhysicsTick(double delta) {
        _dayTimer.PhysicsTick(delta);
        _customerView.PhysicsTick(delta);
        _customerView.UpdateDayTimer(_dayTimer.GetTicksLeft());
    }

    public void ProcessInput(InputEventDto dto) {
        switch (dto) {
            case KeyDto keyDto:
                ProcessKeyInput(keyDto);
                break;
            case MouseButtonDto mouseButtonDto:
                _ProcessMouseButtonInput(mouseButtonDto);
                break;
        }
    }

    private void ProcessKeyInput(KeyDto dto) {
        switch (dto.Identifier) {
            case SwitchView:
                if (!dto.Pressed) {
                    break;
                }

                _SwitchView();
                break;
        }
    }

    private void _ProcessMouseButtonInput(MouseButtonDto dto) {
        if (!dto.Pressed) {
            return;
        }

        switch (_activeView) {
            case ActiveView.CustomerView:
                if (_IsTransactionValid()) {
                    _SellHeldMerchandise();
                }

                break;
            case ActiveView.ShelfView:
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
        }
        else if (_shelfViewCamera.IsCurrent()) {
            _customerViewCamera.MakeCurrent();
            _activeView = ActiveView.CustomerView;
        }
    }

    private void _SellHeldMerchandise() {
        CustomerSaleDto saleDto = _customerView.GetCustomerSale();
        int profit = _transactionService.SellMerchandise(saleDto);
        // update held merch UI
        _customerView.MerchandiseSold(profit);

        _DisplayHand(false);
        _merchandiseService.SetMerchandiseCount(_merchandiseService.GetMerchandiseCount() - 1);

        GD.Print($"Merchandise left: {_merchandiseService.GetMerchandiseCount()}");
        if (_merchandiseService.GetMerchandiseCount() == 0) {
            GD.Print("Sold out");
            _EndDay();
        }
    }

    private void _SetShelfMerchandiseTexture(Vector2I position, Texture2D texture) {
        _shelfView.SetMerchandiseTexture(position, texture);
    }

    private void _SwapMerchandise(Vector2I position) {
        Merchandise heldMerchandise = _merchandiseService.GetHeldMerchandise();
        Merchandise merchandiseInSlot = _merchandiseService.GetMerchandiseFromShelf(position);
        GD.Print($"Before swap. Held is {_merchandiseService.GetHeldMerchandise()?.ToString()}. " +
                 $"Slot is {_merchandiseService.GetMerchandiseFromShelf(position)?.ToString()}");

        _merchandiseService.SetHeldMerchandise(merchandiseInSlot);
        _merchandiseService.SetShelfMerchandise(heldMerchandise, position);

        GD.Print($"After swap. Held is {_merchandiseService.GetHeldMerchandise()?.ToString()}. " +
                 $"Slot is {_merchandiseService.GetMerchandiseFromShelf(position)?.ToString()}");

        _shelfView.RefreshShelfMerchandiseTexture(position);
        _RefreshHandDisplay();
    }

    private void _EndDay() {
        // TODO: Implement transition to EoD screen
        GD.Print("End of Day");
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

    private bool _IsTransactionValid() {
        return _customerView.IsMerchandiseSellSlotHovered()
               && _customerView.IsCustomerReadyToPurchase()
               && _merchandiseService.GetHeldMerchandise() is not null;
    }

    private enum ActiveView {
        CustomerView,
        ShelfView
    }
}