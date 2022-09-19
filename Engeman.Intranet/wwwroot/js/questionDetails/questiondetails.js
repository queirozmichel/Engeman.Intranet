$(document).ready(function () {
  $('.tags').tagsInput({
    'height': 'auto',
    'width': '100%',
    'defaultText': '',
  });

  $("#tab_1_3").removeClass("active");

  $("#comment-form").validate({
    rules: {
      description: {
        required: true
      }
    },
    ignore: []
  });

  richTextBox();

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
      url: "MakeComment",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == -1) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("O comentário foi salvo", "Sucesso!");
          $.ajax({
            type: "POST",
            url: "BackToList",
            success: function (response) {
              $("#question-details").empty();
              $("#question-details").html(response);
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            }
          });
        }
      },
      error: function (response) {
        toastr.error("O comentário não foi salvo", "Erro!");
      }
    })
  }
})

$(".back-to-list-button").on("click", function () {
  $.ajax({
    type: "POST",
    url: "BackToList",
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    },
    error: function () {
      toastr.error("Não foi possível voltar", "Erro!");
    }
  });
})

$(".comment-delete-btn").on("click", function () {
  $("#confirm-modal").modal("toggle");
})

$(".comment-delete-btn, #confirm-delete, #cancel-delete").on("click", function () {
  var id;
  if ($(this).hasClass("comment-delete-btn")) {
    id = $(this).parent().parent().parent().attr("data-comment-id");
    $("#confirm-delete").attr("data-comment-id", id);
  } else if (this.id == "confirm-delete") {
    id = $(this).attr("data-comment-id");
    var comment = getCommentElement(id);
    deleteComment(id, comment);  
  } else if (this.id == "cancel-delete") {
    $("#confirm-modal").modal("toggle");
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
        $("#confirm-modal").modal("toggle");
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