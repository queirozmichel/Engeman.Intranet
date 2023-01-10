$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  FormComponents.init();

  $("#edit-profile-form").validate({
    rules: {
      "name": {
        required: true,
        maxlength: 30
      },
      "photo": {
        accept: "jpg,jpeg",
        filesize: 5,
      }
    },
    messages: {
      "photo": {
        accept: "Por favor, forneça uma imagem válida.( jpg/jpeg )",
      }
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

jQuery.validator.addMethod('filesize', function (value, element, param) {
  return this.optional(element) || (element.files[0].size <= param * 1000000)
}, function (value, element) {
  let imageSize = ((element.files[0].size) / (1000 * 1000)).toFixed(1);
  return `A imagem tem ${imageSize}MB e excede o tamanho máximo de ${value}MB`;
});

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