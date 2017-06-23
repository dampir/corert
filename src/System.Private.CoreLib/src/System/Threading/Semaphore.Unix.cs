// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if MONO
using System.Diagnostics.Private;
#else
using System.Diagnostics;
#endif
using System.IO;

namespace System.Threading
{
    public sealed partial class Semaphore
    {
#if MONO
        private static void Unix_VerifyNameForCreate(string name)
#else   
        private static void VerifyNameForCreate(string name)
#endif
        {
            if (name != null)
            {
                throw new PlatformNotSupportedException(SR.PlatformNotSupported_NamedSynchronizationPrimitives);
            }
        }

#if MONO
        private void Unix_CreateSemaphoreCore(int initialCount, int maximumCount, string name, out bool createdNew)
#else
        private void CreateSemaphoreCore(int initialCount, int maximumCount, string name, out bool createdNew)
#endif
        {
            Debug.Assert(name == null);

            SafeWaitHandle = WaitSubsystem.NewSemaphore(initialCount, maximumCount);
            createdNew = true;
        }

#if MONO
        private static OpenExistingResult Unix_OpenExistingWorker(string name, out Semaphore result)
#else
        private static OpenExistingResult OpenExistingWorker(string name, out Semaphore result)
#endif
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_NamedSynchronizationPrimitives);
        }

#if MONO
        private static int Unix_ReleaseCore(IntPtr handle, int releaseCount)
#else
        private static int ReleaseCore(IntPtr handle, int releaseCount)
#endif
        {
            return WaitSubsystem.ReleaseSemaphore(handle, releaseCount);
        }
    }
}
