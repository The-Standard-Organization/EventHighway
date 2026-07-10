// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems.Exceptions;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    public partial class ExternalMediaItemService
    {
        private static void ValidateExternalMediaItemOnAdd(ExternalMediaItem externalMediaItem)
        {
            ValidateExternalMediaItemIsNotNull(externalMediaItem);
            ValidateMediaItemIsNotNull(externalMediaItem.MediaItem);

            Validate(
                (Rule: IsInvalid(externalMediaItem.ParticipantId),
                    Parameter: nameof(ExternalMediaItem.ParticipantId)),

                (Rule: IsInvalid(externalMediaItem.Secret),
                    Parameter: nameof(ExternalMediaItem.Secret)),

                (Rule: IsInvalid(externalMediaItem.MediaItem.Id),
                    Parameter: nameof(MediaItem.Id)),

                (Rule: IsInvalid(externalMediaItem.MediaItem.Title),
                    Parameter: nameof(MediaItem.Title)),

                (Rule: IsInvalid(externalMediaItem.MediaItem.Type),
                    Parameter: nameof(MediaItem.Type)));
        }

        private static void ValidateExternalMediaItemIsNotNull(ExternalMediaItem externalMediaItem)
        {
            if (externalMediaItem is null)
            {
                throw new NullExternalMediaItemException(
                    message: "External media item is null.");
            }
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
