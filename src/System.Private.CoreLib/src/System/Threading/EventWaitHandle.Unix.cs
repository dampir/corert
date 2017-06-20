// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if MONO
using System.Diagnostics.Private;
#endif
using System.Diagnostics;

namespace System.Threading
{
    public partial class EventWaitHandle
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
        private void Unix_CreateEventCore(bool initialState, EventResetMode mode, string name, out bool createdNew)
#else
        private void CreateEventCore(bool initialState, EventResetMode mode, string name, out bool createdNew)
#endif
        {
            Debug.Assert(name == null);

            SafeWaitHandle = WaitSubsystem.NewEvent(initialState, mode);
            createdNew = true;
        }

#if MONO
        private static OpenExistingResult Unix_OpenExistingWorker(string name, out EventWaitHandle result)
#else
        private static OpenExistingResult OpenExistingWorker(string name, out EventWaitHandle result)
#endif
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_NamedSynchronizationPrimitives);
        }

#if MONO
        private static bool Unix_ResetCore(IntPtr handle)
#else
        private static bool ResetCore(IntPtr handle)
#endif
        {
            WaitSubsystem.ResetEvent(handle);
            return true;
        }

#if MONO
        private static bool Unix_SetCore(IntPtr handle)
#else
        private static bool SetCore(IntPtr handle)
#endif
        {
            WaitSubsystem.SetEvent(handle);
            return true;
        }
    }
}
