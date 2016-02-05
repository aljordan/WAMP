/*jslint browser: true*/
/*global $, jQuery*/

var breadCrumbArray = [];


function escapeQuotes(word) {
    return word.replace(/'/g, "\\'");
}


function buildBreadCrumb(crumb, restart) {
    if (restart === true) {
        breadCrumbArray = [];
    }
    // check to see if item already exists in array
    var index = breadCrumbArray.indexOf(crumb);

    // if the item is in the array, remove everything after that item
    if (index > -1) {
        breadCrumbArray.splice(index + 1, breadCrumbArray.length - (index + 1));
    } else {
        breadCrumbArray.push(crumb);
    }

    $("#breadcrumb").empty();
    for (var count = 0; count < breadCrumbArray.length; count++) {
        $('#breadcrumb').append(breadCrumbArray[count]);
        if (count < breadCrumbArray.length -1 )
            $('#breadcrumb').append('&nbsp;-&nbsp;');
    }
}


function populatePlaylist() {
    //$("#playlist").empty();
    $.ajax({
        url: 'http://localhost:9090/wamp/currentplaylist',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var currentPlaylistJson = $.parseJSON(data.CurrentPlaylistResult);
            paintPlaylist(currentPlaylistJson);
        }
    });
}


function paintPlaylist(playlistData) {
    $("#playlist").empty();
    for (var count = 0; count < playlistData.length; count++) {
        $('#playlist').append('<div>' +
            '<span class="large"><a class="large" title="Play" onclick="playTrack(' + playlistData[count].Id + ')" href="#">'
            + playlistData[count].Title + '</a></span>'
            + '<div class="bar-right"><span class="large" style="text-align: right;"><a class="large" title="Remove" onclick="removeSongFromPlaylist(' + playlistData[count].Id + ')" href="#">'
            + 'X</a></span></div><br>' +
            '<span class="small"><a class="small" onclick="getAlbumsByArtist(\'' + escapeQuotes(playlistData[count].Artist) + '\')" href="#">' + playlistData[count].Artist + '</a></span><br>' +
            '<span class="small"><a class="small" onclick="getSongsByAlbum(\'' + escapeQuotes(playlistData[count].Album) + '\')" href="#">' + playlistData[count].Album + '</a></span><br>' +
            '</div><br>');
    }
}


function getGenres() {
    $.ajax({
        url: 'http://localhost:9090/wamp/genres',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#playlistEditor').empty();
            var genres = $.parseJSON(data.GetGenresResult);
            for (var count = 0; count < genres.length; count++) {
                $('#playlistEditor').append('<div>' +
                    '<span class="large"><a class="large" onclick="getArtistsByGenre(\'' + escapeQuotes(genres[count]) + '\')" href="#">'
                    + genres[count] + '</a></span><br>' +
                    '</div>');
            }
            buildBreadCrumb('<a class="small" onclick="getGenres()" href="#">&nbsp;Genres</a>', true);
        }
    });
}


function getArtists() {
    $.ajax({
        url: 'http://localhost:9090/wamp/artists',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#playlistEditor').empty();
            var artists = $.parseJSON(data.GetArtistsResult);
            for (var count = 0; count < artists.length; count++) {
                $('#playlistEditor').append('<div>' +
                    '<span class="large"><a class="large" onclick="getAlbumsByArtist(\'' + escapeQuotes(artists[count]) + '\')" href="#">'
                    + artists[count] + '</a></span><br>' +
                    '</div>');
            }
            buildBreadCrumb('<a class="small" onclick="getArtists()" href="#">&nbsp;Artists</a>', true);
        }
    });
}


function getAlbums() {
    $.ajax({
        url: 'http://localhost:9090/wamp/albums',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#playlistEditor').empty();
            var albums = $.parseJSON(data.GetAlbumsResult);
            for (var count = 0; count < albums.length; count++) {
                $('#playlistEditor').append('<div>' +
                    '<span class="large"><a class="large" onclick="getSongsByAlbum(\'' + escapeQuotes(albums[count]) + '\')" href="#">'
                    + albums[count] + '</a></span><br>' +
                    '</div>');
            }
            buildBreadCrumb('<a class="small" onclick="getAlbums()" href="#">&nbsp;Albums</a>', true);
        }
    });
}


function getArtistsByGenre(genre) {
    $.ajax({
        url: 'http://localhost:9090/wamp/artistsbygenre/' + genre,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#playlistEditor').empty();
            var artists = $.parseJSON(data.GetArtistByGenreResult);
            for (var count = 0; count < artists.length; count++) {
                $('#playlistEditor').append('<div>' +
                    '<span class="large"><a class="large" onclick="getAlbumsByArtist(\'' + escapeQuotes(artists[count]) + '\')" href="#">'
                    + artists[count] + '</a></span><br>' +
                    '</div>');
            }
            buildBreadCrumb('<a class="small" onclick="getArtistsByGenre(\'' + escapeQuotes(genre) + '\')" href="#">&nbsp;' + genre + '</a>', false);
        }
    });
}

function getAlbumsByArtist(artist) {
    $.ajax({
        url: 'http://localhost:9090/wamp/albumsbyartist/' + artist,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#playlistEditor').empty();
            var albums = $.parseJSON(data.GetAlbumsByArtistResult);
            for (var count = 0; count < albums.length; count++) {
                $('#playlistEditor').append('<div>' +
                    '<span class="large"><a class="large" onclick="getSongsByAlbum(\'' + escapeQuotes(albums[count]) + '\')" href="#">'
                    + albums[count] + '</a></span><br>' +
                    '</div>');

            }
            buildBreadCrumb('<a class="small" onclick="getAlbumsByArtist(\'' + escapeQuotes(artist) + '\')" href="#">&nbsp;' + artist + '</a>', false);
        }
    });
}

function getSongsByAlbum(album) {
    $.ajax({
        url: 'http://localhost:9090/wamp/songsbyalbum/' + album,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#playlistEditor').empty();
            var songs = $.parseJSON(data.GetSongsByAlbumResult);
            for (var count = 0; count < songs.length; count++) {
                $('#playlistEditor').append('<div>' +
                    '<span class="large"><a class="large" title="Add to playlist" onclick="addSongToPlaylist(\'' + songs[count].Id + '\')" href="#">'
                    + songs[count].Title + '</a></span><br>' +
                    '</div>');
            }
            buildBreadCrumb('<a class="small" onclick="getSongsByAlbum(\'' + escapeQuotes(album) + '\')" href="#">&nbsp;' + album + '</a>', false);
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
            document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(track.Artist) + '\')" href="#">' + track.Artist + '</a>';
            document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(track.Album) + '\')" href="#">' + track.Album + '</a>';
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
            document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(track.Artist) + '\')" href="#">' + track.Artist + '</a>';
            document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(track.Album) + '\')" href="#">' + track.Album + '</a>';
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
            document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(track.Artist) + '\')" href="#">' + track.Artist + '</a>';
            document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(track.Album) + '\')" href="#">' + track.Album + '</a>';
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
            document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(track.Artist) + '\')" href="#">' + track.Artist + '</a>';
            document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(track.Album) + '\')" href="#">' + track.Album + '</a>';
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

function addSongToPlaylist(id) {
    $.ajax({
        url: 'http://localhost:9090/wamp/addtrack/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $("#playlist").empty();
            var currentPlaylistJson = $.parseJSON(data.AddTrackResult);
            paintPlaylist(currentPlaylistJson);
        }
    });
}


function removeSongFromPlaylist(id) {
    $.ajax({
        url: 'http://localhost:9090/wamp/removetrack/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $("#playlist").empty();
            var currentPlaylistJson = $.parseJSON(data.RemoveTrackResult);
            paintPlaylist(currentPlaylistJson);
        }
    });
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



