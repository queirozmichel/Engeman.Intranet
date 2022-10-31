$(document).ready(function () {

  $('#tags-comment-edit-form').tagsInput({
    'height': 'auto',
    'width': '100%',
    'defaultText': '',
  });

  $("#comment-edit-form").validate({
    rules: {
      "comment.description": {
        required: true
      },
    },
    ignore: []
  });
})

$("#cancel-comment-edit-btn").on("click", function (event) {
  event.preventDefault();
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
      toastr.error("Não foi possível cancelar", "Erro!");
    }
  })
})

$("#comment-edit-form").on("submit", function (event) {
  event.preventDefault();
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
        $("#comment-list").empty();
        $("#comment-list").html(response);
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
    $("#file").addClass("required");
    $("#file").parent().prev().append("<span class=\"required\">*</span>");
    $(this).parent().parent().append("<p>Nenhum arquivo</p>");
  }
})

$("#file").on("change", function () {
  $("#file").parent().parent().removeClass("has-error").addClass("has-success")
  $("#file").removeClass("required has-error").addClass("has-success");
  $("#file").parent().find("label").remove();
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