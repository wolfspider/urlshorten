// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function getShortUrl() {
//response text needs to be specified here for return type
    
    var url = document.querySelector("#url-entry-input").value;

    fetch(`https://localhost:5001/api/Url?url=${url}`, { method: "POST" })
        .then(function (response) {
            return response.text().then(function (text) {
                console.log(text);
                
                var input = document.querySelector("#url-shorten-input");

                input.value = text;

            })
        });
}