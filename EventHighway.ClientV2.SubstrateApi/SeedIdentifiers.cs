// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.Seed
{
    // Fixed identifiers for this app's sample data. Each sample app carries its OWN copy of these
    // rather than sharing one file across projects, so every app builds and runs in isolation with
    // no cross-project entanglement — deleting one app can never break another. The same well-known
    // Guids are mirrored privately in EventHighway.Portal.Seed's DatabaseHydrator; keep the copies
    // in step so every app and the hydrator address the same participants/addresses/listeners.
    public static class SeedIdentifiers
    {
        // Participants
        public static readonly Guid NFlixParticipant =
            new Guid("a817f520-c7e5-4831-a67b-171902bf28ba");

        public static readonly Guid MediaItemServiceParticipant =
            new Guid("7a7513d8-bc8f-4a7f-b740-b00bbca519c6");

        public static readonly Guid SofaBoxParticipant =
            new Guid("72edb46a-4e55-49dc-8b92-16baf040c6fd");

        public static readonly Guid JoeParticipant =
            new Guid("523a9adc-a582-42da-ab0d-762eb8782962");

        public static readonly Guid AnnParticipant =
            new Guid("ab496d88-7cf5-4e8f-af45-5e75583fb5d0");

        public static readonly Guid FlakyBoxParticipant =
            new Guid("dd9c020b-b528-4058-9ca0-750ef128b9ca");

        // The EventHighway.ClientV2.SubstrateApi chat app. It both publishes (the UI submits media
        // items through its own /submit endpoint under this identity) and subscribes (its
        // unfiltered listener below relays every new release to its /receive endpoint).
        public static readonly Guid SubstrateApiParticipant =
            new Guid("80aa28e0-faca-4984-a1ac-bfa2e2d3926c");

        // Participant secrets
        public static readonly Guid NFlixSecret =
            new Guid("5b1f7ee4-d421-4e2a-a534-c41cb1627bd1");

        public static readonly Guid MediaItemServiceSecret =
            new Guid("69e4a4ad-fcd1-446b-838e-961dd37763e5");

        public static readonly Guid SubstrateApiSecret =
            new Guid("5279a8cd-fadb-4a6e-b4fc-dab683202c8f");

        // Participant secret values (what publishers present; verified by the substrate core).
        public const string NFlixSecretValue = "NFlix";
        public const string MediaItemServiceSecretValue = "MediaItemService";
        public const string SubstrateApiSecretValue = "SubstrateApi";

        // Event addresses
        public static readonly Guid NFlixNewReleasesAddress =
            new Guid("be0dd6e0-b545-435d-9541-d1ac386469ce");

        public static readonly Guid NFlixExternalContributionsAddress =
            new Guid("2cdc1b26-f5b6-43f4-9855-f2ca13ed02a9");

        // Event listeners
        public static readonly Guid SofaBoxNewReleasesListener =
            new Guid("07864612-508c-4177-a0b6-061f9efa48d8");

        public static readonly Guid JoeGoodMoviesListener =
            new Guid("523a9adc-a582-42da-ab0d-762eb8782962");

        public static readonly Guid AnnNewReleasesListener =
            new Guid("ab496d88-7cf5-4e8f-af45-5e75583fb5d0");

        public static readonly Guid FlakyBoxNewReleasesListener =
            new Guid("c00c96c4-ad10-47a8-b2bc-b8d18efcde5a");

        // Deliberately unfiltered and with no promoted properties: every new release reaching the
        // address is relayed, whole, to the SubstrateApi's /receive endpoint and onto its chat UI.
        public static readonly Guid SubstrateApiNewReleasesListener =
            new Guid("d90282d6-fd28-4914-a898-8dff75a112ee");

        // MediaItemService's ONLY subscription: the listener on NFlix-ExternalContributions.
        // It does not listen to NFlix-NewReleases — it publishes there.
        public static readonly Guid MediaItemServiceContributionsListener =
            new Guid("b14dbd4c-1494-4a42-b35c-e5323db70a03");

        // Event handlers. Stable Ids (mirrored across the sample apps and the Portal.Seed hydrator)
        // so a listener registered by one app references a handler another app also registers under
        // the same Id — dispatch then works regardless of which app created the listener or the run
        // order.
        public static readonly Guid SofaBoxHandler =
            new Guid("6326cae3-04ff-411f-93fb-e606859390f6");

        public static readonly Guid JoeHandler =
            new Guid("9846c9e3-2843-4a2e-a586-4321c3a5f1a9");

        public static readonly Guid AnnHandler =
            new Guid("a9079276-fbbe-4176-9744-9fee3354f3e7");

        public static readonly Guid FlakyBoxHandler =
            new Guid("9cc4ac0e-6ef2-4c99-a0cd-831867fff9df");

        public static readonly Guid MediaItemServiceHandler =
            new Guid("6743a4f1-07c4-4def-9d1f-e0ca926c6b90");

        // Registered by the sample apps (BasicApp and the SubstrateApi itself), each pointing at the
        // SAME running SubstrateApi /receive endpoint — so whichever app dispatches an event, its
        // delivery lands on the one chat UI.
        public static readonly Guid SubstrateApiHandler =
            new Guid("3282e8fd-b6ae-4bbc-86f4-d019ffa7ccca");
    }
}
