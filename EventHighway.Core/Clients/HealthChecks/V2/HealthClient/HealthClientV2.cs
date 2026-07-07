// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the container/exposer for the V2 health check sub-clients. As a composition
    /// root it holds the sub-clients as read-only properties and has no retrieval method of its
    /// own, so it is exempt from the Florance dependency rule (like the root client).
    /// </summary>
    internal class HealthClientV2 : IHealthClientV2
    {
        public HealthClientV2(
            IHealthStatusClientV2 healthStatusClientV2,
            IHealthTrafficClientV2 healthTrafficClientV2,
            IHealthAddressClientV2 healthAddressClientV2,
            IHealthLoopClientV2 healthLoopClientV2,
            IHealthDuplicateClientV2 healthDuplicateClientV2,
            IHealthRetryClientV2 healthRetryClientV2,
            IHealthParticipantClientV2 healthParticipantClientV2)
        {
            this.HealthStatusClientV2 = healthStatusClientV2;
            this.HealthTrafficClientV2 = healthTrafficClientV2;
            this.HealthAddressClientV2 = healthAddressClientV2;
            this.HealthLoopClientV2 = healthLoopClientV2;
            this.HealthDuplicateClientV2 = healthDuplicateClientV2;
            this.HealthRetryClientV2 = healthRetryClientV2;
            this.HealthParticipantClientV2 = healthParticipantClientV2;
        }

        public IHealthStatusClientV2 HealthStatusClientV2 { get; }
        public IHealthTrafficClientV2 HealthTrafficClientV2 { get; }
        public IHealthAddressClientV2 HealthAddressClientV2 { get; }
        public IHealthLoopClientV2 HealthLoopClientV2 { get; }
        public IHealthDuplicateClientV2 HealthDuplicateClientV2 { get; }
        public IHealthRetryClientV2 HealthRetryClientV2 { get; }
        public IHealthParticipantClientV2 HealthParticipantClientV2 { get; }
    }
}
