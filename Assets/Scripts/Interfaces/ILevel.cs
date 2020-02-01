
namespace Assets.Scripts.Interfaces
{

    public delegate void ReduceHandler();

    public interface ILevel
    {

        event ReduceHandler OnReduce;

        Level NextLevel();

    }
}
