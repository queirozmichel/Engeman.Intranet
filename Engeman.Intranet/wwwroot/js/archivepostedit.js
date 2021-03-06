$(document).ready(function () {
  FormComponents.init(); // Init all form-specific plugins
  countFiles();
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

$("#archive-post-edit-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#archive-post-edit-form").valid()) {
    var formData = new FormData(this);
    $.ajax({
      type: "POST",
      url: "UpdateArchive",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          $("#question-details").empty();
          $("#question-details").html(response);
          toastr.success("O arquivo foi salvo", "Sucesso!");
        }
      },
      error: function (response) {
        toastr.error("O arquivo não foi salvo", "Erro!");
      }
    });
  }
})

$("#back-to-list-button").on("click", function () {
  $.ajax({
    type: "POST",
    url: "BackToList",
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    },
    error: function () {
      toastr.error("O aarquivo não foi atualizad", "Erro!");
    }
  });
})

$(".fa-xmark").on("click", function () {
  $(this).parent().css("display", "none");
  $(this).parent().find(".archive-active").val("N");
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