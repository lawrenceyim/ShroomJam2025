using Godot;
using Godot.Collections;
using RepositorySystem;

public partial class Texture2dRepository : Node, IAutoload, IRepository {
    public static string AutoloadPath { get; } = "/root/Texture2dRepository";

    [Export]
    private Dictionary<Texture2dId, Texture2D> _textures;

    public Texture2D GetTexture(Texture2dId id) {
        return _textures[id];
    }
}

public enum Texture2dId {
    CustomerPlaceholder = 0,
    MerchandisePlaceholder = 1,
    RichManHappy = 2,
    RichManNeutral = 3,
    RichManAngry = 4,
    RichWomanHappy = 5,
    RichWomanNeutral = 6,
    RichWomanAngry = 7,
    AverageManHappy = 8,
    AverageManNeutral = 9,
    AverageManAngry = 10,
    AverageWomanHappy = 11,
    AverageWomanNeutral = 12,
    AverageWomanAngry = 13,
    PoorManHappy = 14,
    PoorManNeutral = 15,
    PoorManAngry = 16,
    PoorWomanHappy = 17,
    PoorWomanNeutral = 18,
    PoorWomanAngry = 19,
}