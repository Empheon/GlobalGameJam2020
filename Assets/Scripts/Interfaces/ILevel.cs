
namespace Assets.Scripts.Interfaces
{

    public delegate void ReduceHandler();
    public delegate void NextLevelReadyHandler(Level nextLevel);

    public interface ILevel
    {

        event ReduceHandler OnReduce;
        event NextLevelReadyHandler OnNextLevelReady;
        Level NextLevel();
        void Init();

    }
}
