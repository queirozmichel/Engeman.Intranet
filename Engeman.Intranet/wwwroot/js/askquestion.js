 $(document).ready(function () {
   clearForm();
 })

 function clearForm() {
   $("#subject").val('');
   $("#description").val('');
   $(".tag").remove();
 } 

 $("#ask-form").on("submit", function (event) {
 //ignora o submit padrão do formulário
   event.preventDefault();
   var formData = $("#ask-form").serialize();
    $.ajax({
      type: "POST",
      dataType: 'text',
      async: true,
      url: "savequestion",
      data: formData,
      success: function () {
        toastr.success("A pergunta foi salva", "Sucesso!");
        clearForm();
      },
      error: function () {
        toastr.error("A pergunta não foi salva", "Erro!");
      }
    });
  })