
namespace Assets.Scripts.Interfaces
{

    public delegate void ReduceHandler();
    public delegate void NextLevelReadyHandler();

    public interface ILevel
    {

        event ReduceHandler OnReduce;
        event NextLevelReadyHandler OnNextLevelReady;
        void InstantiateNext(bool init = false);
        void Init();

    }
}
