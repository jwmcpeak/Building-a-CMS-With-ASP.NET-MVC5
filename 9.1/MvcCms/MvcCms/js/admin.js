$("a[data-action=delete]").click(function (e) {
    e.preventDefault();

    var value = this.getAttribute("data-value");

    if (!confirm("Are you sure you want to delete " + value + "?")) {
        return;
    }

    var token = $("input[name=__RequestVerificationToken]").val();

    $.post(this.href, {
        __RequestVerificationToken: token
    }, function () {
        location.reload();
    });
});