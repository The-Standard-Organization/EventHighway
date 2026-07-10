// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems.Exceptions;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    internal partial class MediaItemService
    {
        private static void ValidateMediaItemOnAdd(MediaItem mediaItem)
        {
            ValidateMediaItemIsNotNull(mediaItem);

            Validate(
                (Rule: IsInvalid(mediaItem.Id), Parameter: nameof(MediaItem.Id)),
                (Rule: IsInvalid(mediaItem.Title), Parameter: nameof(MediaItem.Title)),
                (Rule: IsInvalid(mediaItem.Type), Parameter: nameof(MediaItem.Type)));
        }

        private static void ValidateMediaItemOnModify(MediaItem mediaItem)
        {
            ValidateMediaItemIsNotNull(mediaItem);

            Validate(
                (Rule: IsInvalid(mediaItem.Id), Parameter: nameof(MediaItem.Id)),
                (Rule: IsInvalid(mediaItem.Title), Parameter: nameof(MediaItem.Title)),
                (Rule: IsInvalid(mediaItem.Type), Parameter: nameof(MediaItem.Type)));
        }

        private static void ValidateMediaItemId(Guid mediaItemId) =>
            Validate((Rule: IsInvalid(mediaItemId), Parameter: nameof(MediaItem.Id)));

        private static void ValidateMediaItemIsNotNull(MediaItem mediaItem)
        {
            if (mediaItem is null)
            {
                throw new NullMediaItemException(
                    message: "Media item is null.");
            }
        }

        private static void ValidateStorageMediaItemExists(MediaItem maybeMediaItem, Guid mediaItemId)
        {
            if (maybeMediaItem is null)
            {
                throw new NotFoundMediaItemException(
                    message: $"Could not find media item with id: {mediaItemId}.");
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
            var invalidMediaItemException =
                new InvalidMediaItemException(
                    message: "Media item is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidMediaItemException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidMediaItemException.ThrowIfContainsErrors();
        }
    }
}
