// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

window.eventChat = {
    // A chat that does not follow its own conversation is a chat you have to scroll by hand.
    scrollToLatest: function (element) {
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },

    // navigator.clipboard needs a secure context — localhost counts as one, so no fallback is
    // needed for the only host this sample ever runs on.
    copyToClipboard: function (text) {
        return navigator.clipboard.writeText(text);
    }
};
