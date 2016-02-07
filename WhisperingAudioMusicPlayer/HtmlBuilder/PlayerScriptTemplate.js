/*jslint browser: true*/
/*global $, jQuery*/

var breadCrumbArray = []; // for user's trail of functions
var currentSongTitle = ""; // used to ee if the song has changed
var timeoutHandle; // used to control timed callback to server


function escapeQuotes(word) {
    var step1 = word.replace(/'/g, "\\'");
    var step2 = step1.replace(/\(/g, "\\(");
    return step2.replace(/\)/g, "\\)");
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
            + 'x</a></span></div><br>' +
            '<span class="small"><a class="small" title="Show albums" onclick="getAlbumsByArtist(\'' + escapeQuotes(playlistData[count].Artist) + '\')" href="#">' + playlistData[count].Artist + '</a></span><br>' +
            '<span class="small"><a class="small" title="Show songs" onclick="getSongsByAlbum(\'' + escapeQuotes(playlistData[count].Album) + '\')" href="#">' + playlistData[count].Album + '</a></span><br>' +
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
                    '<span class="large"><a class="large" title="Show artists" onclick="getArtistsByGenre(\'' + escapeQuotes(genres[count]) + '\')" href="#">'
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
                    '<span class="large"><a class="large" title="Show albums" onclick="getAlbumsByArtist(\'' + escapeQuotes(artists[count]) + '\')" href="#">'
                    + artists[count] + '</a></span>'
                    + '<div class="bar-right"><span class="large"><a class="large" title="Add to playlist" onclick="addArtistToPlaylist(\'' + escapeQuotes(artists[count]) + '\')" href="#">'
                    + '+</a></span></div><br>'
                    + '<br></div>');
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
                    '<span class="large"><a class="large" title="Show songs" onclick="getSongsByAlbum(\'' + escapeQuotes(albums[count]) + '\')" href="#">'
                    + albums[count] + '</a></span>'
                    + '<div class="bar-right"><span class="large"><a class="large" title="Add to playlist" onclick="addAlbumToPlaylist(\'' + escapeQuotes(albums[count]) + '\')" href="#">'
                    + '+</a></span></div><br>'
                    + '<br></div>');
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
                    '<span class="large"><a class="large" title="Show albums" onclick="getAlbumsByArtist(\'' + escapeQuotes(artists[count]) + '\')" href="#">'
                    + artists[count] + '</a></span>'
                    + '<div class="bar-right"><span class="large"><a class="large" title="Add to playlist" onclick="addArtistToPlaylist(\'' + escapeQuotes(artists[count]) + '\')" href="#">'
                    + '+</a></span></div><br>'
                    + '<br></div>');
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
                    '<span class="large"><a class="large" title="Show songs" onclick="getSongsByAlbum(\'' + escapeQuotes(albums[count]) + '\')" href="#">'
                    + albums[count] + '</a></span>'
                    + '<div class="bar-right"><span class="large"><a class="large" title="Add to playlist" onclick="addAlbumToPlaylist(\'' + escapeQuotes(albums[count]) + '\')" href="#">'
                    + '+</a></span></div><br>'
                    + '<br></div>');
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
            //document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("songName").innerHTML = '<a class="medium" onclick="scrollToInPlaylist(\'' + escapeQuotes(track.Title) + '\')" href="#">' + track.Title + '</a>';
            document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(track.Artist) + '\')" href="#">' + track.Artist + '</a>';
            document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(track.Album) + '\')" href="#">' + track.Album + '</a>';
        }
    };
    xhttp.open("GET", "http://localhost:9090/wamp/play", true);
    xhttp.send();
}


// Once this method starts, it doesn't stop
function getCurrentSongInfo() {
    $.ajax({
        url: 'http://localhost:9090/wamp/songinfo',
        success: function (data) {
            if (data.GetCurrentSongInfoResult !== "") {
                var info = $.parseJSON(data.GetCurrentSongInfoResult);
                if (info.SongTitle !== currentSongTitle) {
                    currentSongTitle = info.SongTitle;
                    scrollToInPlaylist(info.SongTitle);
                }

                //document.getElementById("songName").innerHTML = info.SongTitle;
                document.getElementById("songName").innerHTML = '<a class="medium" onclick="scrollToInPlaylist(\'' + escapeQuotes(info.SongTitle) + '\')" href="#">' + info.SongTitle + '</a>';
                document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(info.Artist) + '\')" href="#">' + info.Artist + '</a>';
                document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(info.Album) + '\')" href="#">' + info.Album + '</a>';
            }
        },
        complete: function () {
            // Schedule the next request when the current one's complete
            timeoutHandle =  setTimeout(getCurrentSongInfo, 1000);
        }
    });
}

function onBlur() {
    clearTimeout(timeoutHandle);
};

function onFocus() {
    getCurrentSongInfo();
};

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
            //document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("songName").innerHTML = '<a class="medium" onclick="scrollToInPlaylist(\'' + escapeQuotes(track.Title) + '\')" href="#">' + track.Title + '</a>';
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
            //document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("songName").innerHTML = '<a class="medium" onclick="scrollToInPlaylist(\'' + escapeQuotes(track.Title) + '\')" href="#">' + track.Title + '</a>';
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
            //document.getElementById("songName").innerHTML = track.Title;
            document.getElementById("songName").innerHTML = '<a class="medium" onclick="scrollToInPlaylist(\'' + escapeQuotes(track.Title) + '\')" href="#">' + track.Title + '</a>';
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
            var currentPlaylistJson = $.parseJSON(data.AddTrackResult);
            paintPlaylist(currentPlaylistJson);
        }
    });
}

function addAlbumToPlaylist(album) {
    $.ajax({
        url: 'http://localhost:9090/wamp/addalbum/' + album,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var currentPlaylistJson = $.parseJSON(data.AddAlbumResult);
            paintPlaylist(currentPlaylistJson);
        }
    });
}


function addArtistToPlaylist(artist) {
    $.ajax({
        url: 'http://localhost:9090/wamp/addartist/' + artist,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var currentPlaylistJson = $.parseJSON(data.AddArtistResult);
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
            // the next line will cause the current song to be highlited in the playlist
            currentSongTitle = "";
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

function scrollToInPlaylist(title) {
    $("#playlist > div > span > .large").css("color", "white");
    //$("#playlist > div:contains(" + title + ")").css("color", "#ffcc00");
    $("#playlist > div > span:contains(" + title + ") > .large").css("color", "#ffcc00");
    //$parentDiv.scrollTop($parentDiv.scrollTop() + $innerListItem.position().top);
    //$("#playlist").scrollTop($("#playlist").scrollTop() + $("#playlist > div:contains(" + title + ")").position().top);
    //$("#playlist").scrollTop($("#playlist > div:contains(" + title + ")").position().top);
}

$(document).ready(function () {
    getVolume();
    populatePlaylist();

    //this controls the timed callback to the server. Only calls back when window is in focus.
    if (/*@cc_on!@*/false) { // check for Internet Explorer
        document.onfocusin = onFocus;
        document.onfocusout = onBlur;
    } else {
        window.onfocus = onFocus;
        window.onblur = onBlur;
    }
});



