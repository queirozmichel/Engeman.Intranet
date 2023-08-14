$(document).ready(function () {

  jQuery.validator.setDefaults({
    rules: {
      "description": {
        required: true,
        normalizer: function (value) {
          return removeHTMLTags(value);
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
  showConfirmationModal(aproveComment, { commentId: id, element: this.closest(".comment-box") });
})

$(".comment-delete-btn").on("click", function () {
  var id = $(this).parents(".comment-box").attr("data-comment-id");
  showConfirmationModal(deleteComment, { commentId: id, element: this.closest(".comment-box") });
})

function aproveComment(args) {

  const { commentId, element } = args;

  $.ajax({
    type: "PUT",
    data: { "commentId": commentId },
    url: "/comments/aprovecomment",
    dataType: "text",
    success: function (response) {
      $(element).find(".comment-aprove-btn").remove();
      $(element).find(".status-post").remove();
      toastr.success("Comentário aprovado", "Sucesso!");
    },
    error: function (response) {
      if (response == "false") {
        toastr.error("Não foi possível aprovar o comentário", "Erro!");
      }
    }
  })
}

function deleteComment(args) {

  const { commentId, element } = args;
  $.ajax({
    type: "DELETE",
    data: {
      "commentId": commentId
    },
    url: "/comments/deletecomment",
    dataType: "text",
    success: function (response) {
      if (response == 200) {
        $(element).remove();
        toastr.success("O comentário foi apagado.", "Sucesso!");
        $("#comment-count").text($("#comment-count").text() - 1);
      }
    },
    error: function (response) {
      toastr.error("Ocorreu um erro ao tentar enviar a requisição.", "Erro!");
    }
  })
}

$(".comment-edit-btn").on("click", function () {
  $(".wang-editor").remove();
  $("#comment-tab").css("pointer-events", "none");
  $("#comment-tab").css("color", "#55555555");
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