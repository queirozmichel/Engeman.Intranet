$(document).ready(function () { 
  $('.tags').tagsInput({
    'height': 'auto',
    'width': '100%',
    'defaultText': '',
  });

  $("#tab_1_3").removeClass("active");
})

$("#comment-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#comment-form").valid()) {
    //usado para receber além dos dados texto, o arquivo também
    var formData = new FormData(this);
    formData.append("postId", sessionStorage.getItem("postId"));
    //contentType e processData são obrigatórios
    $.ajax({
      type: "POST",
      url: "MakeComment",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == -1) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("O comentário foi salvo", "Sucesso!");
          $.ajax({
            type: "POST",
            url: "BackToList",
            success: function (response) {
              $("#question-details").empty();
              $("#question-details").html(response);
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            }
          });
        }
      },
      error: function (response) {
        toastr.error("O comentário não foi salvo", "Erro!");
      }
    })
  }
})

$(".back-to-list-button").on("click", function () {
  $.ajax({
    type: "POST",
    url: "BackToList",
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    },
    error: function () {
      toastr.error("Não foi possível voltar", "Erro!");
    }
  });
})

$(".fa-trash-o").on("click", function () {
  console.log("ok");
})