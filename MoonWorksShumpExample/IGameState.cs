namespace MoonWorksShumpExample;
public interface IGameState
{
    public void Start();
    public void Update(TimeSpan delta);
    public void Draw(double alpha);
    public void End();
}
