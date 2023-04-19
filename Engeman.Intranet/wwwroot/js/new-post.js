$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  FormComponents.init();

  jQuery.validator.setDefaults({
    debug: true,
    rules: {
      "subject": {
        required: true
      },
      "description": {
        required: true
      },
      "departmentsList": {
        required: {
          depends: function (element) {
            return $("#restricted").bootstrapSwitch("state");
          }
        }
      },
      "addFiles": {
        required: {
          depends: function (element) {
            let aux = $("#postType").val();
            if (aux == 'D' || aux == 'M') {
              return true;
            } else {
              false
            }
          }
        }
      },
      "postType": {
        required: true
      }
    },
    highlight: function (element, errorClass, validClass) {
      $(element).parents('.form-group').addClass(errorClass).removeClass(validClass);
      $(element.form).find("label[for=" + element.id + "]").addClass(errorClass);
      $("#wang-editor-div").css("border", "1px solid #a94442");
    },
    unhighlight: function (element, errorClass, validClass) {
      $(element).parents('.form-group').removeClass(errorClass).addClass(validClass);
      $(element.form).find("label[for=" + element.id + "]").removeClass(errorClass);
      if (element.id == "wang-editor-description") {
        $("#wang-editor-div").css("border", "1px solid #3c763d");
      }
    },
    errorPlacement: function (error, element) {
      if (element.attr("type") == "radio") {
        element.parent().parent().append(error);
      }
      else if (element[0].id == "wang-editor-description") {
        error.insertAfter("#wang-editor-div");
      }
      else {
        error.insertAfter(element);
      }
    },
    messages: {
      "addFiles": {
        accept: "Por favor, forneça arquivo(s) com a extensão .pdf",
      }
    },
    ignore: '*:not([name])',
  });
})

$("#new-post-form").on("submit", function (event) {
  event.preventDefault();
  $("#new-post-form").validate();
  if ($("#new-post-form").valid()) {
    var formData = new FormData(this);
    //contentType e processData são obrigatórios para upload de arquivos
    $.ajax({
      type: "POST",
      url: "/blacklistterms/blacklisttest",
      contentType: false,
      processData: false,
      data: formData,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.occurrences != 0) {
          showAlertModal("Atenção!", `O formulário contém ${response.occurrences} termo(s) de uso não permitido, é necessário removê-lo(s) para poder continuar.`);
        } else {
          $.ajax({
            type: "POST",
            url: "/posts/newpost/",
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
              if (response == 200) {
                $.ajax({
                  type: "GET",
                  url: "/posts/grid?filter=allPosts",
                  dataType: "html",
                  success: function (response) {
                    $("#render-body").empty();
                    $("#render-body").html(response);
                    window.history.pushState(this.url, null, this.url);
                    toastr.success("A postagem foi salva.", "Sucesso!");
                  },
                  error: function () {
                    toastr.error("Não foi possível ir para a tela de postagens.", "Erro!");
                  },
                });
              }
            },
            error: function (response) {
              if (response.status == 500) {
                toastr.error(response.responseText, "Erro " + response.status);
              }
            }
          });
        }
      },
      error: function (response) { },
      complete: function (response) {
        stopSpinner();
      }
    })
  }
})

$("#postType").change(function () {
  if ((($(this).val() == 'D' || $(this).val() == 'M')) && $(".add-files").find("span").length == 0) {
    $(".add-files").find("label").append("<span class=\"required\">*</span>");
  }
  if (($(this).val() == 'I' || $(this).val() == 'Q')) {
    $(".add-files").find("span").remove();
  }
})