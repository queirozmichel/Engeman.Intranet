$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  FormComponents.init();

  countFiles();

  sessionStorage.setItem("postId", $("#post-id").val());

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
      "binaryData": {
        required: {
          depends: function (element) {
            if (countFiles() > 0) {
              return false
            } else {
              return true
            }
          }
        }
      },
      "fileType": {
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

  if (sessionStorage.getItem("editAfterDetails") != null) {
    $(".back-button").attr("title", "Voltar para os detalhes da postagem");
  }
})

if ($("#restricted").is(":checked")) {
  $("#restricted").bootstrapSwitch({
    onText: "sim",
    offText: "n&atilde;o",
    size: "normal",
    state: true,
  });
  $(".departments-list").css("display", "block");
}

$("#edit-post-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  $("#edit-post-form").validate();
  if ($("#edit-post-form").valid()) {
    var formData = new FormData(this);
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
            type: "PUT",
            contentType: false,
            processData: false,
            url: "/posts/updatepost",
            data: formData,
            beforeSend: function () {
              startSpinner();
            },
            success: function (response) {
              if (response == 200) {
                toastr.success("As alterações foram salvas", "Sucesso!");
                window.history.back();
              }
            },
            error: function (response) {
              if (response.status == 500) {
                toastr.error(response.responseText, "Erro " + response.status);
              }
            },
            complete: function () {
              stopSpinner();
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

$(".back-button").on("click", function (event) {
  previousPage();
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

$(".icon-remove-circle").on("click", function () {
  $(this).parent().css("display", "none");
  $(this).parent().find(".file-active").val("false");
  var qty = countFiles();
  if (qty == 0) {
    $("#file").parent().prev().append("<span class=\"required\">*</span>");
    $(this).parent().parent().append("<p class=\"none-file\">Nenhum arquivo</p>");
  }
})