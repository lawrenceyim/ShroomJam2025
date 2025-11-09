using Godot;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class MainLevel : Node, IInputState, ITick {
	[Export]
	private Camera2D _customerViewCamera;

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

	public override void _Ready() {
		_serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		_merchandiseService = _serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise);
		_inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
		_inputStateMachine.SetState(this);
		RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
		_packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene);
		_shelfView.Initialize(
			_serviceLocator.GetService<MerchandiseService>(ServiceName.Merchandise),
			repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture)
		);

		_tickTimer = new TickTimer();

	}

	public void PhysicsTick(double delta) { }

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
		}
		else if (_shelfViewCamera.IsCurrent()) {
			_customerViewCamera.MakeCurrent();
		}
	}

	private void _HandleMouseClick() {
		Vector2I? hoveredDvdSlot = _shelfView.GetHoveredSlot();
		if (hoveredDvdSlot != null) {
			_SwapDvds(hoveredDvdSlot.Value);
			return;
		}

		// get customer hovered
		// int? customerId =
	}

	private void _SwapDvds(Vector2I dvdSlot) {
		Merchandise heldMerchandise = _merchandiseService.GetHeldDvd();
		Merchandise merchandiseInSlot = _merchandiseService.GetDvdFromShelf(dvdSlot);

		_merchandiseService.SetHeldDvd(merchandiseInSlot);
		_merchandiseService.SetShelfDvd(heldMerchandise, dvdSlot);

		// _shelfView.SetDvdTexture(dvdSlot, );
	}
}
