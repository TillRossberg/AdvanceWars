public abstract class Order 
{
    public abstract AI_Unit aiUnit { get; set; }
    public abstract Unit TargetUnit { get; set; }
    public abstract Tile TargetTile { get; set; }
    public abstract bool OrderFinished { get; set; }
    public abstract void Start();   
    public abstract void Continue();
    public abstract void Exit();
    public abstract void Terminate();
    public Order(AI_Unit aiUnit, Tile tile)
    {
        this.aiUnit = aiUnit;
        this.TargetTile = tile;
    }
    public Order(AI_Unit aiUnit, Unit unit)
    {
        this.aiUnit = aiUnit;
        this.TargetUnit = unit;
    }
    public Order(AI_Unit aiUnit)
    {
        this.aiUnit = aiUnit;
    }
    public Order()
    {

    }


}
