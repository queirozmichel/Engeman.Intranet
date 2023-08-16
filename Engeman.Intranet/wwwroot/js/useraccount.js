$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  FormComponents.init();

  $("#edit-profile-form").validate({
    rules: {
      "name": {
        required: true,
        maxlength: 19,
      },
      "photo": {
        accept: "jpg,jpeg",
        filesize: 5,
      },
      "description": {
        required: false,
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
      url: "/blacklistterms/blacklisttest",
      contentType: false,
      processData: false,
      data: formData,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.occurrences != 0) {
          showAlertModal("Atenção!", `O formulário contém o(s) termo(s) ${response.termsFounded}, de uso não permitido. É necessário removê-lo(s) para poder continuar.`);
        } else {
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
                  url: "/useraccount/edituserprofile",
                  success: function (response) {
                    $("#render-body").empty();
                    $("#render-body").html(response);
                    window.history.pushState(this.url, null, this.url);
                  },
                  error: function (response) {
                    toastr.error("Não foi possível atualizar a página.", "Erro!");
                  }
                });
              };
              toastr.success("As alterações foram salvas.", "Sucesso!");
            },
            error: function (response) {
              if (response.status == 500) {
                toastr.error(response.responseText, "Erro " + response.status);
              }
            },
            complete: function () {
              stopSpinner();
            }
          });
        }
      },
      error: function (response) { },
      complete: function (response) {
        stopSpinner();
      }
    })
  }
});

$(':radio').click(function (e) {
  e.preventDefault();
});