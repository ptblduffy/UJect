namespace UJect
{
    public interface IInstaller
    {
        void Install(DiContainer diContainer);
    }

    public class Installer : IInstaller
    {
        public void Install(DiContainer diContainer)
        {
            throw new System.NotImplementedException();
        }
    }
}