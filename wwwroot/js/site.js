// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function getShortUrl(url) {

    fetch("https://localhost:44340/api/Url?url='alachua'", { method: "POST" })
        .then((rs) => console.log(rs));

}