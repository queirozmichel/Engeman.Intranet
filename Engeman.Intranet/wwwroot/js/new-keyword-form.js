$(function () {
  FormComponents.init();

  jQuery.validator.setDefaults({
    debug: true,
    ignoreTitle: true,
    rules: {
      "description": {
        required: true
      }
    },
    ignore: '*:not([name])',
  });
});


$("#new-keyword-form").on("submit", function (event) {
  event.preventDefault();
  $("#new-keyword-form").validate();
  if ($("#new-keyword-form").valid()) {
    var newKeyword = $(this).serialize();
    $.ajax({
      type: "POST",
      url: "/keywords/newkeyword",
      data: newKeyword,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.result == 200) {
          toastr.success("Nova palavra-chave salva.", "Sucesso!");
          $("#new-keyword-modal").modal("hide");
          $("#keywords-grid").bootgrid("reload");
        } else if (response.result == 500) {
          $("#new-keyword-modal").modal("hide");
          showAlertModal("Não foi possível salvar a palavra-chave!", response.message);
        }
      },
      error: function (response) {
        toastr.error("Ocorreu um erro ao tentar processar a solicitação.", "Erro ");
        $("#new-keyword-modal").modal("hide");
      },
      complete: function () {
        stopSpinner();
      }
    })
  }
})

$("#cancel-new-keyword").on("click", function (event) {
  event.preventDefault();
  $("#new-keyword-modal").modal("hide");
})

$('#new-keyword-modal').on('shown.bs.modal', function () {
  $('#new-keyword-form').find('#description').focus();
})