// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function normalizeUrl(url) {

    //Use Url-Knife in order to match via fuzzy pattern
    var normUrl = Pattern.UrlArea.normalizeUrl(url);

    if (!!normUrl.normalizedUrl) {
        console.log("Got Normalized URL " + normUrl.normalizedUrl);
        return normUrl;
    }
    else {
        console.log("Normalized URL is null!");
        return url;
    }

}

function getShortUrl() {
    //response text needs to be specified here for return type

    const redirHost = typeof urlBase == "string" ?
     `https://localhost:${urlBase}/` : "https://localhost/";

    var url = document.querySelector("#url-entry-input").value;
    
    url = normalizeUrl(url);

    fetch(`/api/Url`, {
        method: "POST", body: JSON.stringify(url),
        headers: {
            'Accept': 'application/json, text/plain',
            'Content-Type': 'application/json;charset=UTF-8'
        }
    })
        .then(async function (response) {
            const text = await response.text();

            let input = document.querySelector("#url-shorten-input");
            input.value = text.replace(/\\([\s\S])|(")/g, "");

            if (!text.includes("invalid") && !text.includes("error processing url please try again."))
                document.querySelector("#btn-confirm-url").style.display = 'block';

            return fetch(`/api/Url/${text.replace(/\\([\s\S])|(")/g, "").replace(redirHost, "")}`, { method: "POST" })
        })
        .then(async function (response) {
            const text = await response.text();
            let hash = document.querySelector("#url-shorten-hash");
            hash.value = text;
        });
}


