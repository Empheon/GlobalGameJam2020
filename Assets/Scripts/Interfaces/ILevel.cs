
namespace Assets.Scripts.Interfaces
{

    public delegate void ReduceHandler();
    public delegate void NextLevelDoneHandler();

    public interface ILevel
    {

        event ReduceHandler OnReduce;
        event NextLevelDoneHandler OnNextLevelDone;
        Level NextLevel();

    }
}
