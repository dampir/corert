// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Runtime.Augments;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Threading
{
    //
    // Windows-specific implementation of ThreadPool
    //
    public static partial class ThreadPool
    {
        /// <summary>
        /// The maximum number of threads in the default thread pool on Windows 10 as computed by
        /// TppComputeDefaultMaxThreads(TppMaxGlobalPool).
        /// </summary>
        /// <remarks>
        /// Note that Windows 8 and 8.1 used a different value: Math.Max(4 * ThreadPoolGlobals.processorCount, 512).
        /// </remarks>
#if MONO
        private static readonly int Windows_MaxThreadCount = Math.Max(8 * ThreadPoolGlobals.processorCount, 768);
#else
        private static readonly int MaxThreadCount = Math.Max(8 * ThreadPoolGlobals.processorCount, 768);
#endif

        private static IntPtr s_work;

#if MONO
        public static bool Windows_SetMaxThreads(int workerThreads, int completionPortThreads)
#else
        public static bool SetMaxThreads(int workerThreads, int completionPortThreads)
#endif
        {
            // Not supported at present
            return false;
        }

#if MONO
        public static void Windows_GetMaxThreads(out int workerThreads, out int completionPortThreads)
#else
        public static void GetMaxThreads(out int workerThreads, out int completionPortThreads)
#endif
        {
            // Note that worker threads and completion port threads share the same thread pool.
            // The total number of threads cannot exceed MaxThreadCount.
            workerThreads = MaxThreadCount;
            completionPortThreads = MaxThreadCount;
        }

#if MONO
        public static bool Windows_SetMinThreads(int workerThreads, int completionPortThreads)
#else
        public static bool SetMinThreads(int workerThreads, int completionPortThreads)
#endif
        {
            // Not supported at present
            return false;
        }

#if MONO
        public static void Windows_GetMinThreads(out int workerThreads, out int completionPortThreads)
#else
        public static void GetMinThreads(out int workerThreads, out int completionPortThreads)
#endif
        {
            workerThreads = 0;
            completionPortThreads = 0;
        }

#if MONO
        public static void Windows_GetAvailableThreads(out int workerThreads, out int completionPortThreads)
#else
        public static void GetAvailableThreads(out int workerThreads, out int completionPortThreads)
#endif
        {
            // Make sure we return a non-negative value if thread pool defaults are changed
            int availableThreads = Math.Max(MaxThreadCount - ThreadPoolGlobals.workQueue.numWorkingThreads, 0);

            workerThreads = availableThreads;
            completionPortThreads = availableThreads;
        }

        [NativeCallable(CallingConvention = CallingConvention.StdCall)]
        private static void DispatchCallback(IntPtr instance, IntPtr context, IntPtr work)
        {
            RuntimeThread.InitializeThreadPoolThread();
            Debug.Assert(s_work == work);
            ThreadPoolWorkQueue.Dispatch();
        }

#if MONO
        internal static void Windows_QueueDispatch()
#else
        internal static void QueueDispatch()
#endif
        {
            if (s_work == IntPtr.Zero)
            {
                IntPtr nativeCallback = AddrofIntrinsics.AddrOf<Interop.mincore.WorkCallback>(DispatchCallback);

                IntPtr work = Interop.mincore.CreateThreadpoolWork(nativeCallback, IntPtr.Zero, IntPtr.Zero);
                if (work == IntPtr.Zero)
                    throw new OutOfMemoryException();

                if (Interlocked.CompareExchange(ref s_work, work, IntPtr.Zero) != IntPtr.Zero)
                    Interop.mincore.CloseThreadpoolWork(work);
            }

            Interop.mincore.SubmitThreadpoolWork(s_work);
        }
    }
}
