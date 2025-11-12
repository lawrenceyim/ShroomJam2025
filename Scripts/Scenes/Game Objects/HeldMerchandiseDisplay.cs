using Godot;

public partial class HeldMerchandiseDisplay : Sprite2D {
	[Export]
	private Sprite2D _merchandise;

	public void SetMerchandise(Texture2D texture) {
		_merchandise.Texture = texture;
	}

	public void ToggleVisibility(bool visible) {
		Visible = visible;
	}
}
