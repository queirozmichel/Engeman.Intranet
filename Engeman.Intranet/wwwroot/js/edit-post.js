$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {
  FormComponents.init();
  countFiles();
  sessionStorage.setItem("postId", $("#post-id").val());

  $("#edit-post-form").validate({
    debug: true,
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
      "BinaryData": {
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
      "FileType": {
        required: {
          depends: function (element) {
            return $("#attach-files").bootstrapSwitch("state");
          }
        }
      }
    },
    highlight: function (element) {
      $(element).parents('.form-group').addClass('has-error');
    },
    errorPlacement: function (error, element) {
      if (element.attr("type") == "radio") {
        element.parent().parent().append(error);
      } else {
        error.insertAfter(element);
      }
    },
    ignore: '*:not([name])',
  });

  if ($("#restricted").is(":checked")) {
    $("#restricted").bootstrapSwitch({
      onText: "sim",
      offText: "n&atilde;o",
      size: "normal",
      state: true,
    });

    $(".departments-list").css("display", "block");
  }

  if (sessionStorage.getItem("editAfterDetails") != null) {
    $(".back-button").attr("title", "Voltar para os detalhes da postagem");
  }
})

$("#edit-post-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#edit-post-form").valid()) {
    var formData = new FormData(this);
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
          if (sessionStorage.getItem("editAfterDetails") != null) {
            backToPostDetails();
          } else {
            $.ajax({
              type: "GET",
              dataType: "html",
              url: "/posts/grid" + "?filter=" + sessionStorage.getItem("filterGrid"),
              success: function (response) {
                $("#render-body").empty();
                $("#render-body").html(response);
                window.history.pushState({}, {}, "/posts/grid?filter=" + sessionStorage.getItem("filterGrid"));
              },
              error: function () {
                toastr.error("Não foi possível voltar", "Erro!");
              },
              complete: function () {
                stopSpinner();
                toastr.success("A postagem foi atualizada", "Sucesso!");
              },
            });
          }

        } else {
          toastr.error("Código de resposta não tratado", "Erro!");
        }
      },
      error: function () {
        toastr.error("A postagem não foi atualizada", "Erro!");
      },
      complete: function () {
        stopSpinner();
      }
    });
  }
})

$(".back-button").on("click", function (event) {
  event.preventDefault();
  if (sessionStorage.getItem("editAfterDetails") != null) {
    backToPostDetails();
  } else {
    filter = "?filter=" + sessionStorage.getItem("filterGrid");
    $.ajax({
      type: "GET",
      url: "/posts/grid" + filter,
      dataType: "html",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        $("#render-body").empty();
        $("#render-body").html(response);
      },
      error: function () {
        toastr.error("Não foi possível voltar para as postagens", "Erro!");
      },
      complete: function () {
        stopSpinner();
        window.history.pushState({}, {}, "/posts/grid" + filter);
      },
    })
  }
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

function backToPostDetails() {
  $.ajax({
    type: "GET",
    dataType: "html",
    url: "/posts/postdetails?postId=" + sessionStorage.getItem("postId"),
    beforeSend: function () {
      startSpinner();
    },
    error: function () {
      toastr.error("Não foi possível voltar para os detalhes da postagem", "Erro!");
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState({}, {}, this.url);
    },
    complete: function () {
      stopSpinner();
    }
  })
}