// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

// The Process Flow tab draws its diagrams with Mermaid. The vendored mermaid.min.js sets
// window.mermaid the moment it loads (a classic script, ahead of blazor.web.js), so by the time a
// component asks to render, the library is already here — the guard below only covers the case it
// somehow is not.
window.processFlow = {
    initialized: false,

    ensureInitialized: function () {
        if (this.initialized) {
            return true;
        }

        if (typeof window.mermaid === "undefined") {
            return false;
        }

        window.mermaid.initialize({
            startOnLoad: false,

            // Every definition rendered here is an author-written constant with no user input in
            // it, so "loose" only buys the HTML line breaks (<br/>) the labels use — it never sees
            // anything a caller supplied.
            securityLevel: "loose",
            theme: "default"
        });

        this.initialized = true;

        return true;
    },

    // The target element is owned entirely by this call: the component renders it empty and never
    // puts child content in it, so Blazor's diff and the injected SVG never contend for the same
    // DOM. A failed parse is shown in place rather than thrown, so one broken diagram cannot take
    // the tab — or the circuit — down with it.
    render: async function (element, id, definition) {
        if (!element) {
            return;
        }

        if (this.ensureInitialized() === false) {
            element.innerHTML =
                "<div class=\"text-danger small\">Diagram library did not load.</div>";

            return;
        }

        try {
            const result = await window.mermaid.render(id, definition);
            element.innerHTML = result.svg;
        } catch (error) {
            const message = error && error.message ? error.message : "Diagram error";

            element.innerHTML =
                "<pre class=\"text-danger small mb-0\">" + message + "</pre>";
        }
    }
};
