// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
**
** Purpose: The exception class for OOM.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class OutOfMemoryException : SystemException
    {
        public OutOfMemoryException()
            : base(SR.Arg_OutOfMemoryException)
        {
            HResult = __HResults.COR_E_OUTOFMEMORY;
        }

        public OutOfMemoryException(String message)
            : base(message)
        {
            HResult = __HResults.COR_E_OUTOFMEMORY;
        }

        public OutOfMemoryException(String message, Exception innerException)
            : base(message, innerException)
        {
            HResult = __HResults.COR_E_OUTOFMEMORY;
        }

        protected OutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
