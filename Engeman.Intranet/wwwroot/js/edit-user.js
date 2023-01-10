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

$(".permissions-switch").bootstrapSwitch('readonly', true);

$("#edit-user-form").on("submit", function (event) {
  event.preventDefault();
  if ($("#edit-user-form").valid()) {
    var editedUser = new FormData(this);
    $.ajax({
      type: "POST",
      url: "/useraccount/updatebymoderator",
      data: editedUser,
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
        } else {
          toastr.error("Código de resposta não tratado", "Erro!");
        }
      },
      error: function () {
        toastr.error("A requisição não foi enviada", "Erro!");
      },
      complete: function () {
        stopSpinner();
      }
    })
  }
})

$("#permissions").on("change", function (e) {
  $(".permissions-switch").bootstrapSwitch('readonly', false);
  if (this.value == 0) {
    $("#create-post-radio").prop("checked", true);
    $("#edit-owner-post-radio").prop("checked", true);
    $("#delete-owner-post-radio").prop("checked", true);
    $("#edit-any-post-radio").prop("checked", false);
    $("#delete-any-post-radio").prop("checked", false);
    $("#need-revision-radio").prop("checked", false);
  } else if (this.value == 1) {
    $("#create-post-radio").prop("checked", true);
    $("#edit-owner-post-radio").prop("checked", true);
    $("#delete-owner-post-radio").prop("checked", true);
    $("#edit-any-post-radio").prop("checked", false);
    $("#delete-any-post-radio").prop("checked", false);
    $("#need-revision-radio").prop("checked", true);
  } else if (this.value == 2) {
    $("#create-post-radio").prop("checked", true);
    $("#edit-owner-post-radio").prop("checked", true);
    $("#delete-owner-post-radio").prop("checked", true);
    $("#edit-any-post-radio").prop("checked", true);
    $("#delete-any-post-radio").prop("checked", true);
    $("#need-revision-radio").prop("checked", false);
  }
  $(".permissions-switch").bootstrapSwitch('readonly', true);
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