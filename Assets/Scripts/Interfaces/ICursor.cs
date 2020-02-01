
namespace Assets.Scripts.Interfaces
{

    public delegate void NewTurnHandler();

    public interface ICursor
    {

        event NewTurnHandler OnNewTurn;

    }
}
