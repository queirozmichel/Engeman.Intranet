$(document).ready(function () {
  FormComponents.init();

  $("#permission").tooltip({
    position: { my: "left+15 center", at: "right center" },
    content: function () {
      return $(this).attr('title');
    }
  });

  jQuery.validator.setDefaults({
    debug: true,
    ignoreTitle: true,
    rules: {
      "name": {
        required: true,
        maxlength: 30,
        pattern: "[A-Za-záàâãéèêíóôõúçñÁÀÂÃÉÈÍÓÔÕÚÇÑ ]+"
      },
      "username": {
        required: true,
        pattern: "[a-z]+[.][a-z]+"
      },
      "departmentId": {
        required: true,
      },
      "permission": {
        required: true,
      }
    },
    messages: {
      username: {
        pattern: "O formato fornecido é inválido. Ex: joao.alencar",
      }
    },
    highlight: function (element, errorClass, validClass) {
      $(element).parents('.form-group').addClass(errorClass).removeClass(validClass);
      $(element.form).find("label[for=" + element.id + "]").addClass(errorClass);
    },
    unhighlight: function (element, errorClass, validClass) {
      $(element).parents('.form-group').removeClass(errorClass).addClass(validClass);
      $(element.form).find("label[for=" + element.id + "]").removeClass(errorClass);
    },
    errorPlacement: function (error, element) {
      error.insertAfter(element);
    },
    ignore: '*:not([name])',
  });
})

$("#new-user-form").on("submit", function (event) {
  event.preventDefault();
  $("#new-user-form").validate();
  if ($("#new-user-form").valid()) {
    var newUser = $(this).serialize();
    $.ajax({
      type: "POST",
      url: "/useraccount/newuser",
      data: newUser,
      dataType: "json",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        if (response.result == 200) {
          toastr.success("Novo usuário salvo.", "Sucesso!");
          $("#new-user-modal").modal("hide");
          $("#users-grid").bootgrid("reload");
        } else if (response.result == 500) {
          $("#new-user-modal").modal("hide");
          showAlertModal("Não foi possível salvar o novo usuário.", response.message);
        } else {
          $("#new-user-modal").modal("hide");
          showAlertModal("Ocorreu um erro indefinido ao tentar processar a solicitação:", "Erro!");
        }
      },
      error: function (response) {
        toastr.error("Ocorreu um erro ao tentar processar a solicitação.", "Erro ");
        $("#new-user-modal").modal("hide");
      },
      complete: function () {
        stopSpinner();
      }
    })
  }
})

$("#cancel-form-button").on("click", function (event) {
  event.preventDefault();
  $("#new-user-modal").modal("hide");
})

$("#username-input").on("input", function () {
  $("#email-input").val(this.value);
})