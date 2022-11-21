$(window).on("load", function () {
  closeSpinner();  
});

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

  if ($(".bootstrap-switch-on").length) {
    $(".departments-list").css("display", "block");
  }

  if (sessionStorage.getItem("editAfterDetails") != null) {
    $(".back-button").contents().filter(function () {
        return this.nodeType == Node.TEXT_NODE;
      })[0].nodeValue = "Voltar para os detalhes";
  }
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
      url: "/posts/updatequestion" + window.location.search,
      data: formData,
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A pergunta foi atualizada", "Sucesso!");
          $("#render-body").empty();
          $("#render-body").html(response);
          window.history.pushState({}, '', window.location.search);
        }
      },
      error: function () {
        toastr.error("A pergunta não foi atualizada", "Erro!");
      },
      complete: function () {
        closeSpinner();
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

$(".back-button").on("click", function (event) {
  event.preventDefault();
  if (sessionStorage.getItem("editAfterDetails") != null) {
    window.postDetails(sessionStorage.getItem("postId"), sessionStorage.getItem("postType"));
  } else {
    filter = "?filter=" + sessionStorage.getItem("filterGrid");
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
  }
})