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
  showConfirmationModal("Apagar a postagem?", "Esta ação não poderá ser revertida.", "delete-post", sessionStorage.getItem("postId"));
})

$(".aprove-post-button").on("click", function () {
  showConfirmationModal("Aprovar a postagem?", "Esta ação não poderá ser revertida.", "aprove-post", sessionStorage.getItem("postId"));
})

$(".btn-yes, .btn-no").on("click", function () {
  if ($(this).attr("id") == "delete-post") {
    deletePost(sessionStorage.getItem("postId")).then((response) => {
      $.ajax({
        type: "GET",
        dataType: "html",
        url: "/posts/grid" + "?filter=" + sessionStorage.getItem("filterGrid"),
        beforeSend: function () {
          startSpinner();
        },
        success: function (response) {
          $("#render-body").empty();
          $("#render-body").html(response);
          window.history.pushState("/posts/grid?filter=" + sessionStorage.getItem("filterGrid"), null, "/posts/grid?filter=" + sessionStorage.getItem("filterGrid"));
          $.ajax({
            type: "GET",
            url: "/posts/unrevisedlist",
            dataType: "html",
            success: function (result) {
              $(".sub-menu > li.all-posts").remove();
              $(".sub-menu > li.unrevised-posts").remove();
              $(".sub-menu > li.unrevised-comments").remove();
              $(".aprove-post-button").remove();
              $("#list-posts-content").html(result);
            },
            error: function (result) {
              toastr.error("Não foi possível atualizar o menu de postagens", "Erro!");
            },
          })
        },
        error: function () {
          toastr.error("Não foi possível voltar", "Erro!");
        },
        complete: function () {
          stopSpinner();
        }
      });
    })
      .catch((error) => {
        console.log(error)
      })
  }
  else if ($(this).attr("id") == "aprove-post") {
    aprovePost(sessionStorage.getItem("postId"));
  }
  else if ($(this).attr("id") == "aprove-comment") {
    var id = $(this).attr("data-id");
    var comment = getCommentElement(id);
    aproveComment(id, comment);
    hideConfirmationModal();
  }
  else if ($(this).attr("id") == "delete-comment") {
    var id = $(this).attr("data-id");
    var comment = getCommentElement(id);
    deleteComment(id, comment);
    hideConfirmationModal();
  }
  else if ($(this).hasClass("btn-no")) {
    hideConfirmationModal();
  }
})

function deletePost(postId) {
  return new Promise((resolve, reject) => {
    $.ajax({
      type: "DELETE",
      data: {
        'postId': postId
      },
      url: "/posts/deletepost",
      dataType: "html",
      success: function (response) {
        hideConfirmationModal();
        toastr.success("A postagem foi apagada", "Sucesso!");
        setTimeout(() => {
          resolve(response)
        }, 350);
      },
      error: function (error) {
        reject(error)
      },
    })
  })
}

function aprovePost(postId) {
  $.ajax({
    type: "PUT",
    data: {
      'postId': postId
    },
    url: "/posts/aprovepost",
    dataType: "text",
    success: function (result) {
      $(".sub-menu > li.all-posts").remove();
      $(".sub-menu > li.unrevised-posts").remove();
      $(".sub-menu > li.unrevised-comments").remove();
      $(".aprove-post-button").remove();
      $(".status-post").remove();
      $("#list-posts-content").html(result);
      hideConfirmationModal();
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
