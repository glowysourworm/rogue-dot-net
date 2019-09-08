
namespace Rogue.NET.Scenario.Controller.Interface
{
    public interface IFrontEndController
    {
        void PostInputMessage<T>(T message);

        T ReadOutputMessage<T>();
    }
}
