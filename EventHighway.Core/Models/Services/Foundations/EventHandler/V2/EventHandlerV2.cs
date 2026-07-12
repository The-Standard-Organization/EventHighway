// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Services.Foundations.EventHandler.V2
{
    /// <summary>
    /// Represents a registered event handler in the V2 service model, persisting the identity
    /// of handlers registered within the EventHighway pipeline.
    /// </summary>
    public class EventHandlerV2
    {
        /// <summary>
        /// Gets or sets the unique id that identifies this handler within the EventHighway pipeline.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique name that identifies this handler within the EventHighway pipeline.
        /// </summary>
        public string Name { get; set; }
    }
}
