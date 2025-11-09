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
	private CustomerSpawner _customerSpawner;
	private DvdService _dvdService;

	public override void _Ready() {
		_serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		_dvdService = _serviceLocator.GetService<DvdService>(ServiceName.Dvd);
		_inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
		_inputStateMachine.SetState(this);
		RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
		_packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene);
		_shelfView.Initialize(
			_serviceLocator.GetService<DvdService>(ServiceName.Dvd),
			repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture)
		);

		_customerSpawner = new CustomerSpawner();
		AddChild(_customerSpawner);
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
		Dvd heldDvd = _dvdService.GetHeldDvd();
		Dvd dvdInSlot = _dvdService.GetDvdFromShelf(dvdSlot);

		_dvdService.SetHeldDvd(dvdInSlot);
		_dvdService.SetShelfDvd(heldDvd, dvdSlot);

		// _shelfView.SetDvdTexture(dvdSlot, );
	}
}
