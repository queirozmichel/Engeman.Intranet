$(window).on("load", function () {
  closeSpinner();
});

$("#edit-account-form").submit(function (event) {
  if (!$("#edit-account-form").valid()) {
    event.preventDefault();
  }
});

$("#photo").on("change", function () {
  if (this.files[0].size > 5000000) {
    let imageSize = ((this.files[0].size) / (1000 * 1000)).toFixed(1);
    $(this).siblings().find("span").text(imageSize);
    $("#photo").siblings("label").css("display", "block");
    this.value = "";
  } else {
    $("#photo").siblings("label").css("display", "none");
  }
})