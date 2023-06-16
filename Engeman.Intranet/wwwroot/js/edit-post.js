var filesToBeRemove = new String();

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
        required: true,
        normalizer: function (value) {
          return RemoveHTMLTags(value);
        }
      },
      "departmentsList": {
        required: {
          depends: function (element) {
            return $("#restricted").bootstrapSwitch("state");
          }
        }
      },
      "postType": {
        required: true
      },
      "addFiles": {
        required: {
          depends: function (element) {
            if (($("#postType").val() == 'D' || $("#postType").val() == 'M') && countFiles() == 0) {
              return true
            } else {
              return false
            }
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
    messages: {
      "addFiles": {
        accept: "Por favor, forneça arquivo(s) com a extensão .pdf",
      }
    },
    ignore: '*:not([name])',
  });

  if (sessionStorage.getItem("editAfterDetails") != null) {
    $(".back-button").attr("title", "Voltar para os detalhes da postagem");
  }

  new Promise((resolve, reject) => {
    $.ajax({
      type: "GET",
      url: "/postkeywords/getkeywordlist",
      datatype: "json",
      data: { "postId": sessionStorage.getItem("postId") },
      success: function (response) {
        resolve(
          new Tokenfield({
            el: document.querySelector("#keywords"),
            items: response.keywords,
            setItems: response.postKeywords,
            newItems: false
          }),
          $(".tokenfield-input").blur(),
        )
      },
      error: function (error) {
        reject(error)
      },
    })
  })
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
          showAlertModal("Atenção!", `O formulário contém o(s) termo(s) ${response.termsFounded}, de uso não permitido. É necessário removê-lo(s) para poder continuar.`);
        } else {
          Cookies.set('FilesToBeRemove', filesToBeRemove);
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
              Cookies.remove('FilesToBeRemove');
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
  filesToBeRemove = filesToBeRemove + $(this).data("file-id") + " ";
  $(this).parent().css("display", "none");
  var qty = countFiles();
  if (($("#postType").val() == 'D' || $("#postType").val() == 'M') && qty == 0) {
    $(".add-files").find("label").append("<span class=\"required\">*</span>");
  }
  if (qty == 0) {
    $(".files").hide();
  }
})

$("#postType").change(function () {
  if ((($(this).val() == 'D' || $(this).val() == 'M') && countFiles() == 0) && $(".add-files").find("span").length == 0) {
    $(".add-files").find("label").append("<span class=\"required\">*</span>");
  }
  if (($(this).val() == 'I' || $(this).val() == 'Q')) {
    $(".add-files").find("span").remove();
  }
})