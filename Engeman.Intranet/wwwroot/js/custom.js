$(document).ready(function () {

  $(document).tooltip();
  toastrConfig();

});

//Lista de restri��o de setores
$("#multiselect-department").multiselect({
  nonSelectedText: 'Nenhum',
  includeSelectAllOption: false,
  buttonTitle: function () { },
});
$(".multiselect-container").find('label').each(function () {
  $(this).removeAttr("title");
});

//bot�o para habilitar a restri��o
$("#restricted").bootstrapSwitch({
  onText: "sim",
  offText: "n&atilde;o",
  size: "normal",
  state: false,
});

//Valida��o do campo ap�s inser��o do(s) arquivo(s)
$("#add-files").on("change", function () {
  $(this).valid();
})

//Valida��o do campo sele��o dos setores
$("#multiselect-department").on("change", function () {
  $(this).valid();
})

$("#restricted").on("switchChange.bootstrapSwitch", function (event, state) {
  if (state == true) {
    $(".departments-list").css("display", "block");
    $(".departments-list").find(".btn-group").removeClass("open");
  } else {
    $(".departments-list").find(".btn-group").addClass("open");
    $("#multiselect-department").multiselect('deselectAll', true);
    $("#multiselect-department").multiselect('updateButtonText');
    $(".departments-list").css("display", "none");
  }
});

//configura��o do Toastr
function toastrConfig() {
  toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-bottom-center",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
  };
}

//voltar para a p�gina anterior
function previousPage() {
  event.preventDefault();
  window.history.back();
}