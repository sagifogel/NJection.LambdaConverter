namespace NJection.LambdaConverter.Synchronization
{
    public interface ILocker : ILockIndicator
    {
        ILockerFinalizer AcquireReadLock();
        IDowngradeableLocker AcquireWriterLock();
        IUpgradeableLocker AcquireUpgradeableReadLock();
    }
}