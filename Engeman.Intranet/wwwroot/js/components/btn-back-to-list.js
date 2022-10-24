$(".back-to-list-button").on("click", function (event) {
  event.preventDefault();
  var postId = $(this).attr("data-post-id");
  $.ajax({
    type: "POST",
    url: "/posts/backtolist" + window.location.search,
    data: { "postId": postId },
    success: function (response) {
      $(".body-content").empty();
      $(".body-content").html(response);
      if ($("#wang-editor-script").length) $("#wang-editor-script").remove();
    },
    error: function () {
      toastr.error("Não foi possível voltar", "Erro!");
    }
  });
})