$(document).ready(function () {
  $('.tags').tagsInput({
    'height': 'auto',
    'width': '100%',
    'defaultText': '',
  });

  $("#tab_1_3").removeClass("active");
})

$("#answer-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#answer-form").valid()) {
    console.log("ok");
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