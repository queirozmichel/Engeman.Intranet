$(window).on("load", function () {
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
  var filter = "?filter=allPosts";
  if ($("#ask-form").valid()) {
    var formData = $("#ask-form").serialize();
    $.ajax({
      type: "POST",
      dataType: 'text',
      async: true,
      url: "/posts/newquestion",
      data: formData,
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A pergunta foi salva", "Sucesso!");
          $.ajax({
            type: "GET",
            url: "/posts/listall" + "?filter=" + sessionStorage.getItem("filterGrid"),
            dataType: "html",
            success: function (response) {
              $("#render-body").empty();
              $("#render-body").html(response);
              window.history.pushState({}, '', "/posts/listall?filter=allPosts");
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            },
            complete: function () {
              closeSpinner();
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

$(".back-button").on("click", function (event) {
  event.preventDefault();
  var filter = "?filter=allPosts";
  $.ajax({
    type: "GET",
    url: "/posts/listall" + filter,
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
    },
    error: function () {
      toastr.error("Não foi possível conluir a ação", "Erro!");
    },
    complete: function () {
      closeSpinner();
      window.history.pushState({}, {}, "/posts/listall" + filter);
    },
  })
})