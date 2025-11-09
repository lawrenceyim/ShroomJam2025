using Godot;

public partial class CustomerSpawner : Node, ITick {
    [Export]
    private int _customerSpawndelayMax;

    [Export]
    private int _customerSpawndelayMin;

    private int _customerSpawndelay;

    public void PhysicsTick(double delta) {
        throw new System.NotImplementedException();
    }
}