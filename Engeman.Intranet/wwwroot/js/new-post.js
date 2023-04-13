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
      "files": {
        required: {
          depends: function (element) {
            return $("#attach-files").bootstrapSwitch("state");
          }
        }
      },
      "postType": {
        required: {
          depends: function (element) {
            return $("#attach-files").bootstrapSwitch("state");
          }
        }
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
    ignore: '*:not([name])',
  });

  $(".files").css("display", "none");
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

$("#attach-files").on("switchChange.bootstrapSwitch", function (event, state) {
  if (state == true) {
    $(".files").css("display", "block");
  } else {
    $(".files").css("display", "none");
    $("#file").val('');
  }
});