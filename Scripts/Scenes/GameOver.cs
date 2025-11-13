using Godot;
using RepositorySystem;
using ServiceSystem;

public partial class GameOver : Node2D {
    [Export]
    private Sprite2D _endingSprite;

    [Export]
    private AudioStreamPlayer _soundEffectPlayer;

    private const int NeutralEndingThreshold = 5000;
    private const int GoodEndingThreshold = 7500;
    private SoundEffectRepository _soundEffectRepository; // Refactor into component
    private Texture2dRepository _textureRepository;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>(ServiceName.RepositoryLocator);
        _textureRepository = repositoryLocator.GetRepository<Texture2dRepository>(RepositoryName.Texture);
        _soundEffectRepository = repositoryLocator.GetRepository<SoundEffectRepository>(RepositoryName.SoundEffect);
        PlayerDataService playerDataService = serviceLocator.GetService<PlayerDataService>(ServiceName.PlayerData);
        _SetEnding(playerDataService.GetCareerProfit());
    }

    private void _SetEnding(int careerProfit) {
        _endingSprite.Texture = careerProfit switch {
            < NeutralEndingThreshold => _textureRepository.GetTexture(Texture2dId.BadEnding),
            < GoodEndingThreshold => _textureRepository.GetTexture(Texture2dId.NeutralEnding),
            _ => _textureRepository.GetTexture(Texture2dId.GoodEnding)
        };

        _PlaySoundEffect(careerProfit switch {
            < NeutralEndingThreshold => SoundEffectId.SaleFailed,
            < GoodEndingThreshold => SoundEffectId.SaleMade,
            _ => SoundEffectId.GreatSaleMade
        });
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