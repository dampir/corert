namespace System.Runtime.InteropServices
{
    internal static class SafeHandleExtensions
    {
        public static void DangerousAddRef(this SafeHandle safeHandle)
        {
            // This check provides rough compatibility with the desktop code (this code's desktop counterpart is AcquireSafeHandleFromWaitHandle() inside clr.dll)
            // which throws ObjectDisposed if someone passes an uninitialized WaitHandle into one of the Wait apis. We use an extension method
            // because otherwise, the "null this" would trigger a NullReferenceException before we ever get to this check.
            if (safeHandle == null)
                throw new ObjectDisposedException(SR.ObjectDisposed_Generic);
            safeHandle.DangerousAddRef_WithNoNullCheck();
        }
    }
}