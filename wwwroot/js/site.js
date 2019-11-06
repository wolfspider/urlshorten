// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function getShortUrl() {
//response text needs to be specified here for return type
    
    var url = document.querySelector("#url-entry-input").value;

    fetch(`/api/Url?url=${url}`, { method: "POST" })
        .then(async function (response) {
            const text = await response.text();
            let input = document.querySelector("#url-shorten-input");
            input.value = text;
            if (!text.includes("invalid"))
                document.querySelector("#btn-confirm-url").style.display = 'block';
        });

    //We can possibly trim this on the backend and get URL Formatted correctly
    
    fetch(`/api/Url/${url.replace("https://","").replace("http://", "")}`, { method: "POST" })
        .then(async function(response) {
            const text = await response.text();
            let hash = document.querySelector("#url-shorten-hash");
            hash.value = text; 
        });
}