$(window).on("load", function () {

  stopSpinner();

})

$(document).ready(function () {

  FormComponents.init();

  $("#permissions").tooltip({
    position: { my: "left+15 center", at: "right center" },
    content: function () {
      return $(this).attr('title');
    }
  });

  jQuery.validator.setDefaults({
    debug: true,
    rules: {
      "name": {
        required: true,
        maxlength: 19,
      },
      "username": {
        required: true,
        pattern: "[a-z]+[.][a-z]+",
      },
      "photo": {
        accept: "jpg,jpeg,png",
        filesize: 5,
      }
    },
    messages: {
      "username": {
        pattern: "O formato fornecido é inválido. Ex: joao.alencar",
      },
      "photo": {
        accept: "Por favor, forneça uma imagem válida.( jpg/jpeg/png )",
      }
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

$(".switch").bootstrapSwitch({
  onText: "sim",
  offText: "não",
  size: "small",
});

$("#edit-user-form").on("submit", function (event) {
  event.preventDefault();
  if ($("#edit-user-form").valid()) {
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
            url: "/useraccount/updateuseraccount",
            data: formData,
            contentType: false,
            processData: false,
            dataType: "html",
            beforeSend: function () {
              startSpinner();
            },
            success: function (response) {
              if (response == 200) {
                toastr.success("As alterações foram salvas", "Sucesso!");
                window.history.back();
              }
            },
            error: function () {
              if (response.status == 500) {
                toastr.error(response.responseText, "Erro " + response.status);
              }
            },
            complete: function () {
              stopSpinner();
            }
          })
        }
      },
      error: function (response) { },
      complete: function (response) {
        stopSpinner();
      }
    })
  }
})

$("#username-input").on("input", function () {
  $("#email-input").val(this.value);
})

$(".back-button").on("click", function (event) {
  previousPage();
})

$("#log-tab").on("click", function () {
  let params = new URLSearchParams(document.location.search);
  let userId = params.get("userId");

  $.ajax({
    type: "GET",
    dataType: "html",
    url: "/logs/grid",
    data: { "filterByUsername": true, "userId": userId },
    beforeSend: function () {
      startSpinner();
    },
    success: function (reponse) {
      $("#tab-2").empty();
      $("#tab-2").html(reponse);
    },
    error: function () {
      toastr.error("Não foi possível concluir a requisição", "Erro");
    },
    complete: function () {
      stopSpinner();
    }
  })
})

$(".template").on("click", function (e) {
  e.preventDefault();
  if ($(this).data('template') == "moderator") {
    $(".can-post").prop("checked", true);
    $(".can-comment").prop("checked", true);
    $(".edit-any-post").prop("checked", true);
    $(".delete-any-post").prop("checked", true);
    $(".requires-moderation").prop("checked", false);
  }
  else if ($(this).data('template') == "default") {
    $(".can-post").prop("checked", true);
    $(".can-comment").prop("checked", true);
    $(".edit-any-post").prop("checked", false);
    $(".delete-any-post").prop("checked", false);
    $(".requires-moderation").prop("checked", false);
  }
  else if ($(this).data('template') == "novice") {
    $(".can-post").prop("checked", true);
    $(".can-comment").prop("checked", true);
    $(".edit-any-post").prop("checked", false);
    $(".delete-any-post").prop("checked", false);
    $(".requires-moderation").prop("checked", true);
  }
});

