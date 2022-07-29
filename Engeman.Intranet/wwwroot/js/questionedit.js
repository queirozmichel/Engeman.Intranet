$(document).ready(function () {
  FormComponents.init(); // Init all form-specific plugins

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

  //$('#multiselect-department').multiselect('select', ['1', '2', '4'])

})

$("#edit-question-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#edit-question-form").valid()) {
    var formData = $("#edit-question-form").serializeArray();
    $.ajax({
      type: "POST",
      dataType: 'text',
      async: true,
      url: "UpdateQuestion",
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A pergunta foi atualizada", "Sucesso!");
          $("#question-details").empty();
          $("#question-details").html(response);
        }
      },
      error: function () {
        toastr.error("A pergunta não foi atualizada", "Erro!");
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
      toastr.error("A pergunta não foi atualizada", "Erro!");
    }
  });
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