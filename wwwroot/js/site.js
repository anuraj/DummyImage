// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#Height").on("input propertychange paste", function () {
        buildUrl();
    });
    $("#Width").on("input propertychange paste", function () {
        buildUrl();
    });
    $("#TextInput").on("input propertychange paste", function () {
        buildUrl();
    });
    var colors = [];
    for (r = 0; r < 16; r += 3) {
        for (g = 0; g < 16; g += 3) {
            for (b = 0; b < 16; b += 3) {
                colors.push(r.toString(16) + g.toString(16) + b.toString(16));
            }
        }
    }
    $("#bgColor").change(function () {
        buildUrl();
    });
    $("#fgColor").change(function () {
        buildUrl();
    });
    var $bgColor = $("#bgColor");
    $bgColor.empty();
    var $fgColor = $("#fgColor");
    $fgColor.empty();
    $.each(colors, function (index, value) {
        if (value === "ccc") {
            $bgColor.append("<option selected style=\"background-color:#" + value + "\">" + value + "</option>");
        } else {
            $bgColor.append("<option style=\"background-color:#" + value + "\">" + value + "</option>");
        }

        if (value === "fff") {
            $fgColor.append("<option selected style=\"background-color:#" + value + "\">" + value + "</option>");
        } else {
            $fgColor.append("<option style=\"background-color:#" + value + "\">" + value + "</option>");
        }
    });
    $("#format").change(function () {
        buildUrl();
    });
    var buildUrl = function () {
        var protocol = location.protocol;
        var slashes = protocol.concat("//");
        var port = location.port;
        var host = slashes.concat(window.location.hostname);
        if (port && port !== 80) {
            host = host.concat(":", port);
        }
        host = host.concat("/");
        $("#ImageURL").val(host + $("#Width").val() + "x" + $("#Height").val()
            + "/" + $("#bgColor").val() + "/" + $("#fgColor").val() + "." + $("#format").val()
            + ($("#TextInput").val().length > 1 ? "?text=" + encodeURI($("#TextInput").val()) : ""));
        $("#PreviewImage").attr("src", $("#ImageURL").val());
    };

    buildUrl();

    $("#CopyToClipboard").click(function () {
        var copyText = document.getElementById("ImageURL");
        copyText.select();
        document.execCommand("copy");
        if (window.getSelection) { window.getSelection().removeAllRanges(); }
        else if (document.selection) { document.selection.empty(); }
        ShowCopiedMessage();
    });

    function ShowCopiedMessage() {
        var snackbar = document.getElementById("snackbar");
        snackbar.className = "show";
        setTimeout(function () { snackbar.className = snackbar.className.replace("show", ""); }, 3000);
    }
});