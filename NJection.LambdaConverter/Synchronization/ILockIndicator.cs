namespace NJection.LambdaConverter.Synchronization
{
    public interface ILockIndicator
    {
        bool LockAcquired { get; }
    }
}