using System.Text.Json;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventPublisherApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventPublisherApi.Controllers
{
    [ApiController]
    public class SubmitEventController : ControllerBase
    {
        private readonly IEventHighwayClient eventHighwayClient;

        public SubmitEventController(IEventHighwayClient eventHighwayClient) =>
            this.eventHighwayClient = eventHighwayClient;

        [HttpPost("api/submit-event-and-observe")]
        public async ValueTask<ActionResult> PostAndObserveAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string listenerBaseUrl = "https://localhost:7201/api";

            // School Address & Listener
            Guid schoolAddressId = Guid.NewGuid();

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(new EventAddressV1
                {
                    Id = schoolAddressId,
                    Name = "school-events-observe",
                    Description = "Observable address for school events",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "School Observe Listener",
                    Description = "Listens and observes school events",
                    Endpoint = $"{listenerBaseUrl}/schools",
                    HeaderSecret = "observe-secret-123",
                    EventAddressId = schoolAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // Group Address & Listener
            Guid groupAddressId = Guid.NewGuid();

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(new EventAddressV1
                {
                    Id = groupAddressId,
                    Name = "group-events-observe",
                    Description = "Observable address for group events",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "Group Observe Listener",
                    Description = "Listens and observes group events",
                    Endpoint = $"{listenerBaseUrl}/groups",
                    HeaderSecret = "observe-secret-123",
                    EventAddressId = groupAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // Student Address & Listener
            Guid studentAddressId = Guid.NewGuid();

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(new EventAddressV1
                {
                    Id = studentAddressId,
                    Name = "student-events-observe",
                    Description = "Observable address for student events",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "Student Observe Listener",
                    Description = "Listens and observes student events",
                    Endpoint = $"{listenerBaseUrl}/students",
                    HeaderSecret = "observe-secret-123",
                    EventAddressId = studentAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "User Observe Listener",
                    Description = "Listens and observes user events",
                    Endpoint = $"{listenerBaseUrl}/users",
                    HeaderSecret = "observe-secret-123",
                    EventAddressId = studentAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // Publish School Event
            EventV1 submittedSchoolEvent =
                await this.eventHighwayClient.EventV1s
                    .SubmitEventV1AsyncV1(new EventV1
                    {
                        Id = Guid.NewGuid(),
                        Content = JsonSerializer.Serialize(new School
                        {
                            Id = Guid.NewGuid(),
                            Name = "Global University",
                            Description = "A top university"
                        }),
                        EventAddressId = schoolAddressId,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // Publish Group Event
            EventV1 submittedGroupEvent =
                await this.eventHighwayClient.EventV1s
                    .SubmitEventV1AsyncV1(new EventV1
                    {
                        Id = Guid.NewGuid(),
                        Content = JsonSerializer.Serialize(new Group
                        {
                            Id = Guid.NewGuid(),
                            Name = "Computer Science 101",
                            Description = "Intro to CS"
                        }),
                        EventAddressId = groupAddressId,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // Publish Student Event
            EventV1 submittedStudentEvent =
                await this.eventHighwayClient.EventV1s
                    .SubmitEventV1AsyncV1(new EventV1
                    {
                        Id = Guid.NewGuid(),
                        Content = JsonSerializer.Serialize(new Student
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "John",
                            LastName = "Doe"
                        }),
                        EventAddressId = studentAddressId,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            return Ok(new
            {
                Mode = "Observe",
                SchoolEvent = submittedSchoolEvent,
                GroupEvent = submittedGroupEvent,
                StudentEvent = submittedStudentEvent
            });
        }

        [HttpPost("api/submit-event-and-forget")]
        public async ValueTask<ActionResult> PostAndForgetAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string listenerBaseUrl = "https://localhost:7201/api";

            // School Address & Listener
            Guid schoolAddressId = Guid.NewGuid();

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(new EventAddressV1
                {
                    Id = schoolAddressId,
                    Name = "school-events-forget",
                    Description = "Fire and forget address for school events",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "School Forget Listener",
                    Description = "Consumes school events without observation",
                    Endpoint = $"{listenerBaseUrl}/schools",
                    HeaderSecret = "forget-secret-456",
                    EventAddressId = schoolAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // Group Address & Listener
            Guid groupAddressId = Guid.NewGuid();

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(new EventAddressV1
                {
                    Id = groupAddressId,
                    Name = "group-events-forget",
                    Description = "Fire and forget address for group events",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "Group Forget Listener",
                    Description = "Consumes group events without observation",
                    Endpoint = $"{listenerBaseUrl}/groups",
                    HeaderSecret = "forget-secret-456",
                    EventAddressId = groupAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // Student Address & Listener
            Guid studentAddressId = Guid.NewGuid();

            await this.eventHighwayClient.EventAddressV1s
                .RegisterEventAddressV1Async(new EventAddressV1
                {
                    Id = studentAddressId,
                    Name = "student-events-forget",
                    Description = "Fire and forget address for student events",
                    CreatedDate = now,
                    UpdatedDate = now
                });

            await this.eventHighwayClient.EventListenerV1s
                .RegisterEventListenerV1Async(new EventListenerV1
                {
                    Id = Guid.NewGuid(),
                    Name = "Student Forget Listener",
                    Description = "Consumes student events without observation",
                    Endpoint = $"{listenerBaseUrl}/students",
                    HeaderSecret = "forget-secret-456",
                    EventAddressId = studentAddressId,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // Publish School Event (different data)
            EventV1 submittedSchoolEvent =
                await this.eventHighwayClient.EventV1s
                    .SubmitEventV1Async(new EventV1
                    {
                        Id = Guid.NewGuid(),
                        Content = JsonSerializer.Serialize(new School
                        {
                            Id = Guid.NewGuid(),
                            Name = "Future Tech Institute",
                            Description = "Innovation focused university"
                        }),
                        EventAddressId = schoolAddressId,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // Publish Group Event (different data)
            EventV1 submittedGroupEvent =
                await this.eventHighwayClient.EventV1s
                    .SubmitEventV1Async(new EventV1
                    {
                        Id = Guid.NewGuid(),
                        Content = JsonSerializer.Serialize(new Group
                        {
                            Id = Guid.NewGuid(),
                            Name = "AI Research Group",
                            Description = "Advanced AI research division"
                        }),
                        EventAddressId = groupAddressId,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // Publish Student Event (different data)
            EventV1 submittedStudentEvent =
                await this.eventHighwayClient.EventV1s
                    .SubmitEventV1Async(new EventV1
                    {
                        Id = Guid.NewGuid(),
                        Content = JsonSerializer.Serialize(new Student
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "Alice",
                            LastName = "Smith"
                        }),
                        EventAddressId = studentAddressId,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            return Ok(new
            {
                Mode = "FireAndForget",
                SchoolEvent = submittedSchoolEvent,
                GroupEvent = submittedGroupEvent,
                StudentEvent = submittedStudentEvent
            });
        }
    }
}
