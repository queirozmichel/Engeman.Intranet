$(window).on("load", function () {
  closeSpinner();
});

$(document).ready(function () {
  $("#multiselect-department").multiselect({
    nonSelectedText: 'Nenhum ',
    includeSelectAllOption: true,
    allSelectedText: 'Todos ',
  });

  $("#restricted").bootstrapSwitch({
    onText: "Sim",
    offText: "Não",
    size: "normal",
    state: false,
  });
})

function clearForm() {
  $(".archive-type").prop('checked', false);
  $("#subject").val('');
  $("#description").val('');
  $("#file").val('');
  $(".tag").remove();
  $(".form-group").removeClass("has-success")
  $("#restricted").bootstrapSwitch("state", false);
}

$("#document-manual-form").on("submit", function (event) {
  var formType = $("#submit-button").attr("data-file-type");
  //ignora o submit padrão do formulário
  event.preventDefault();
  //usado para receber além dos dados texto, o arquivo também
  if ($("#document-manual-form").valid()) {
    var formData = new FormData(this);
    //contentType e processData são obrigatórios para upload de arquivos
    if (formType == 'D') {
      formData.append('fileType', 'D');
    } else {
      formData.append('fileType', 'M');
    }
    $.ajax({
      type: "POST",
      url: "newdocumentmanual",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("O documento/manual foi salvo", "Sucesso!");
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
        toastr.error("O documento/manual não foi salvo", "Erro!");
      }
    });
  }
})

$("#file").on("change", function () {
  $(this).valid();
})

$("#multiselect-department").on("change", function () {
  $(this).valid();
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