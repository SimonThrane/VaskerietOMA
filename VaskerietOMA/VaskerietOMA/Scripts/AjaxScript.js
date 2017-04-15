// Convert links (<a...>) to ajax requests
$("#homeLink").click(function (event) {
    event.preventDefault();
    var url = $(this).attr('href');
    $('#content').load(url);
});

$("#aboutLink").click(function (event) {
    event.preventDefault();
    var url = $(this).attr('href');
    $('#content').load(url);
});

$("#loginLink").click(function (event) {
    event.preventDefault();
    var url = $(this).attr('href');
    $('#content').load(url);
});

$("#contactLink").click(function (event) {
    event.preventDefault();
    var url = $(this).attr('href');
    $('#content').load(url);
});

$("#barLink").click(function (event) {
    event.preventDefault();
    var url = $(this).attr('href');
    $('#content').load(url);
});
