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

closeSpinner();

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
          toastr.success("O comentário foi salvo", "Sucesso!");
          $.ajax({
            type: "POST",
            dataType: "html",
            url: "/posts/backtolist" + window.location.search,
            success: function (response) {
              $("#render-body").empty();
              $("#render-body").html(response);
              if ($("#wang-editor-script").length) $("#wang-editor-script").remove(); // remove o script do componente WangEditor par aque possa ser criado novamente na próxima chamada
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            },
            complete: function () {
              closeSpinner();
            },
          });
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
    success: function (response) {
      $("#form-group-wang-editor").html(response);
    },
    error: function (response) {
      toastr.error("", "Erro!");
    }
  })
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

$(".edit-post-button").on("click", function () {
  var idPost = $("#id-post").text();
  $.ajax({
    type: "GET",
    data: { "idPost": idPost },
    dataType: "html",
    url: "/posts/questionedit" + window.location.search, //assim é passado os parâmetros da url na chamada ajax "ViewBag.FilterGrid = Request.Query["filter"]"
    error: function () {
      toastr.error("Não foi possível editar a postagem", "Erro!");
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
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
        type: "POST",
        dataType: "html",        
        url: "/posts/backtolist" + window.location.search,
        beforeSend: function () {
          startSpinner();
        },
        success: function (response) {
          $("#render-body").empty();
          $("#render-body").html(response);
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