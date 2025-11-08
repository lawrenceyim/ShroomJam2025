using Godot;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class MainLevel : Node, IInputState, ITick {
	// customer spawner
	// scene for customer front
	// scene for dvd storage 
	// order catalog screen
	// camera2d that switches between the two scenes

	private ServiceLocator _serviceLocator;
	private Shelf _shelf;
	private PackedSceneRepository _packedSceneRepository;

	public override void _Ready() {
		_serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		_shelf = new Shelf();
		AddChild(_shelf);
		RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
		_shelf.Initialize(
			_serviceLocator.GetService<DvdService>(ServiceName.Dvd),
			_packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene)
		);
	}

	public void PhysicsTick(double delta) { }

	public void ProcessInput(InputEventDto dto) { }
}
