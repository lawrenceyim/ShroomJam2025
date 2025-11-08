using System.Collections.Generic;
using Godot;
using RepositorySystem;

public class DvdRepository : IRepository {
    private int _numberOfRows = 2;
    private int _numberOfColumns = 2;

    private Dvd _heldDvd;
    private Dictionary<Vector2I, Dvd> _shelf = new Dictionary<Vector2I, Dvd>();

    public void RemoveDvd(Vector2I position) {
        _shelf.Remove(position);
    }

    public void AddDvd(Dvd dvd, Vector2I position) {
        _shelf[position] = dvd;
    }

    public Dvd GetDvd(Vector2I position) {
        return _shelf.GetValueOrDefault(position, null);
    }

    public void SetHeldDvd(Dvd heldDvd) {
        _heldDvd = heldDvd;
    }

    public Dvd GetHeldDvd() {
        return _heldDvd;
    }

    public void RemoveHeldDvd() {
        _heldDvd = null;
    }

    public Vector2I GetShelfSize() {
        return new Vector2I(_numberOfColumns, _numberOfRows);
    }

    public void SetShelfSize(Vector2I shelfSize) {
        _numberOfColumns = shelfSize.X;
        _numberOfRows = shelfSize.Y;
    }
}