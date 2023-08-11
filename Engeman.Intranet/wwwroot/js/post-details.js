$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {
  FormComponents.init();
  sessionStorage.setItem("postId", $("#post-id").text());

  $("#tab_1_3").removeClass("active");

  $("#comment-tab-disabled").css("pointer-events", "none");
  $("#comment-tab-disabled").css("color", "#55555555");
})

$(".edit-post-button").on("click", function () {
  sessionStorage.setItem("editAfterDetails", true);
  $.ajax({
    type: "GET",
    data: { "postId": sessionStorage.getItem("postId") },
    dataType: "html",
    url: "/posts/editpost",
    beforeSend: function () {
      startSpinner();
    },
    error: function () {
      toastr.error("Não foi possível editar a postagem", "Erro!");
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    complete: function () {
      stopSpinner();
    }
  })
})

$(".delete-post-button").on("click", function () {
  showConfirmationModal(deletePost, { postId: sessionStorage.getItem("postId"), redirectTo: "postGrid" });
})

$(".aprove-post-button").on("click", function () {
  showConfirmationModal(aprovePost, { postId: sessionStorage.getItem("postId") });
})

function aprovePost(args) {

  const { postId } = args;

  $.ajax({
    type: "PUT",
    data: {
      'postId': postId
    },
    url: "/posts/aprovepost",
    dataType: "text",
    success: function (result) {
      $(".aprove-post-button").remove();
      $(".status-post").remove();
      toastr.success("Postagem aprovada", "Sucesso!");
    },
    error: function (result) {
      toastr.error("Não foi possível aprovar a postagem", "Erro!");
    },
    complete: function () {
      $("#posts-grid").bootgrid("reload");
    }
  })
}

$(".back-button").on("click", function (event) {
  previousPage();
})

$("pre").addClass("line-numbers");
