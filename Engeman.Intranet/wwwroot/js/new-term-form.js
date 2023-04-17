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


$("#new-term-form").on("submit", function (event) {
  event.preventDefault();
  $("#new-term-form").validate();
  if ($("#new-term-form").valid()) {
    var newTerm = $(this).serialize();
    $.ajax({
      type: "POST",
      url: "/blacklistterms/newterm",
      data: newTerm,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.result == 200) {
          toastr.success("Novo termo salvo.", "Sucesso!");
          $("#new-term-modal").modal("hide");
          $("#blacklist-terms-grid").bootgrid("reload");
        } else if (response.result == 500) {
          $("#new-term-modal").modal("hide");
          showAlertModal("Não foi possível salvar o termo!", response.message);
        }
      },
      error: function (response) {
        toastr.error("Ocorreu um erro ao tentar processar a solicitação.", "Erro ");
        $("#new-term-modal").modal("hide");
      },
      complete: function () {
        stopSpinner();
      }
    })
  }
})

$("#cancel-new-term").on("click", function (event) {
  event.preventDefault();
  $("#new-term-modal").modal("hide");
})

$('#new-term-modal').on('shown.bs.modal', function () {
  $('#new-term-form').find('#description').focus();
})