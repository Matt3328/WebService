function addOrRemoveLike(idFilm, addOrRemove) {
    idFilm = parseInt(idFilm);
    $.ajax({
        url: '/Home/AddOrRemoveLike',
        type: "POST",
        data: { id: idFilm, addOrRemove : addOrRemove },
    }).done(function (result) {
        var div = document.getElementById(idFilm);
        var span = div.getElementsByTagName('span');
        var i = div.getElementsByTagName('i');
        if (addOrRemove == "add") {
            i[0].className = 'bi bi-heart-fill';
            i[0].setAttribute('onclick', 'addOrRemoveLike(' + idFilm + ', "remove")');
            span[0].innerHTML = result.toString();
        }
        else if (addOrRemove == "remove") {
            i[0].className = 'bi bi-heart';
            i[0].setAttribute('onclick', 'addOrRemoveLike(' + idFilm + ', "add")');
            span[0].innerHTML = result.toString();
        }      
    });
}
