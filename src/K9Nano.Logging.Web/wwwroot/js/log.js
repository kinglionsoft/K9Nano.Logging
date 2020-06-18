"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/logHub").withAutomaticReconnect().build();
var currentGroup = "";
var apps = ["ALL"];
var $messagesList;

function join(add) {
    return connection.invoke("Join", add, currentGroup)
        .then(function () {
            console.log("Joined group: " + add + "; Removed from: " + currentGroup);
            currentGroup = add;
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function leave() {
    return connection.invoke("Leave", currentGroup)
        .then(function () {
            console.log("Left from group: " + currentGroup);
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function encodedMsg(message) {
    if (!message) return "";
    return message.replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}

function prefixInteger(num, length) {
    return (Array(length).join('0') + num).slice(-length);
}

function formatDate(timestamp) {
    var d = new Date(timestamp);
    return prefixInteger(d.getHours(), 2)
        + ":"
        + prefixInteger(d.getMinutes(), 2)
        + ":"
        + prefixInteger(d.getSeconds(), 2)
        + "."
        + prefixInteger(d.getMilliseconds(), 3);
}

function formatLevel(level) {
    switch (level) {
        case 0:
            return "V";
        case 1:
            return "D";
        case 2:
            return "I";
        case 3:
            return "W";
        case 4:
            return "E";
        case 5:
            return "F";
        default:
            return level;
    }
}

function getMessageClass(exception) {
    return exception ? " d-w-50" : "";
}

function generateException(exception) {
    if (exception) {
        return '<div class="log-box-item"><label class="log-box-label text-black-50 min-w-4">异常</label><span class="log-box-text">' + encodedMsg(exception) + '</span></div>';
    }

    return "";
}

$(function () {
    document.getElementById("inputFrom").valueAsDate = document.getElementById("inputTo").valueAsDate = new Date();

    $messagesList = $("#messagesList");

    $messagesList.css({ 'height': document.body.scrollHeight - 60 });

    connection.on("ReceiveMessage", function (log) {
        if (apps.indexOf(log.application) < 0) {
            apps.push(log.application);
            $("#inputApp").append('<option value="' + log.application + '">' + log.application + "</option>");
        }
        var logLevel = formatLevel(log.level);

        var html = '<li class="list-group-item">' +
            '<section class="log-box d-flex" onclick="$(this).toggleClass(\'flex-column\')">' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">时间</label>' +
            '<span class="log-box-text d-w-6">' + formatDate(log.timestamp) + '</span>' +
            '</div>' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">级别</label>' +
            '<span class="log-box-text d-w-2  log-level-' + logLevel + '">' + logLevel + '</span>' +
            '</div>' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">主机</label>' +
            '<span class="log-box-text d-w-5">' + log.machine + '</span>' +
            '</div>' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">应用</label>' +
            '<span class="log-box-text d-w-10">' + log.application + '</span>' +
            '</div>' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">分类</label>' +
            '<span class="log-box-text d-w-10">' + log.category + '</span>' +
            '</div>' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">Trace</label>' +
            '<span class="log-box-text d-w-10">' + (log.traceId || "") + '</span>' +
            '</div>' +
            '<div class="log-box-item">' +
            '<label class="log-box-label text-black-50 min-w-4">消息</label>' +
            '<span class="log-box-text' + getMessageClass(log.exception) + '">' + encodedMsg(log.message) + '</span>' +
            '</div> ' +
            generateException(log.exception) +
            '</section></li>';

        $messagesList.append(html);

        var logs = $messagesList.children('li');
        if (logs.length > 500) {
            logs[0].remove();
        }
        logs[logs.length - 1].scrollIntoView();
    });

    connection.start().then(function () {
        console.log("connection started");
        join("ALL");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $("#inputApp").on("change",
        function () {
            join($("#inputApp").val());
        });

    $("#btnPause").on("click",
        function (e) {
            var $this = $(this);
            if ($this.text() === "暂停") {
                leave()
                    .then(function () {
                        $this.text("恢复");
                    });
            } else {
                join(currentGroup)
                    .then(function () {
                        $this.text("暂停");
                    });
            }
        });
});

