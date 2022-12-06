$(window).on("load", function () {
  closeSpinner();
});

$(document).ready(function () {
  FormComponents.init();

  $("#tab_1_3").removeClass("active");

  $("#comment-form").validate({
    rules: {
      description: {
        required: true
      }
    },
    ignore: []
  });
})

$("#comment-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#comment-form").valid()) {
    //usado para receber além dos dados texto, o arquivo também
    var formData = new FormData(this);
    formData.append("postId", sessionStorage.getItem("postId"));
    //contentType e processData são obrigatórios
    $.ajax({
      type: "POST",
      url: "/posts/newcomment",
      dataType: "html",
      contentType: false,
      processData: false,
      data: formData,
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response == -1) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          var idPost = $("#id-post").text();          
          $.ajax({
            type: "GET",
            dataType: "html",
            data: {"idPost": idPost},
            url: "/posts/postdetails",
            success: function (response) {
              $("#render-body").empty();
              $("#render-body").html(response);
            },
            error: function () {
              toastr.error("Não foi possível ir para os detalhes da postagem", "Erro!");
            },
            complete: function () {
              closeSpinner();
            },
          });
          toastr.success("O comentário foi salvo", "Sucesso!");
        }
      },
      error: function (response) {
        toastr.error("O comentário não foi salvo", "Erro!");
      }
    })
  }
})

$("#comment-tab").on("click", function () {
  $(".wang-editor").remove();
  $("#wang-editor-script").remove();
  $.ajax({
    type: "GET",
    dataType: "html",
    url: "/comments/wangeditor",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#form-group-wang-editor").html(response);
    },
    error: function (response) {
      toastr.error("Não foi possível carregar o editor", "Erro!");
    },
    complete: function () {
      closeSpinner();
    }
  })
})

$("#post-tab").on("click", function () {
  var idPost = $("#id-post").text();
  $.ajax({
    type: "GET",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    url: "/posts/postdetails?idPost=" + idPost,
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
    },
    error: function () {
      toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
    },
    complete: function () {
      closeSpinner();
    }
  })
})

$(".edit-post-button").on("click", function () {
  sessionStorage.setItem("editAfterDetails", "");
  var idPost = $("#id-post").text();
  $.ajax({
    type: "GET",
    data: { "idPost": idPost },
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
      window.history.pushState({}, {}, this.url);
    },
    complete: function () {
      closeSpinner();
    }
  })
})

$(".delete-post-button").on("click", function () {
  var idPost = $("#id-post").text();
  showConfirmationModal("Apagar a postagem?", "Esta ação não poderá ser revertida.", "delete-post", idPost);
})

$(".aprove-post-button").on("click", function () {
  var idPost = $("#id-post").text();
  showConfirmationModal("Aprovar a postagem?", "Esta ação não poderá ser revertida.", "aprove-post", idPost);
})

$(".btn-yes, .btn-no").on("click", function () {
  var postId = $("#id-post").text();
  if ($(this).attr("id") == "delete-post") {
    deletePost(postId).then((response) => {
      $.ajax({
        type: "GET",
        dataType: "html",
        url: "/posts/listall" + "?filter=" + sessionStorage.getItem("filterGrid"),
        beforeSend: function () {
          startSpinner();
        },
        success: function (response) {
          $("#render-body").empty();
          $("#render-body").html(response);
          window.history.pushState({}, '', "/posts/listall?filter=" + sessionStorage.getItem("filterGrid"));
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
          closeSpinner();
        }
      });
    })
      .catch((error) => {
        console.log(error)
      })
  } else if ($(this).attr("id") == "aprove-post") {
    aprovePost(postId);
  }
})


function deletePost(postId) {
  return new Promise((resolve, reject) => {
    $.ajax({
      type: "DELETE",
      data: {
        'idPost': postId
      },
      url: "/posts/removepost",
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

function aprovePost(idPost) {
  $.ajax({
    type: "PUT",
    data: {
      'idPost': idPost
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
      $("#post-grid").bootgrid("reload");
    }
  })
}

$(".back-button").on("click", function (event) {
  event.preventDefault();
  filter = "?filter=" + sessionStorage.getItem("filterGrid");
  $.ajax({
    type: "GET",
    url: "/posts/listall" + filter,
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
    },
    error: function () {
      toastr.error("Não foi possível conluir a ação", "Erro!");
    },
    complete: function () {
      closeSpinner();
      window.history.pushState({}, {}, "/posts/listall" + filter);
    },
  })
})

$(".comment-aprove-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal("Aprovar o comentário?", "Esta ação não poderá ser revertida.", "aprove-comment", id);
})

$(".comment-delete-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal("Apagar o comentário?", "Se houver quaisquer arquivos associados ao comentário, eles também serão excluídos.", "delete-comment", id);
})

$(".btn-yes, .btn-no").on("click", function (event) {
  if ($(this).attr("id") == "aprove-comment") {
    var id = $(this).attr("data-id");
    var comment = getCommentElement(id);
    aproveComment(id, comment);
    hideConfirmationModal();
  } else if ($(this).attr("id") == "delete-comment") {
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
      console.log("error");
    }
  })
})