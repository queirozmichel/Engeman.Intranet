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
      dataType: "html",
      url: "/posts/newcomment",
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
            url: "/posts/backtolist",
            success: function (response) {
              $(".body-content").empty();
              $(".body-content").html(response);
              if ($("#wang-editor-script").length) $("#wang-editor-script").remove(); // remove o script do componente WangEditor par aque possa ser criado novamente na próxima chamada
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

$("#post-tab").on("click", function () {
  var idPost = $("#id-post").text();
  $.ajax({
    type: "GET",
    dataType: "html",
    data: { "idPost": idPost },
    url: "/comments/commentlist",
    success: function (response) {
      $("#comment-list").empty();
      $("#comment-list").html(response);
    },
    error: function (response) {
      toastr.error("", "Erro!");
    }
  })
})

$("#comment-tab").on("click", function () {
  $(".wang-editor").remove();
  $("#wang-editor-script").remove();
  $.ajax({
    type: "GET",
    dataType: "html",
    url: "/comments/newcomment",
    success: function (response) {
      $("#form-group-wang-editor").html(response);
    },
    error: function (response) {
      toastr.error("", "Erro!");
    }
  })
})