$(document).ready(function () {
  FormComponents.init(); // Init all form-specific plugins
})

$("#archive-post-edit-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#archive-post-edit-form").valid()) {
    var formData = new FormData(this);
    $.ajax({
      type: "POST",
      url: "UpdateArchive",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          $("#question-details").empty();
          $("#question-details").html(response);
          toastr.success("O arquivo foi salvo", "Sucesso!");
        }
      },
      error: function (response) {
        toastr.error("O arquivo não foi salvo", "Erro!");
      }
    });
  }
})