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
        maxlength: 30,
        pattern: "[A-Za-záàâãéèêíóôõúçñÁÀÂÃÉÈÍÓÔÕÚÇÑ ]+",
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
          showAlertModal("Atenção!", `O formulário contém ${response.occurrences} termo(s) de uso não permitido, é necessário removê-lo(s) para poder continuar.`);
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

$("#permissions").on("change", function (e) {
  if (this.value == 0) {
    $("#create-post-switch").bootstrapSwitch('state', true);
    $("#create-post-switch").bootstrapSwitch('readonly', false);
    $("#create-post-radio").prop("checked", true);
    $("#edit-owner-post-radio").prop("checked", true);
    $("#delete-owner-post-radio").prop("checked", true);
    $("#edit-any-post-radio").prop("checked", false);
    $("#delete-any-post-radio").prop("checked", false);
    $("#need-revision-radio").prop("checked", false);
  } else if (this.value == 1) {
    $("#create-post-switch").bootstrapSwitch('state', true);
    $("#create-post-switch").bootstrapSwitch('readonly', false);
    $("#create-post-radio").prop("checked", true);
    $("#edit-owner-post-radio").prop("checked", true);
    $("#delete-owner-post-radio").prop("checked", true);
    $("#edit-any-post-radio").prop("checked", false);
    $("#delete-any-post-radio").prop("checked", false);
    $("#need-revision-radio").prop("checked", true);
  } else if (this.value == 2) {
    $("#create-post-switch").bootstrapSwitch('state', true);
    $("#create-post-switch").bootstrapSwitch('readonly', true);
    $("#create-post-radio").prop("checked", true);
    $("#edit-owner-post-radio").prop("checked", true);
    $("#delete-owner-post-radio").prop("checked", true);
    $("#edit-any-post-radio").prop("checked", true);
    $("#delete-any-post-radio").prop("checked", true);
    $("#need-revision-radio").prop("checked", false);
  }
})

$("#username-input").on("input", function () {
  $("#email-input").val(this.value);
})

$(':radio').click(function (e) {
  e.preventDefault();
});

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

if ($("#permissions")[0].value == 2) {
  $("#create-post-switch").bootstrapSwitch('readonly', true);
}

$("#create-post-switch").on("switchChange.bootstrapSwitch", function (event, state) {
  if (state == true) {
    $("#create-post-radio").prop("checked", true);
  } else if (state == false) {
    $("#create-post-radio").prop("checked", false);
  }
});