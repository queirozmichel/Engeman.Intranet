$(document).ready(function () {
  FormComponents.init(); // Init all form-specific plugins
})

$("#edit-question-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#edit-question-form").valid()) {
    var formData = $("#edit-question-form").serializeArray();
    $.ajax({
      type: "POST",
      dataType: 'text',
      async: true,
      url: "UpdateQuestion",
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A pergunta foi atualizada", "Sucesso!");
          $("#question-details").empty();
          $("#question-details").html(response);
        }
      },
      error: function () {
        toastr.error("A pergunta não foi atualizada", "Erro!");
      }
    });
  }
})

$("#back-to-list-button").on("click", function () {
  $.ajax({
    type: "POST",
    url: "BackToList",
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    },
    error: function () {
      toastr.error("A pergunta não foi atualizada", "Erro!");
    }
  });
})