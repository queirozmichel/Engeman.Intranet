$(document).ready(function () {
})

function clearForm() {
  $("#subject").val('');
  $("#description").val('');
  $(".tag").remove();
  $(".form-group").removeClass("has-success");
}

$("#ask-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#ask-form").valid()) {
    var formData = $("#ask-form").serialize();
    $.ajax({
      type: "POST",
      dataType: 'text',
      async: true,
      url: "savequestion",
      data: formData,
      success: function (response) {
        if (response == 0) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("A pergunta foi salva", "Sucesso!");
          clearForm();
        }
      },
      error: function () {
        toastr.error("A pergunta não foi salva", "Erro!");
      }
    });
  }
})

$("#restricted").parent().on("change", function () {
  console.log($("#restricted").parent());
  if ($("#restricted").parent().hasClass("switch-on")) {
    console.log("on");
  } else if ($("#restricted").parent().hasClass("switch-off")) {
    console.log("off");
  }
})