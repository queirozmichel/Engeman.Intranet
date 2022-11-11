$(".comment-aprove-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal("Aprovar o comentário?", "Esta ação não poderá ser revertida.", "aprove", id);
})

$(".comment-delete-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal("Apagar o comentário?", "Se houver quaisquer arquivos associados ao comentário, eles também serão excluídos.", "delete", id);
})

$(".btn-yes, .btn-no").on("click", function (event) {
  if ($(this).attr("id") == "aprove") {
    var id = $(this).attr("data-id");
    var comment = getCommentElement(id);
    aproveComment(id, comment);
    hideConfirmationModal();
  } else if ($(this).attr("id") == "delete") {
    var id = $(this).attr("data-id");
    var comment = getCommentElement(id);
    deleteComment(id, comment);
    hideConfirmationModal();
  } else if ($(this).hasClass("btn-no")) {
    hideConfirmationModal();
  }
})

function getCommentElement(id) {
  var aux;
  var comments = $(".comments").find("div.comment-box");
  comments.each(function (index, element) {
    if ($(element).attr("data-comment-id") == id) {
      aux = element;
    }
  })
  return $(aux);
}

function aproveComment(id, comment) {
  $.ajax({
    type: "PUT",
    data: { "idComment": id },
    url: "aprovecomment",
    dataType: "text",
    success: function (response) {
      $(comment).css("background", "white");
      $(comment).find(".comment-aprove-btn").remove();
      $(comment).find("#awaiting-review").remove();
      $(".sub-menu > li.all-posts").remove();
      $(".sub-menu > li.unrevised-posts").remove();
      $(".sub-menu > li.unrevised-comments").remove();
      $("#list-posts-content").html(response);
      toastr.success("Comentário aprovado", "Sucesso!");
    },
    error: function (response) {
      if (response == "false") {
        toastr.error("Não foi possível aprovar o comentário", "Erro!");
      }
    }
  })
}

function deleteComment(id, comment) {
  $.ajax({
    type: "DELETE",
    data: {
      "idComment": id
    },
    url: "deletecomment",
    dataType: "text",
    success: function (response) {
      if (response == "true") {
        comment.fadeOut(700);
        setTimeout(() => {
          comment.remove();
        }, 700)
        toastr.success("O comentário foi apagado com sucesso", "Sucesso!");
        $("#comment-count").text($("#comment-count").text() - 1);
      }
    },
    error: function (response) {
      toastr.error("Não foi possível apagar o comentário", "Erro!");
    }
  })
  comment.fadeOut(700);
  setTimeout(() => {
    comment.remove();
  }, 700)
}

$("pre").addClass("line-numbers");

$(".comment-edit-btn").on("click", function () {
  $(".wang-editor").remove();
  $(".comment-action-buttons").css("display", "none");
  var comment = $(this).parents(".comment-box");
  var id = $(this).parents(".comment-box").data("comment-id");
  var commentDescription = $(comment).children("#editor-content-view").html();
  $.ajax({
    type: "GET",
    dataType: "html",
    data: { "commentId": id },
    url: "/comments/commenteditform",
    success: function (response) {
      $("#wang-editor-script").remove();
      $(comment).css("background", "white");
      $(comment).html(response);
    },
    error: function (response) {
      console.log("error");
    }
  })
})