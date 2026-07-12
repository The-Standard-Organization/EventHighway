// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems.Exceptions;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    public partial class ExternalMediaItemService
    {
        private static void ValidateExternalMediaItemOnAdd(
            MediaItem mediaItem,
            string participantId,
            string participantSecret)
        {
            ValidateMediaItemIsNotNull(mediaItem);

            Validate(
                (Rule: IsInvalidId(participantId),
                    Parameter: nameof(participantId)),

                (Rule: IsInvalid(participantSecret),
                    Parameter: nameof(participantSecret)),

                (Rule: IsInvalid(mediaItem.Id),
                    Parameter: nameof(MediaItem.Id)),

                (Rule: IsInvalid(mediaItem.Title),
                    Parameter: nameof(MediaItem.Title)),

                (Rule: IsInvalid(mediaItem.Type),
                    Parameter: nameof(MediaItem.Type)));
        }

        private static void ValidateMediaItemIsNotNull(MediaItem mediaItem)
        {
            if (mediaItem is null)
            {
                throw new NullExternalMediaItemException(
                    message: "External media item is null.");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalidId(string participantId) => new
        {
            Condition = Guid.TryParse(participantId, out Guid parsedParticipantId) is false
                || parsedParticipantId == Guid.Empty,

            Message = "Id is required and must be a valid non-empty GUID"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidExternalMediaItemException =
                new InvalidExternalMediaItemException(
                    message: "External media item is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidExternalMediaItemException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidExternalMediaItemException.ThrowIfContainsErrors();
        }
    }
}
