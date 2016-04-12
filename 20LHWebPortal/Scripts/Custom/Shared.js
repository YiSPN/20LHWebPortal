(function (window) {
    window.SetupOnLoadEvents = function() {
        $(".datepicker").datepicker();
        $(".timepicker").timepicker();

        console.log(new Date() + " : Done setting up stuff.");
    };


    window.ShowNotification = function(message, type) {
        var notification = $("<div>");
        notification.attr("notification-type", type || "info");
        notification.html("<i class='fa fa-close'></i> <span>" + message + "</span>");
        notification.addClass("notification animated bounce");
        $("#notfications").append(notification);
    };
})(window);




$(document).ready(function() {
    window.SetupOnLoadEvents();
});