$(document).ready(function () {
  FormComponents.init(); // Init all form-specific plugins
  $(document).tooltip();
  countFiles();
  $("#multiselect-department").multiselect({
    nonSelectedText: 'Nenhum ',
    includeSelectAllOption: true,
    allSelectedText: 'Todos ',
  });

  $("#restricted").bootstrapSwitch({
    onText: "Sim",
    offText: "Não",
    size: "normal",
  });

  if ($(".bootstrap-switch-on").length) {
    $(".departments-list").css("display", "block");
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

$("#document-manual-edit-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#document-manual-edit-form").valid()) {
    var formData = new FormData(this);
    $.ajax({
      type: "POST",
      url: "documentmanualupdate",
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
          window.history.pushState({}, '', '/posts/listall');
        }
      },
      error: function (response) {
        toastr.error("O arquivo não foi salvo", "Erro!");
      }
    });
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

$("#restricted").on("switchChange.bootstrapSwitch", function (event, state) {
  if (state == true) {
    $(".departments-list").css("display", "block");
    $(".departments-list").find(".btn-group").removeClass("open");
  } else {
    $(".departments-list").find(".btn-group").addClass("open");
    $("#multiselect-department").multiselect('deselectAll', true);
    $("#multiselect-department").multiselect('updateButtonText');
    $(".departments-list").css("display", "none");
  }
});