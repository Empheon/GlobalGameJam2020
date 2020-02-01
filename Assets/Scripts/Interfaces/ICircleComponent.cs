
namespace Assets.Scripts.Interfaces
{

    public delegate void CircleComponentReduceFinishedeHandler(CircleComponent circleComponent);

    public interface ICircleComponent
    {

        event CircleComponentReduceFinishedeHandler OnReduceFinished;

    }
}
