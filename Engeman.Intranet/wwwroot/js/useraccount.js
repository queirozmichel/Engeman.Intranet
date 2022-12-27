$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {
  FormComponents.init();

  $("#edit-profile-form").validate({
    rules: {
      name: {
        required: true,
        maxlength: 19
      },
    },
    highlight: function (element) {
      $(element).parents('.form-group').addClass('has-error');
    },
    errorPlacement: function (error, element) {
      error.insertAfter(element);
    },
    ignore: '*:not([name])',
  })
})

$("#edit-profile-form").submit(function (event) {
  event.preventDefault();
  if ($("#edit-profile-form").valid()) {
    var formData = new FormData(this);
    $.ajax({
      type: "POST",
      contentType: false,
      processData: false,
      url: "/useraccount/updateuserprofile",
      dataType: "html",
      data: formData,
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response == 200) {
          $.ajax({
            type: "GET",
            dataType: "html",
            url: "/useraccount/userprofile",
            success: function (response) {
              $("#render-body").empty();
              $("#render-body").html(response);
              window.history.pushState(this.url, null, this.url);
            },
            error: function () {
              toastr.error("Não foi possível atualizar o perfil", "Erro!");
            }
          });
        };
        toastr.success("O perfil foi atualizado", "Sucesso!");
      },
      error: function () {
        toastr.error("Não foi possível atualizar o perfil", "Erro!");
      },
      complete: function () {
        stopSpinner();
      }
    });
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