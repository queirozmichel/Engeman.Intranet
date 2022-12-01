﻿$(window).on("load", function () {
  closeSpinner();
});

$(document).ready(function () {
  FormComponents.init();
  countFiles();

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
      "Files": {
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

window.onpopstate = function (event) {
  console.log("location: " + document.location + ", state: " + JSON.stringify(event.state));
};

$("#edit-post-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#edit-post-form").valid()) {
    var formData = new FormData(this);
    $.ajax({
      type: "POST",
      contentType: false,
      processData: false,
      url: "/posts/updatepost",
      data: formData,
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          $.ajax({
            type: "GET",
            dataType: "html",
            url: "/posts/listall" + "?filter=" + sessionStorage.getItem("filterGrid"),
            success: function (response) {
              $("#render-body").empty();
              $("#render-body").html(response);
              window.history.pushState({}, '', "/posts/listall?filter=" + sessionStorage.getItem("filterGrid"));
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            },
            complete: function () {
              closeSpinner();
              toastr.success("A postagem foi atualizada", "Sucesso!");
            },
          });
        }
      },
      error: function () {
        toastr.error("A postagem não foi atualizada", "Erro!");
      },
      complete: function () {
        closeSpinner();
      }
    });
  }
})

$(".back-button").on("click", function (event) {
  event.preventDefault();

  if (sessionStorage.getItem("editAfterDetails") != null) {
    $.ajax({
      type: "GET",
      dataType: "html",
      url: "/posts/postdetails?idPost=" + sessionStorage.getItem("postId"),
      beforeSend: function () {
        startSpinner();
      },
      error: function () {
        toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
      },
      success: function (response) {
        $("#render-body").empty();
        $("#render-body").html(response);
        window.history.pushState({}, {}, this.url);
      },
      complete: function () {
        closeSpinner();
      }
    })
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
  $(this).parent().find(".file-active").val("N");
  var qty = countFiles();
  if (qty == 0) {
    $("#file").parent().prev().append("<span class=\"required\">*</span>");
    $(this).parent().parent().append("<p>Nenhum arquivo</p>");
  }
})