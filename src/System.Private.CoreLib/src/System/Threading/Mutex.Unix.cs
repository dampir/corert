// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if MONO
using System.Diagnostics.Private;
#endif
using System.Diagnostics;
using System.IO;

namespace System.Threading
{
    public sealed partial class Mutex
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
        private void Unix_CreateMutexCore(bool initiallyOwned, string name, out bool createdNew)
#else
        private void CreateMutexCore(bool initiallyOwned, string name, out bool createdNew)
#endif
        {
            Debug.Assert(name == null);

            SafeWaitHandle = WaitSubsystem.NewMutex(initiallyOwned);
            createdNew = true;
        }

#if MONO
        private static OpenExistingResult Unix_OpenExistingWorker(string name, out Mutex result)
#else
        private static OpenExistingResult OpenExistingWorker(string name, out Mutex result)
#endif
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_NamedSynchronizationPrimitives);
        }

#if MONO
        private static void Unix_ReleaseMutexCore(IntPtr handle)
#else
        private static void ReleaseMutexCore(IntPtr handle)
#endif
        {
            WaitSubsystem.ReleaseMutex(handle);
        }
    }
}
