// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
#if MONO
using System.Diagnostics.Private;
#endif
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace Internal.Runtime.Augments
{
    public sealed partial class RuntimeThread
    {
        // Event signaling that the thread has stopped
        private ManualResetEvent _stopped;

        private readonly WaitSubsystem.ThreadWaitInfo _waitInfo;

        internal WaitSubsystem.ThreadWaitInfo WaitInfo => _waitInfo;

#if MONO
        private void Unix_PlatformSpecificInitialize()
#else
        private void PlatformSpecificInitialize()
#endif
        {
            RuntimeImports.RhSetThreadExitCallback(AddrofIntrinsics.AddrOf<Action>(OnThreadExit));
        }

        // Platform-specific initialization of foreign threads, i.e. threads not created by Thread.Start
#if MONO
        private void Unix_PlatformSpecificInitializeExistingThread()
#else
        private void PlatformSpecificInitializeExistingThread()
#endif
        {
            _stopped = new ManualResetEvent(false);
        }

#if MONO
        private SafeWaitHandle[] Unix_RentWaitedSafeWaitHandleArray(int requiredCapacity)
#else
        /// <summary>
        /// Callers must ensure to clear and return the array after use
        /// </summary>
        internal SafeWaitHandle[] RentWaitedSafeWaitHandleArray(int requiredCapacity)
#endif
        {
            Debug.Assert(this == CurrentThread);
            Debug.Assert(!ReentrantWaitsEnabled); // due to this, no need to actually rent and return the array

            _waitedSafeWaitHandles.VerifyElementsAreDefault();
            _waitedSafeWaitHandles.EnsureCapacity(requiredCapacity);
            return _waitedSafeWaitHandles.Items;
        }

#if MONO
        private void Unix_ReturnWaitedSafeWaitHandleArray(SafeWaitHandle[] waitedSafeWaitHandles)
#else
        internal void ReturnWaitedSafeWaitHandleArray(SafeWaitHandle[] waitedSafeWaitHandles)
#endif
        {
            Debug.Assert(this == CurrentThread);
            Debug.Assert(!ReentrantWaitsEnabled); // due to this, no need to actually rent and return the array
            Debug.Assert(waitedSafeWaitHandles == _waitedSafeWaitHandles.Items);
        }

#if MONO
        private ThreadPriority Unix_GetPriorityLive()
#else
        private ThreadPriority GetPriorityLive()
#endif
        {
            return ThreadPriority.Normal;
        }

#if MONO
        private bool Unix_SetPriorityLive(ThreadPriority priority)
#else
        private bool SetPriorityLive(ThreadPriority priority)
#endif
        {
            return true;
        }

        [NativeCallable]
        private static void OnThreadExit()
        {
            // Set the Stopped bit and signal the current thread as stopped
            RuntimeThread currentThread = t_currentThread;
            if (currentThread != null)
            {
                int state = currentThread._threadState;
                if ((state & (int)(ThreadState.Stopped | ThreadState.Aborted)) == 0)
                {
                    currentThread.SetThreadStateBit(ThreadState.Stopped);
                }
                currentThread._stopped.Set();
            }
        }

#if MONO
        private ThreadState Unix_GetThreadState() => (ThreadState)_threadState;
#else
        private ThreadState GetThreadState() => (ThreadState)_threadState;
#endif

#if MONO
        private bool Unix_JoinInternal(int millisecondsTimeout)
#else
        private bool JoinInternal(int millisecondsTimeout)
#endif
        {
            // This method assumes the thread has been started
            Debug.Assert(!GetThreadStateBit(ThreadState.Unstarted) || (millisecondsTimeout == 0));
            SafeWaitHandle waitHandle = _stopped.SafeWaitHandle;

            // If an OS thread is terminated and its Thread object is resurrected, waitHandle may be finalized and closed
            if (waitHandle.IsClosed)
            {
                return true;
            }

            // Prevent race condition with the finalizer
            try
            {
                waitHandle.DangerousAddRef();
            }
            catch (ObjectDisposedException)
            {
                return true;
            }

            try
            {
                return _stopped.WaitOne(millisecondsTimeout);
            }
            finally
            {
                waitHandle.DangerousRelease();
            }
        }

#if MONO
        private bool Unix_CreateThread(GCHandle thisThreadHandle)
#else
        private bool CreateThread(GCHandle thisThreadHandle)
#endif
        {
            // Create the Stop event before starting the thread to make sure
            // it is ready to be signaled at thread shutdown time.
            // This also avoids OOM after creating the thread.
            _stopped = new ManualResetEvent(false);

            if (!Interop.Sys.RuntimeThread_CreateThread((IntPtr)_maxStackSize,

#if MONO
                AddrofIntrinsics.AddrOf<Interop.Sys.ThreadProc>(Unix_ThreadEntryPoint), 
#else
                AddrofIntrinsics.AddrOf<Interop.Sys.ThreadProc>(ThreadEntryPoint),
#endif

                (IntPtr)thisThreadHandle))
            {
                return false;
            }

            // CoreCLR ignores OS errors while setting the priority, so do we
            SetPriorityLive(_priority);

            return true;
        }

#if MONO
        /// <summary>
        /// This an entry point for managed threads created by applicatoin
        /// </summary>
        [NativeCallable]
        private static IntPtr Unix_ThreadEntryPoint(IntPtr parameter)
#else
        /// <summary>
        /// This an entry point for managed threads created by applicatoin
        /// </summary>
        [NativeCallable]
        private static IntPtr ThreadEntryPoint(IntPtr parameter)
#endif
        {
            StartThread(parameter);
            return IntPtr.Zero;
        }

#if MONO
        internal void Unix_Interrupt() => WaitSubsystem.Interrupt(this);
#else
        public void Interrupt() => WaitSubsystem.Interrupt(this);
#endif

#if MONO
        internal static void Unix_UninterruptibleSleep0() => WaitSubsystem.UninterruptibleSleep0();
#else
        internal static void UninterruptibleSleep0() => WaitSubsystem.UninterruptibleSleep0();
#endif

#if MONO
        private static void Unix_SleepInternal(int millisecondsTimeout) => WaitSubsystem.Sleep(millisecondsTimeout);
#else
        private static void SleepInternal(int millisecondsTimeout) => WaitSubsystem.Sleep(millisecondsTimeout);
#endif

#if MONO
        internal const bool Unix_ReentrantWaitsEnabled = false;
#else
        internal const bool ReentrantWaitsEnabled = false;
#endif

#if MONO
        internal static void Unix_SuppressReentrantWaits()
#else
        internal static void SuppressReentrantWaits()
#endif
        {
            throw new PlatformNotSupportedException();
        }

#if MONO
        internal static void Unix_RestoreReentrantWaits()
#else
        internal static void RestoreReentrantWaits()
#endif
        {
            throw new PlatformNotSupportedException();
        }
    }
}
