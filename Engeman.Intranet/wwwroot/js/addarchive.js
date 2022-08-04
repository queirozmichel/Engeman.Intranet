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

$("#archive-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  //usado para receber além dos dados texto, o arquivo também
  if ($("#archive-form").valid()) {
    var formData = new FormData(this);
    //contentType e processData são obrigatórios
    $.ajax({
      type: "POST",
      url: "addarchive",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("O arquivo foi salvo", "Sucesso!");
          clearForm();
        }
      },
      error: function (response) {
        toastr.error("O arquivo não foi salvo", "Erro!");
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