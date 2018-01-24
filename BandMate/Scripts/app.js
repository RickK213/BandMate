//This code will be loaded on all pages

$(document).ready(function () {

    //Add 'active' state to links for current url
    $(function () {
        let controllerName = '/' + location.pathname.split("/")[1];
        let actionName = "/" + location.pathname.split("/")[2];
        let parameters = location.search;
        let fullPathname = controllerName;
        if (actionName !== '/undefined') {
            fullPathname += actionName;
        }
        if (parameters !== 'undefined') {
            fullPathname += parameters;
        }
        $('li a[href="' + fullPathname + '"]').parent('li').addClass('active');
        $('a[href="' + fullPathname + '"]').addClass('active');
    });

    //Show the infoMessage
    if ($("#infoAlertMessage").text().length > 0) {
        $("#infoAlert").fadeIn();
    }

    //Show the dangerMessage
    if ($("#dangerAlertMessage").text().length > 0) {
        $("#dangerAlert").fadeIn();
    }


});