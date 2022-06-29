$(document).ready(function () {
  clearForm();

})

function clearForm() {
  $(".archive-type").prop('checked', false);
  $("#subject").val('');
  $("#description").val('');
  $("#file").val('');
  $(".tag").remove();
}

$("#archive-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  //usado para receber além dos dados texto, o arquivo também
  var formData = new FormData(this);
  //contentType e processData são obrigatórios
  $.ajax({
    type: "POST",
    url: "insertarchive",
    contentType: false,
    processData: false,
    data: formData,
    success: function (response) {
      if (response == true) {
        toastr.success("O arquivo foi salvo", "Sucesso!");
        clearForm();
      } else {
        toastr.error("O arquivo não foi salvo", "Erro!");
      }
    },
    error: function (response) {
      toastr.error("O arquivo não foi salvo", "Erro!");
    }
  });
})