namespace Veeam.Signature.Solution
{
    public interface IOutputConsumer
    {
        void Process(Block block);
    }
}
