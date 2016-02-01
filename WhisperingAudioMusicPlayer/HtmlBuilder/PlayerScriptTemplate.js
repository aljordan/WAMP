/*jslint browser: true*/
/*global $, jQuery*/

function populatePlaylist() {
    $("#playlist").empty();
    $.ajax({
        url: 'http://localhost:9090/wamp/currentplaylist',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var currentPlaylistJson = $.parseJSON(data.CurrentPlaylistResult);
            for (var count = 0; count <  currentPlaylistJson.length; count++) {
                //var title = currentPlaylistJson[count].Title;
                //console.log(title);
                $('#playlist').append('<div>' + 
                    '<span class="large"><a class="large" onclick="playTrack(' + currentPlaylistJson[count].Id + ')" href="#">'
                    + currentPlaylistJson[count].Title + '</a></span><br>' +
                    '<span class="small">' + currentPlaylistJson[count].Artist + '</span><br>' +
                    '<span class="small">' + currentPlaylistJson[count].Album + '</span><br>' +
                    '</div><br>');
            }
//            console.log(json[0]["Title"]);
        }
    });
}


function play() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            var track = $.parseJSON(Data.PlayResult);
            document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("artistName").innerHTML = track.Artist;
            document.getElementById("albumName").innerHTML = track.Album;
        }
    };
    xhttp.open("GET", "http://localhost:9090/wamp/play", true);
    xhttp.send();
}


function stop() {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", "http://localhost:9090/wamp/stop", true);
    xhttp.send();
}


function next() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            var track = $.parseJSON(Data.NextResult);
            document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("artistName").innerHTML = track.Artist;
            document.getElementById("albumName").innerHTML = track.Album;
        }
    };
    xhttp.open("GET", "http://localhost:9090/wamp/next", true);
    xhttp.send();
}


function previous() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            var track = $.parseJSON(Data.PreviousResult);
            document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("artistName").innerHTML = track.Artist;
            document.getElementById("albumName").innerHTML = track.Album;
        }
    };
    xhttp.open("GET", "http://localhost:9090/wamp/previous", true);
    xhttp.send();
}

function getVolume() {
    var url = "http://localhost:9090/wamp/volume/";
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            document.getElementById("volumeLabel").innerHTML = "Volume: " + Data.GetVolumeResult;
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();

}

function playTrack(id) {
    var url = "http://localhost:9090/wamp/playtrack/" + id;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            var track = $.parseJSON(Data.PlayTrackResult);
            document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("artistName").innerHTML = track.Artist;
            document.getElementById("albumName").innerHTML = track.Album;
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

function changeVolume(direction) {
    var url = "http://localhost:9090/wamp/changeVolume/" + direction;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            document.getElementById("volumeLabel").innerHTML = "Volume: " + Data.ChangeVolumeResult;
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

$(document).ready(function () {
    getVolume();
    populatePlaylist();
});



