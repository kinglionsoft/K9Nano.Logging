"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/logHub").withAutomaticReconnect().build();
var currentGroup = "";
var apps = ["ALL"];

connection.on("ReceiveMessage", function (app, message) {
    if (apps.indexOf(app) < 0) {
        apps.push(app);
        $("#inputApp").append('<option value="' + app + '">' + app + "</option>");
    }

    var encodedMsg = message.replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");

    var li = document.createElement("li");
    li.textContent = encodedMsg;
    li.classList.add("list-group-item");
    li.classList.add("text-pre");
    document.getElementById("messagesList").appendChild(li);

    if ($("#messagesList li").length > 500) {
        $("#messagesList li:first").remove();
    }
    $("#messagesList li:last")[0].scrollIntoView();
});

connection.start().then(function () {
    console.log("connection started");
    join("ALL");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.pus

function join(add) {
    connection.invoke("Join", add, currentGroup)
        .then(function () {
            console.log("Joined group: " + add + "; Removed from: " + currentGroup);
            currentGroup = add;
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
}

$(function () {
    document.getElementById("inputFrom").valueAsDate = document.getElementById("inputTo").valueAsDate = new Date();

    $("#messagesList").css({ 'height': document.body.scrollHeight - 60 });

    $("#inputApp").on("change",
        function () {
            join($("#inputApp").val());
        });

    $("#messagesList").on("click",
        function(e) {
            var li = $(e.target);
            li.toggleClass('text-pre-wrap');
        });
});

