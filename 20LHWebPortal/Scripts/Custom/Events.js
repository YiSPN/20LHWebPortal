(function (window) {
    window.Events = window.Events || {};
    window.Events.Init = window.Events.Init || {};
    window.Events.EditEvent = window.Events.EditEvent || {};
    window.Events.SaveEvent = window.Events.SaveEvent || {};

    window.Events.EditEvent = function (eventId) {
        var editEventModal = $("#edit-event-modal");
        var contentArea = $("#edit-event-modal-content");
        var editEventUrl = editEventModal.attr("data-url");
        var token = $("[name='__RequestVerificationToken']").val();

        //reset modal content
        contentArea.html("");
        editEventModal.attr("data-event-id", eventId);

        $.ajax({
            type: "GET",
            url: editEventUrl,
            data: {
                id: eventId,
                __RequestVerificationToken: token
            }
        })
        .done(function (response) {
            contentArea.html(response);
        });
    }

    window.Events.SaveEvent = function () {
        var editEventModal = $("#edit-event-modal");
        var form = editEventModal.find("form");

        $.ajax({
            type: "POST",
            url: form.attr("action"),
            data: form.serialize()
        }).done(function (data) {
            if (data.Errors && data.Errors.length > 0) {
                for (var i = 0; i < data.Errors.length; i++) {
                    window.ShowNotification(data.Errors[i], "error");
                }
            }
            else {
                var eventId = editEventModal.attr("data-event-id");
                var eventCard = $(".event.upcoming[data-event-id='" + eventId + "']");
                eventCard.replaceWith(data.View);

                editEventModal.modal('toggle');
            }
            //Enable this for debugging
            //console.log(data);
        });
    };

})(window);



$(document).on("click", "[data-button-type='edit-event']", function () {
    var eventId = $(this).attr("data-event-id");
    window.Events.EditEvent(eventId);
});

$(document).on("click", "[data-button-type='edit-event-save']", function () {
    window.Events.SaveEvent();
});

$(document).on("click", ".notification", function () {
    $(this).fadeOut(function () {
        $(this).remove();
    });
});