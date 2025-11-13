using Godot;
using Godot.Collections;
using RepositorySystem;

public partial class SoundEffectRepository : Node, IRepository {
	public static string AutoloadPath { get; } = "/root/SoundEffectRepository";

	[Export]
	private Dictionary<SoundEffectId, AudioStream> _soundEffects;

	public AudioStream GetSoundEffect(SoundEffectId id) {
		return _soundEffects[id];
	}
}

public enum SoundEffectId {
	EndOfDay = 0,
	MerchandisePickedUp = 1,
	MerchandisePutDown = 2,
	GreatSaleMade = 3,
	SaleMade = 4,
	SaleFailed = 5,
	StartOfDay = 6,
}
