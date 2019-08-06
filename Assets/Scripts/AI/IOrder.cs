public interface IOrder 
{
    void Init(AI_Unit aiUnit);
    void Init(AI_Unit aI_Unit, Unit target);
    void Init(AI_Unit aI_Unit, Tile target);
    void Execute();
    void Exit();
}
