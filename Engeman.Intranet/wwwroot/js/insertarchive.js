$(document).ready(function () {
})



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
      console.log(response);
      toastr.success("O arquivo foi salvo", "Sucesso!");
    },
    error: function (response) {
      console.log(response);
      toastr.error("O arquivo não foi salvo", "Erro!");
    }
  });
})