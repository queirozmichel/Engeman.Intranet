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
})

$("#edit-term-form").on("submit", function (event) {
  event.preventDefault();
  $("#edit-term-form").validate();
  if ($("#edit-term-form").valid()) {
    var editedTerm = $(this).serialize();
    $.ajax({
      type: "PUT",
      url: "/blacklistterms/updateterm",
      data: editedTerm,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.result == 200) {
          toastr.success("O termo foi atualizado.", "Sucesso!");
          $("#edit-term-modal").modal("hide");
          $("#blacklist-terms-grid").bootgrid("reload");
        } else if (response.result == 500) {
          $("#edit-term-modal").modal("hide");
          showAlertModal("Não foi possível editar o termo!", response.message);
        }
      },
      error: function (response) {
        toastr.error("Ocorreu um erro ao tentar processar a solicitação.", "Erro ");
        $("#edit-term-modal").modal("hide");
      },
      complete: function () {
        stopSpinner();
      }
    })
  }
})

$("#cancel-edit-term").on("click", function (event) {
  event.preventDefault();
  $("#edit-term-modal").modal("hide");
})

$('#edit-term-modal').on('shown.bs.modal', function () {
  $('#edit-term-form').find('#description').focus();
})