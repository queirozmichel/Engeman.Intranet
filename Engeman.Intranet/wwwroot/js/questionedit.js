$(document).ready(function () { 
  FormComponents.init(); // Init all form-specific plugins
})

$("#edit-question-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  var formData = $("#edit-question-form").serializeArray();
  $.ajax({
    type: "POST",
    dataType: 'text',
    async: true,
    url: "UpdateQuestion",
    data: formData,
    success: function (response) {
      toastr.success("A pergunta foi atualizada", "Sucesso!");
      $("#question-details").empty();
      $("#question-details").html(response);
    },
    error: function () {
      toastr.error("A pergunta não foi atualizada", "Erro!");
    }
  });
})