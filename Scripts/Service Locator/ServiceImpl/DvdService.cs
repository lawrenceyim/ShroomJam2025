using Godot;
using ServiceSystem;

public class DvdService : IService {
    private DvdRepository _dvdRepository;

    public DvdService(DvdRepository dvdRepository) {
        _dvdRepository = dvdRepository;
    }

    public Vector2I GetShelfSize() {
        return _dvdRepository.GetShelfSize();
    }

    public Dvd GetDvd(Vector2I position) {
        return _dvdRepository.GetDvd(position);
    }
}