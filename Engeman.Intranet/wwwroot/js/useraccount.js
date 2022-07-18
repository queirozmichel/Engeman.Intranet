$(window).on("load", function () {
  closeSpinner();
});

$("#edit-account-form").submit(function (event) {
  if (!$("#edit-account-form").valid()) {
    event.preventDefault();
  }
});