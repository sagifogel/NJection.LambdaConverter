using System;

namespace NJection.LambdaConverter.Synchronization
{
    public interface IDowngradeableLocker : ILockIndicator, IDisposable
    {
        IUpgradeableLocker DowngradeToReadLock();
    }
}