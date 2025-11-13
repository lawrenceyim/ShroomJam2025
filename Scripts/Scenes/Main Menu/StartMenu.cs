using Godot;
using RepositorySystem;
using ServiceSystem;

public partial class StartMenu : Node2D {
    [Export]
    private Button _startButton;

    private PackedScene _mainLevelPackedScene;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        SceneRepository sceneRepository = repositoryLocator.GetRepository<SceneRepository>(RepositoryName.Scene);
        PlayerDataService playerDataService = serviceLocator.GetService<PlayerDataService>(ServiceName.PlayerData);
        playerDataService.SetDay(1);
        _mainLevelPackedScene = sceneRepository.GetPackedScene(SceneId.MainLevel);
        _startButton.Pressed += StartGame;
    }

    public void StartGame() {
        GD.Print("Starting Game");
        GetTree().ChangeSceneToPacked(_mainLevelPackedScene);
    }
}