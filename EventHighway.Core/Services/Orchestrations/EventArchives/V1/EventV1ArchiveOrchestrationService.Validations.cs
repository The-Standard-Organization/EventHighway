// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventV1ArchiveOrchestrationService
    {
        private static void ValidateEventV1Arhive(EventV1Archive eventV1Archive)
        {
            ValidateEventV1ArhiveIsNotNull(eventV1Archive);
            ValidateListenerEventV1ArhivesAreNotNull(eventV1Archive);
        }

        private static void ValidateEventV1ArhiveIsNotNull(EventV1Archive eventV1Archive)
        {
            if (eventV1Archive is null)
            {
                throw new NullEventV1ArchiveOrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventV1ArhivesAreNotNull(EventV1Archive eventV1Archive)
        {
            if (eventV1Archive.ListenerEventV1Archives is null)
            {
                throw new NullListenerEventV1ArchivesOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsInvalidEnum(value) is true,
            Message = "Value is not recognized"
        };

        private static bool IsInvalidEnum<T>(T enumValue)
        {
            bool isDefined = Enum.IsDefined(
                enumType: typeof(T),
                value: enumValue);

            return isDefined is false;
        }

        private async ValueTask ValidateArchiveDeletionType(ArchiveDeletionPolicy type) =>
            Validate((Rule: IsInvalid(type), Parameter: nameof(EventV1Archive)));

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventAddressV1ProcessingException =
                new InvalidEventV1ArchiveOrchestrationException(
                    message: "Event archive is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventAddressV1ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventAddressV1ProcessingException.ThrowIfContainsErrors();
        }
    }
}
