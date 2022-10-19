$(".comment-delete-btn").on("click", function () {
  showConfirmModal("Apagar o comentário?", "Se houver quaisquer arquivos associados ao comentário, eles também serão excluídos");
})

$(".comment-delete-btn, .btn-yes-comment, .btn-no-comment").on("click", function () {
  var id;
  if ($(this).hasClass("comment-delete-btn")) {
    id = $(this).parent().parent().parent().attr("data-comment-id");
    $(".btn-yes-comment").attr("data-comment-id", id);
  } else if ($(this).hasClass("btn-yes-comment")) {
    id = $(this).attr("data-comment-id");
    var comment = getCommentElement(id);
    deleteComment(id, comment);
  } else if ($(this).hasClass("btn-no-comment")) {
    hideConfirmModal();
  }
})

$(".comment-aprove-btn").on("click", function () {
  var id = $(this).parent().parent().parent().attr("data-comment-id");
  var comment = getCommentElement(id);
  aproveComment(id, comment);
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
        hideConfirmModal();
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

  //Adicionar a referência de um arquivo .js
  //if (!$("#teste").length) {
  //  function addScript(attribute, text, callback) {
  //    var s = document.createElement('script');
  //    for (var attr in attribute) {
  //      s.setAttribute(attr, attribute[attr] ? attribute[attr] : null)
  //    }
  //    s.innerHTML = text;
  //    s.onload = callback;
  //    document.body.appendChild(s);
  //  }

  //  addScript({
  //    id: 'teste',
  //    src: '../js/components/commentlist.js',
  //    type: 'text/javascript',
  //    async: null
  //  }, '<div>innerHTML</div>', function () { });
  //}
