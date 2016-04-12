(function (window) {
    window.SetupOnLoadEvents = function() {
        $(".datepicker").datepicker();
        $(".timepicker").timepicker();

        console.log(new Date() + " : Done setting up stuff.");
    };
})(window);




$(document).ready(function() {
    window.SetupOnLoadEvents();
});