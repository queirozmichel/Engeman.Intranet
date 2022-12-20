$(document).ready(function () {
  sessionStorage.setItem("postId", $("#post-id").text());

  $('#tags-comment-edit-form').tagsInput({
    'height': 'auto',
    'width': '100%',
    'defaultText': '',
  });

  jQuery.validator.setDefaults({
    rules: {
      "comment.description": {
        required: true
      },
    },
    ignore: '*:not([name])',
  });  
})

$("#cancel-comment-edit-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    dataType: "html",
    url: "/posts/postdetails?postId=" + sessionStorage.getItem("postId"),
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
    },
    error: function () {
      toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
    },
  })  
})

$("#comment-edit-form").on("submit", function (event) {
  event.preventDefault();
  $("#comment-edit-form").validate();
  if ($("#comment-edit-form").valid()) {
    //usado para receber além dos dados texto, o arquivo também
    var formData = new FormData(this);
    $.ajax({
      type: "POST",
      dataType: "html",
      contentType: false,
      processData: false,
      data: formData,
      url: "/comments/updatecomment",
      success: function (response) {
        toastr.success("O comentário foi atualizado", "Sucesso!");
        $("#render-body").empty();
        $("#render-body").html(response);
      },
      error: function (response) {
        toastr.error("Não foi possível atualizar o comentário", "Erro!");
      }
    })
  }
})

$(".icon-remove-circle").on("click", function () {
  $(this).parent().css("display", "none");
  $(this).parent().find(".file-active").val("N");
  var qty = countFiles();
  if (qty == 0) {
    $(this).parent().parent().append("<p class=\"none-file\">Nenhum arquivo</p>");
  }
})

function countFiles() {
  var fileElements = $(".files").children("div").children("label");
  var qty = 0
  fileElements.each(function () {
    if ($(this).css("display") == "block") {
      qty++;
    }
  })
  $(".files").children("div").children("span").text(qty);
  return qty;
}