// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

// Sample-data seeder for the EventHighway portal. Tops up the NFlix-NewReleases address with
// backdated EventV2 + ListenerEventV2 history so the portal lists/filters have several pages of
// data. Self-sufficient: it creates the NFlix participant, the NFlix-NewReleases address and its
// listeners when they are missing, so it can run against an empty database on its own. If the
// ClientV2.BasicApp/SubstrateApi samples have already run, it reconciles to their rows (shared
// well-known Guids) instead of duplicating them. The hydration logic lives in DatabaseHydrator so
// any console app can reuse it.

using EventHighway.Portal.Seed;

const string connectionString =
    "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
    "Trusted_Connection=True;MultipleActiveResultSets=true";

await DatabaseHydrator.HydrateNewReleasesAsync(connectionString);
