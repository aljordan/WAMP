/*jslint browser: true*/
/*global $, jQuery*/

var breadCrumbArray = []; // for user's trail of functions
var currentSongTitle = ""; // used to ee if the song has changed
var currentSongTotalTime = 0;
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

function togglePlay() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
        }
    };
    xhttp.open("GET", "http://localhost:9090/wamp/toggleplay", true);
    xhttp.send();
}



// Once this method starts, it doesn't stop
function getCurrentSongInfo() {
    $.ajax({
        url: 'http://localhost:9090/wamp/songinfo',
        success: function (data) {
            if (data.GetCurrentSongInfoResult !== "") {
                var info = $.parseJSON(data.GetCurrentSongInfoResult);

                var progress = document.getElementById("songProgressBar");
                var value = 0;
                if (info.SongLength > 0) {
                    value = Math.floor((100 / info.SongLength) * info.SongProgress);
                }
                progress.style.width = value + "%";
                currentSongTotalTime = info.SongLength;

                if (info.SongTitle !== currentSongTitle) {
                    currentSongTitle = info.SongTitle;
                    scrollToInPlaylist(info.SongTitle);
                }

                //document.getElementById("songName").innerHTML = info.SongTitle;
                document.getElementById("songName").innerHTML = '<a class="medium" onclick="scrollToInPlaylist(\'' + escapeQuotes(info.SongTitle) + '\')" href="#">' + info.SongTitle + '</a>';
                document.getElementById("artistName").innerHTML = '<a class="medium" onclick="getAlbumsByArtist(\'' + escapeQuotes(info.Artist) + '\')" href="#">' + info.Artist + '</a>';
                document.getElementById("albumName").innerHTML = '<a class="medium" onclick="getSongsByAlbum(\'' + escapeQuotes(info.Album) + '\')" href="#">' + info.Album + '</a>';


                if ($("#imgPlayPause").attr("src").indexOf("Play.png") >= 0 && !info.IsPaused) {
                    $("#imgPlayPause").attr("src", "http://localhost:9090/wamp/getimage/Pause.png");
                }
                else if ($("#imgPlayPause").attr("src").indexOf("Pause.png") >= 0 && info.IsPaused) {
                    $("#imgPlayPause").attr("src", "http://localhost:9090/wamp/getimage/Play.png");
                }
            }
            else {
                if ($("#imgPlayPause").attr("src").indexOf("Pause.png") >= 0) {
                    $("#imgPlayPause").attr("src", "http://localhost:9090/wamp/getimage/Play.png");
                }
            }
        },
        complete: function () {
            // Schedule the next request when the current one's complete
            timeoutHandle =  setTimeout(getCurrentSongInfo, 1000);
        }
    });
}


// Add click event to the song progress bar.
// Will move the current song to the time clicked.
$('.clickable').bind('click', function (ev) {
    var $div = $('.clickable');
    var totalWidth = $div.width();
    var offset = $div.offset();
    var clickedWidth = ev.clientX - offset.left;
    var percentage = clickedWidth / totalWidth;

    var url = "http://localhost:9090/wamp/movesongto/" + percentage;
    var xhttp = new XMLHttpRequest();
    //xhttp.onreadystatechange = function () {
    //    if (xhttp.readyState === 4 && xhttp.status === 200) {
    //    }
    //};
    xhttp.open("GET", url, true);
    xhttp.send();
});


// Add mouse over event to the song progress bar.
// Displays the time the song will move to if clicked.
$('.clickable').bind('mousemove', function (ev) {
    var $div = $('.clickable');
    var totalWidth = $div.width();
    var offset = $div.offset();
    var clickedWidth = ev.clientX - offset.left;
    var percentage = clickedWidth / totalWidth;
    var time = percentage * currentSongTotalTime;
    var minutes = Math.floor(time / 60);
    var seconds = Math.floor(time - minutes * 60);
    if (seconds < 10)
        seconds = '0' + seconds;
    var timeLabel = minutes + ':' + seconds;
    $('.clickable').prop('title', timeLabel);
});


function onBlur() {
    clearTimeout(timeoutHandle);
}

function onFocus() {
    getCurrentSongInfo();
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

function getRepeat() {
    var url = "http://localhost:9090/wamp/getrepeat/";
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            if (Data.GetRepeatResult === 'False')
                $("#imgRepeat").attr("src", "http://localhost:9090/wamp/getimage/RepeatOff.png");
            else
                $("#imgRepeat").attr("src", "http://localhost:9090/wamp/getimage/Repeat.png");
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

function toggleRepeat() {
    var url = "http://localhost:9090/wamp/getrepeat/";
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            if (Data.GetRepeatResult === 'False')
                setRepeat('true');
            else
                setRepeat('false');
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}


function setRepeat(repeat) {
    var url = "http://localhost:9090/wamp/setrepeat/" + repeat;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            getRepeat();
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}


function getRandom() {
    var url = "http://localhost:9090/wamp/getrandom/";
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            if (Data.GetRandomResult === 'False')
                $("#imgRandom").attr("src", "http://localhost:9090/wamp/getimage/ShuffleOff.png");
            else
                $("#imgRandom").attr("src", "http://localhost:9090/wamp/getimage/Shuffle.png");
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

function toggleRandom() {
    var url = "http://localhost:9090/wamp/getrandom/";
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            var Data = JSON.parse(xhttp.responseText);
            if (Data.GetRandomResult === 'False')
                setRandom('true');
            else
                setRandom('false');
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}


function setRandom(random) {
    var url = "http://localhost:9090/wamp/setrandom/" + random;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (xhttp.readyState === 4 && xhttp.status === 200) {
            getRandom();
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
    // reset any currently hilighted song name to color white
    $("#playlist > div > span > .large").css("color", "white");
    //find and hilite current playing song title
    $("#playlist > div > span:contains(" + title + ") > .large").css("color", "#ffcc00");

    // scroll to (doesn't work as it scrolls to wrong place all the time)
    $("#playlist").scrollTop(0);
    var scrollpoint = $("#playlist > div > span:contains(" + title + ") > .large").offset().top - ($(".player").height() + 30 );
    $("#playlist").scrollTop(scrollpoint);
}

$(document).ready(function () {
    getVolume();
    populatePlaylist();
    getRepeat();
    getRandom();

    //this controls the timed callback to the server. Only calls back when window is in focus.
    if (/*@cc_on!@*/false) { // check for Internet Explorer
        document.onfocusin = onFocus;
        document.onfocusout = onBlur;
    } else {
        window.onfocus = onFocus;
        window.onblur = onBlur;
    }
});



