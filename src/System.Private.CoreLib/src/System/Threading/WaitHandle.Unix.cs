// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Runtime.Augments;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
    public abstract partial class WaitHandle
    {
#if MONO
        private static bool Unix_WaitOneCore(IntPtr handle, int millisecondsTimeout) =>
            WaitSubsystem.Wait(handle, millisecondsTimeout);
#else
        private static bool WaitOneCore(IntPtr handle, int millisecondsTimeout) =>
            WaitSubsystem.Wait(handle, millisecondsTimeout);
#endif

#if MONO
        private static int Unix_WaitAnyCore(
            RuntimeThread currentThread,
            SafeWaitHandle[] safeWaitHandles,
            WaitHandle[] waitHandles,
            int millisecondsTimeout)
#else
        private static int WaitAnyCore(
            RuntimeThread currentThread,
            SafeWaitHandle[] safeWaitHandles,
            WaitHandle[] waitHandles,
            int millisecondsTimeout)
#endif
        {
            return WaitSubsystem.Wait(currentThread, safeWaitHandles, waitHandles, false, millisecondsTimeout);
        }

#if MONO
        private static bool Unix_WaitAllCore(
            RuntimeThread currentThread,
            SafeWaitHandle[] safeWaitHandles,
            WaitHandle[] waitHandles,
            int millisecondsTimeout)
#else
        private static bool WaitAllCore(
            RuntimeThread currentThread,
            SafeWaitHandle[] safeWaitHandles,
            WaitHandle[] waitHandles,
            int millisecondsTimeout)
#endif
	{
            return WaitSubsystem.Wait(currentThread, safeWaitHandles, waitHandles, true, millisecondsTimeout) != WaitTimeout;
        }

#if MONO
        private static bool Unix_SignalAndWaitCore(IntPtr handleToSignal, IntPtr handleToWaitOn, int millisecondsTimeout)
#else
        private static bool SignalAndWaitCore(IntPtr handleToSignal, IntPtr handleToWaitOn, int millisecondsTimeout)
#endif
        {
            return WaitSubsystem.SignalAndWait(handleToSignal, handleToWaitOn, millisecondsTimeout);
        }
    }
}
