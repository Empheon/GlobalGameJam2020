
namespace Assets.Scripts.Interfaces
{

    public delegate void DestroyHandler(Circle circle);

    public interface ICircle
    {

        event DestroyHandler OnDestroyFinished;

        void Reduce();

        void Destroy();

    }
}
