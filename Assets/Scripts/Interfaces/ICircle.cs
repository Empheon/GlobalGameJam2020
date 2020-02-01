
namespace Assets.Scripts.Interfaces
{

    public delegate void CircleReduceFinishedeHandler(Circle circle);

    public interface ICircle
    {

        event CircleReduceFinishedeHandler OnReduceFinished;

        void Reduce();

    }
}
