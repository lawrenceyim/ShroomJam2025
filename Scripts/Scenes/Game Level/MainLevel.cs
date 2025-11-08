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
    [Export]
    private Camera2D _customerViewCamera;

    [Export]
    private Camera2D _shelfViewCamera;

    private const string SwitchView = "Space";

    private ServiceLocator _serviceLocator;
    private Shelf _shelf;
    private PackedSceneRepository _packedSceneRepository;
    private InputStateMachine _inputStateMachine;

    public override void _Ready() {
        _serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _inputStateMachine = _serviceLocator.GetService<InputStateMachine>(ServiceName.InputStateMachine);
        _inputStateMachine.SetState(this);

        _shelf = new Shelf();
        AddChild(_shelf);
        RepositoryLocator repositoryLocator = _serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        _shelf.Initialize(
            _serviceLocator.GetService<DvdService>(ServiceName.Dvd),
            _packedSceneRepository = repositoryLocator.GetRepository<PackedSceneRepository>(RepositoryName.PackedScene)
        );
    }

    public void PhysicsTick(double delta) { }

    public void ProcessInput(InputEventDto dto) {
        switch (dto) {
            case KeyDto keyDto:
                switch (keyDto.Identifier) {
                    case SwitchView:
                        _SwitchView();
                        break;
                }

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
}