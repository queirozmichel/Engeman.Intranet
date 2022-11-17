$(".back-to-list-button").on("click", function (event) {
  event.preventDefault();
  var postId = $(this).attr("data-post-id");
  $.ajax({
    type: "POST",
    url: "/posts/backtolist" + window.location.search,
    data: { "postId": postId },
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      if ($("#wang-editor-script").length) $("#wang-editor-script").remove();
    },
    error: function () {
      toastr.error("Não foi possível voltar", "Erro!");
    },
    complete: function () {
      closeSpinner();
    }
  });
})