$(document).ready(function () {
})

$(".back-to-list-button").on("click", function () {
  $.ajax({
    type: "POST",
    url: "/posts/backtolist",
    success: function (response) {
      $(".body-content").empty();
      $(".body-content").html(response);
    },
    error: function () {
      toastr.error("Não foi possível voltar", "Erro!");
    }
  });
})