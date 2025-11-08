using Godot;
using ServiceSystem;

public partial class Shelf : Node2D {
    private readonly Vector2I _originPosition = new Vector2I(330, 270);
    private readonly int _xOffset = 170;
    private readonly int _yOffset = 200;
    private DvdService _dvdService;
    private PackedSceneRepository _packedSceneRepository;
    private PackedScene _dvdPackedScene;

    public void Initialize(DvdService dvdService, PackedSceneRepository packedSceneRepository) {
        _dvdService = dvdService;
        _packedSceneRepository = packedSceneRepository;
        PackedScene dvdPackedScene = _packedSceneRepository.GetPackedScene(PackedSceneId.Dvd);
        Vector2I shelfSize = dvdService.GetShelfSize();
        GD.Print(shelfSize);

        for (int column = 0; column < shelfSize.X; column++) {
            for (int row = 0; row < shelfSize.Y; row++) {
                // Dvd dvd = dvdService.GetDvd(new Vector2I(column, row));
                // if (dvd == null) {
                //     continue;
                // }

                DvdObject dvdObject = dvdPackedScene.Instantiate() as DvdObject;
                dvdObject.Position = _originPosition + new Vector2I(column * _xOffset, row * _yOffset);
                AddChild(dvdObject);
            }
        }
    }
}