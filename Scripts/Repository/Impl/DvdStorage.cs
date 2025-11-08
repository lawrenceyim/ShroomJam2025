using System.Collections.Generic;
using Godot;

namespace RepositorySystem;

public class DvdRepository : IRepository {
    private int _numberOfRows = 3;
    private int _numberOfColumns = 3;    
    
    private Dvd _heldDvd;
    private Dictionary<Vector2I, Dvd> _shelf = new Dictionary<Vector2I, Dvd>();

    public void RemoveDvd(Vector2I position) {
        _shelf.Remove(position);
    }

    public void AddDvd(Dvd dvd, Vector2I position) {
        _shelf.Add(position, dvd);
    }

    public Dvd GetDvd(Vector2I position) {
        return _shelf[position];
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
}