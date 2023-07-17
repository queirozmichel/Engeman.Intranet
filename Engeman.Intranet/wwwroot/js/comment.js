$(document).ready(function () {

  jQuery.validator.setDefaults({
    rules: {
      "description": {
        required: true,
        normalizer: function (value) {
          return RemoveHTMLTags(value);
        }
      }
    },
    messages: {
      "files": {
        accept: "Por favor, forneça arquivo(s) com a extensão .pdf",
      }
    },
    ignore: '*:not([name])',
  });

})

$("#comment-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  $("#comment-form").validate();
  if ($("#comment-form").valid()) {
    //usado para receber além dos dados texto, o arquivo também
    var formData = new FormData(this);
    formData.append("postId", sessionStorage.getItem("postId"));
    //contentType e processData são obrigatórios
    $.ajax({
      type: "POST",
      url: "/blacklistterms/blacklisttest",
      contentType: false,
      processData: false,
      data: formData,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.occurrences != 0) {
          showAlertModal("Atenção!", `O formulário contém o(s) termo(s) ${response.termsFounded}, de uso não permitido. É necessário removê-lo(s) para poder continuar.`);
        } else {
          $.ajax({
            type: "POST",
            url: "/comments/newcomment",
            dataType: "html",
            contentType: false,
            processData: false,
            data: formData,
            beforeSend: function () {
              startSpinner();
            },
            success: function (response) {
              if (response == 200) {
                $.ajax({
                  type: "GET",
                  dataType: "html",
                  data: { "postId": sessionStorage.getItem("postId") },
                  url: "/posts/postdetails",
                  success: function (response) {
                    $("#render-body").empty();
                    $("#render-body").html(response);
                  },
                  error: function () {
                    toastr.error("Não foi possível ir para os detalhes da postagem", "Erro!");
                  },
                });
                toastr.success("O comentário foi salvo", "Sucesso!");
              }
            },
            error: function (response) {
              if (response.status == 500) {
                toastr.error(response.responseText, "Erro " + response.status);
              }
            },
            complete: function () {
              stopSpinner();
            },
          })
        }
      },
      error: function (response) { },
      complete: function (response) {
        stopSpinner();
      }
    })
  }
})

$(".comment-aprove-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal("Aprovar o comentário?", "Esta ação não poderá ser revertida.", "aprove-comment", id);
})

$(".comment-delete-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal("Apagar o comentário?", "Se houver quaisquer arquivos associados ao comentário, eles também serão excluídos.", "delete-comment", id);
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
    data: { "commentId": id },
    url: "/comments/aprovecomment",
    dataType: "text",
    success: function (response) {
      $(comment).find(".comment-aprove-btn").remove();
      $(comment).find(".status-post").remove();
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
      "commentId": id
    },
    url: "/comments/deletecomment",
    dataType: "text",
    success: function (response, status) {
      if (status == "success") {
        comment.fadeOut(700);
        setTimeout(() => {
          comment.remove();
        }, 700)
        toastr.success("O comentário foi apagado.", "Sucesso!");
        $("#comment-count").text($("#comment-count").text() - 1);
        $(comment).find(".comment-aprove-btn").remove();
        $(comment).find(".status-post").remove();
        $(".sub-menu > li.all-posts").remove();
        $(".sub-menu > li.unrevised-posts").remove();
        $(".sub-menu > li.unrevised-comments").remove();
        $("#list-posts-content").html(response);
      }
    },
    error: function (response) {
      toastr.error("Ocorreu um erro ao tentar enviar a requisição.", "Erro!");
    }
  })
  comment.fadeOut(700);
  setTimeout(() => {
    comment.remove();
  }, 700)
}

$(".comment-edit-btn").on("click", function () {
  $(".wang-editor").remove();
  $("#comment-tab").css("pointer-events", "none");
  $("#comment-tab").css("color", "#55555545");
  $(".comment-edit-btn").css("display", "none");
  $(".comment-delete-btn").css("display", "none");
  $(".comment-aprove-btn").css("display", "none");
  var comment = $(this).parents(".comment-box");
  var id = $(this).parents(".comment-box").data("comment-id");
  $.ajax({
    type: "GET",
    dataType: "html",
    data: { "commentId": id },
    url: "/comments/commenteditform",
    success: function (response) {
      $("#wang-editor-script").remove();
      $(comment).html(response);
    },
    error: function (response) {
    }
  })
})