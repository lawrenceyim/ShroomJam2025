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
    private TransactionService _transactionService;
    private MerchandiseService _merchandiseService;
    private GameClock _gameClock;
    private ActiveView _activeView = ActiveView.CustomerView;

    public override void _Ready() {
        _serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _merchandiseService = _serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise);
        _inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
        _gameClock = _serviceLocator.GetService<GameClock>(ServiceName.GameClock);
        _transactionService = _serviceLocator.GetService<TransactionService>(ServiceName.Transaction);

        _merchandiseService.RestockMerchandise();
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
                ProcessKeyInput(keyDto);
                break;
            case MouseButtonDto mouseButtonDto:
                _ProcessMouseButtonInput();
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

    private void _ProcessMouseButtonInput() {
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
    }

    private void _SwapMerchandise(Vector2I dvdSlot) {
        Merchandise heldMerchandise = _merchandiseService.GetHeldMerchandise();
        Merchandise merchandiseInSlot = _merchandiseService.GetMerchandiseFromShelf(dvdSlot);

        _merchandiseService.SetHeldMerchandise(merchandiseInSlot);
        _merchandiseService.SetShelfMerchandise(heldMerchandise, dvdSlot);

        // _shelfView.SetDvdTexture(dvdSlot, );
    }

    private bool _IsTransactionValid() {
        return _customerView.IsMerchandiseSellSlotHovered()
               && _customerView.IsCustomerReadytoPurchase()
               && _merchandiseService.GetHeldMerchandise() is not null;
    }

    private enum ActiveView {
        CustomerView,
        ShelfView
    }
}