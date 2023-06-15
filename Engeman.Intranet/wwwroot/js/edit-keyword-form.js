$(function () {
  FormComponents.init();

  jQuery.validator.setDefaults({
    debug: true,
    ignoreTitle: true,
    rules: {
      "description": {
        required: true,
        pattern: "[A-Za-z0-9]+",
      }
    },
    messages: {
      "description": {
        pattern: "Não é permitido o uso de caracteres especiais ou espaços em branco.",
      }
    },
    ignore: '*:not([name])',
  });
})

$("#edit-keyword-form").on("submit", function (event) {
  event.preventDefault();
  $("#edit-keyword-form").validate();
  if ($("#edit-keyword-form").valid()) {
    var editedKeyword = $(this).serialize();
    $.ajax({
      type: "PUT",
      url: "/keywords/updatekeyword",
      data: editedKeyword,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.result == 200) {
          toastr.success("A palvra-chave foi atualizada.", "Sucesso!");
          $("#edit-keyword-modal").modal("hide");
          $("#keywords-grid").bootgrid("reload");
        } else if (response.result == 500) {
          $("#edit-keyword-modal").modal("hide");
          showAlertModal("Não foi possível editar a palavra-chave!", response.message);
        }
      },
      error: function (response) {
        toastr.error("Ocorreu um erro ao tentar processar a solicitação.", "Erro ");
        $("#edit-keyword-modal").modal("hide");
      },
      complete: function () {
        stopSpinner();
      }
    })
  }
})

$("#cancel-edit-keyword").on("click", function (event) {
  event.preventDefault();
  $("#edit-keyword-modal").modal("hide");
})

$('#edit-keyword-modal').on('shown.bs.modal', function () {
  $('#edit-keyword-form').find('#description').focus();
})