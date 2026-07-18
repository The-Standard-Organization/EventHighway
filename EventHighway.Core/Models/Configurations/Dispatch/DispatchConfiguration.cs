// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;

namespace EventHighway.Core.Models.Configurations.Dispatch
{
    /// <summary>
    /// Configures how individual event handlers are invoked during dispatch, retry, and replay.
    /// </summary>
    public class DispatchConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum time a single handler invocation may run before it is
        /// abandoned and recorded as a failed (timed-out) delivery that the retry pipeline can
        /// re-attempt. The handler's cancellation token is signalled on timeout so cooperative
        /// handlers can stop early. Defaults to <see cref="Timeout.InfiniteTimeSpan"/>, meaning no
        /// timeout is enforced unless the host opts in.
        /// </summary>
        public TimeSpan HandlerTimeout { get; set; } = Timeout.InfiniteTimeSpan;
    }
}
