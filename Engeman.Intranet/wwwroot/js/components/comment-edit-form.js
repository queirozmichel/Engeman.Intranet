var filesToBeRemove = new String();

$(document).ready(function () {
  sessionStorage.setItem("postId", $("#post-id").text());

  jQuery.validator.setDefaults({
    rules: {
      "comment.description": {
        required: true,
        normalizer: function (value) {
          return removeHTMLTags(value);
        }
      },
    },
    messages: {
      "binarydata": {
        accept: "Por favor, forneça arquivo(s) com a extensão .pdf",
      }
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
      url: "/blacklistterms/blacklisttest",
      contentType: false,
      processData: false,
      data: formData,
      dataType: "json",
      success: function (response) {
        if (response.occurrences != 0) {
          showAlertModal("Atenção!", `O formulário contém o(s) termo(s) ${response.termsFounded}, de uso não permitido. É necessário removê-lo(s) para poder continuar.`);
        } else {
          Cookies.set('FilesToBeRemove', filesToBeRemove);
          $.ajax({
            type: "POST",
            dataType: "html",
            contentType: false,
            processData: false,
            data: formData,
            url: "/comments/updatecomment",
            beforeSend: function () {
              startSpinner();
            },
            success: function (response) {              
              $("#render-body").empty();
              $("#render-body").html(response);
            },
            error: function (response) {
              if (response.status == 500) {
                toastr.error(response.responseText, "Erro " + response.status);
              }
            },
            complete: function (response) {
              Cookies.remove('FilesToBeRemove');
              stopSpinner();
              toastr.success("O comentário foi atualizado", "Sucesso!");
            }
          })
        }
      },
      error: function (response) { },
    })
  }
})

$(".icon-remove-circle").on("click", function () {
  filesToBeRemove = filesToBeRemove + $(this).data("file-id") + " ";
  $(this).parent().css("display", "none");
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