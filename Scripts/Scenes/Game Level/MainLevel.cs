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

    private const string SwitchView = "Space";

    private ServiceLocator _serviceLocator;
    private PackedSceneRepository _packedSceneRepository;
    private InputStateMachine _inputStateMachine;
    private TickTimer _tickTimer;
    private MerchandiseService _merchandiseService;
    private GameClock _gameClock;
    private ActiveView _activeView = ActiveView.CustomerView;

    public override void _Ready() {
        _serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _merchandiseService = _serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise);
        _inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
        _gameClock = _serviceLocator.GetService<GameClock>(ServiceName.GameClock);

        _gameClock.AddActiveScene(this, GetInstanceId());
        _inputStateMachine.SetState(this);
        RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        _packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene);
        _shelfView.Initialize(
            _serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise),
            repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture)
        );
        _customerView.Initialize(
            repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture)
        );

        _tickTimer = new TickTimer();
    }

    public override void _ExitTree() {
        _gameClock.RemoveActiveScene(GetInstanceId());
    }

    public void PhysicsTick(double delta) {
        _customerView.PhysicsTick(delta);
    }

    public void ProcessInput(InputEventDto dto) {
        switch (dto) {
            case KeyDto keyDto:
                switch (keyDto.Identifier) {
                    case SwitchView:
                        if (!keyDto.Pressed) {
                            break;
                        }

                        _SwitchView();
                        break;
                }

                break;
            case MouseButtonDto mouseButtonDto:
                _HandleMouseClick();
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

    private void _HandleMouseClick() {
        switch (_activeView) {
            case ActiveView.CustomerView:
                if (_customerView.IsMerchandiseSellSlotHovered() 
                    && _customerView.IsCustomerReadytoPurchase()
                    && _merchandiseService.GetHeldMerchandise() is not null) { 
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

    private void _SellHeldMerchandise() {
        CustomerSaleDto saleDto = _customerView.GetCustomerSale();
        
    }

    private void _SwapMerchandise(Vector2I dvdSlot) {
        Merchandise heldMerchandise = _merchandiseService.GetHeldMerchandise();
        Merchandise merchandiseInSlot = _merchandiseService.GetMerchandiseFromShelf(dvdSlot);

        _merchandiseService.SetHeldMerchandise(merchandiseInSlot);
        _merchandiseService.SetShelfMerchandise(heldMerchandise, dvdSlot);

        // _shelfView.SetDvdTexture(dvdSlot, );
    }

    private enum ActiveView {
        CustomerView,
        ShelfView
    }
}