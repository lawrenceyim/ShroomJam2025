using System.Collections.Generic;
using Godot;

public partial class ShelfView : Node2D {
	[Export]
	private PackedScene _dvdSlotPackedScene;

	private readonly Vector2I _originPosition = new Vector2I(-500, -200);

	private readonly int _xOffset = 170;
	private readonly int _yOffset = 200;
	private MerchandiseService _merchandiseService;
	private PackedSceneRepository _packedSceneRepository;
	private Vector2I? _hoveredSlot;
	private Dictionary<Vector2I, MerchandiseSlot> _dvdSlots = new();
	private Texture2dRepository _texture2dRepository;

	public void Initialize(MerchandiseService merchandiseService, Texture2dRepository texture2dRepository) {
		_merchandiseService = merchandiseService;
		_texture2dRepository = texture2dRepository;
		Vector2I shelfSize = merchandiseService.GetShelfSize();
		GD.Print(shelfSize);

		for (int column = 0; column < shelfSize.X; column++) {
			for (int row = 0; row < shelfSize.Y; row++) {
				MerchandiseSlot merchandiseSlot = _dvdSlotPackedScene.Instantiate() as MerchandiseSlot;
				Vector2I position = new Vector2I(column, row);
				merchandiseSlot.Initialize(position);
				merchandiseSlot.Position = _originPosition + new Vector2I(column * _xOffset, row * _yOffset);
				merchandiseSlot.Initialize(position);
				merchandiseSlot.MouseEntered += (Vector2I position) => {
					GD.Print(position);
					_hoveredSlot = position;
				};
				merchandiseSlot.MouseExited += () => _hoveredSlot = null;
				_dvdSlots[position] = merchandiseSlot;
				AddChild(merchandiseSlot);

				Merchandise merchandise = _merchandiseService.GetDvdFromShelf(new Vector2I(column, row));
				if (merchandise != null) {
					// set sprite based on tier and color of the dvd                    
					SetDvdTexture(position, _texture2dRepository.GetTexture(Texture2dId.Placeholder));
				}
			}
		}
	}

	public void SetDvdTexture(Vector2I slot, Texture2D texture) {
		_dvdSlots[slot].SetTexture(texture);
	}

	public Vector2I? GetHoveredSlot() {
		return _hoveredSlot;
	}
}
