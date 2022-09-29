﻿$(window).on("load", function () {
  closeSpinner();
});

$(document).ready(function () {
  FormComponents.init();
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
  $("#subject").val('');
  $("#description").val('');
  $(".tag").remove();
  $(".form-group").removeClass("has-success");
  $("#restricted").bootstrapSwitch("state", false);
}

$("#ask-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#ask-form").valid()) {
    var formData = $("#ask-form").serialize();
    $.ajax({
      type: "POST",
      dataType: 'text',
      async: true,
      url: "/posts/newquestion",
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A pergunta foi salva", "Sucesso!");
          $.ajax({
            type: "POST",
            url: "/posts/backtolist",
            success: function (response) {
              $(".body-content").empty();
              $(".body-content").html(response);
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            }
          });
        }
      },
      error: function () {
        toastr.error("A pergunta não foi salva", "Erro!");
      }
    });
  }
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

$("#multiselect-department").on("change", function () {
  $(this).valid();
})

$(".back-to-list-button").on("click", function () {
  $.ajax({
    type: "POST",
    url: "/posts/backtolist",
    success: function (response) {
      $(".body-content").empty();
      $(".body-content").html(response);
    },
    error: function () {
      toastr.error("Não foi possível voltar", "Erro!");
    }
  });
})