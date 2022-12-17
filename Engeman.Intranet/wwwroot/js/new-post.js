$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  FormComponents.init();
  $("#new-post-form").validate({
    rules: {
      "Subject": {
        required: true
      },
      "Description": {
        required: true
      },
      "DepartmentsList": {
        required: {
          depends: function (element) {
            return $("#restricted").bootstrapSwitch("state");
          }
        }
      },
      "Files": {
        required: {
          depends: function (element) {
            return $("#attach-files").bootstrapSwitch("state");
          }
        }
      },
      "PostType": {
        required: {
          depends: function (element) {
            return $("#attach-files").bootstrapSwitch("state");
          }
        }
      }
    },
    highlight: function (element, errorClass) {
      $(element).parents('.form-group').addClass(errorClass);
    },
    errorPlacement: function (error, element) {
      if (element.attr("type") == "radio") {
        element.parent().parent().append(error);
      }
      else if (element.context.id == "wang-editor-description") {
        error.insertAfter("#wang-editor-div");
      }
      else {
        error.insertAfter(element);
      }
    },
    ignore: []
  });

  $(".files").css("display", "none");
})


$("#description-label").css("color", "#555555");

$("#new-post-form").on("submit", function (event) {
  $("#description-label").css("color", "");
  //ignora o submit padrão do formulário
  event.preventDefault();
  //usado para receber além dos dados texto, o arquivo também
  if ($("#new-post-form").valid()) {
    var formData = new FormData(this);
    //contentType e processData são obrigatórios para upload de arquivos
    $.ajax({
      type: "POST",
      url: "/posts/newpost/",
      contentType: false,
      processData: false,
      data: formData,
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A postagem foi salva", "Sucesso!");
          $.ajax({
            type: "GET",
            url: "/posts/grid?filter=allPosts",
            dataType: "html",
            beforeSend: function () {
              startSpinner();
            },
            success: function (response) {
              $("#render-body").empty();
              $("#render-body").html(response);
              window.history.pushState(this.url, null, this.url);
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            },
            complete: function () {
              stopSpinner();
            }
          });
        }
      },
      error: function (response) {
        toastr.error("A postagem não foi salva", "Erro!");
      }
    });
  }
})

$("#attach-files").on("switchChange.bootstrapSwitch", function (event, state) {
  if (state == true) {
    $(".files").css("display", "block");
  } else {
    $(".files").css("display", "none");
  }
});