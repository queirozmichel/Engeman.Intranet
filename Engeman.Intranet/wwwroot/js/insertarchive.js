$(document).ready(function () {

})

function clearForm() {
  $(".archive-type").prop('checked', false);
  $("#subject").val('');
  $("#description").val('');
  $("#file").val('');
  $(".tag").remove();
  $(".form-group").removeClass("has-success")
}

$("#archive-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  //usado para receber além dos dados texto, o arquivo também
  if ($("#archive-form").valid()) {
    var formData = new FormData(this);
    //contentType e processData são obrigatórios
    $.ajax({
      type: "POST",
      url: "insertarchive",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("O arquivo foi salvo", "Sucesso!");
          clearForm();
        }
      },
      error: function (response) {
        toastr.error("O arquivo não foi salvo", "Erro!");
      }
    });
  }
})