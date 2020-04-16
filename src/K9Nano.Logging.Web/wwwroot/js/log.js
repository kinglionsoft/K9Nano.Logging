"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/logHub").build();

connection.on("ReceiveMessage", function (message) {
    var encodedMsg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    li.classList.add("list-group-item");
    document.getElementById("messagesList").appendChild(li);
    if ($('#messagesList li').length > 500) {
        $('#messagesList li:first').remove();
    }
    $('#messagesList li:last')[0].scrollIntoView();
});

connection.start().then(function () {
    console.log("connection started");
}).catch(function (err) {
    return console.error(err.toString());
});

