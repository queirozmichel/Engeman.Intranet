$("#all-posts-link, #unrevised-posts-link, #unrevised-comments-link").on("click", function (event) {
  var url = "";
  if (this.id == "all-posts-link") {
    url = "/posts/grid?filter=allPosts"
  } else if (this.id == "unrevised-posts-link") {
    url = "/posts/grid?filter=unrevisedPosts";
  } else {
    url = "/posts/grid?filter=unrevisedComments";
  }

  $.ajax({
    type: "GET",
    url: url,
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function () {
      toastr.error("Não foi possível acessar a lista", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
})