$(document).ready(function () {

  $(document).tooltip();
  toastrConfig();

});

//Lista de restri��o de setores
$("#multiselect-department").multiselect({
  nonSelectedText: 'Nenhum ',
  includeSelectAllOption: true,
  allSelectedText: 'Todos ',
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

//bot�o para habilitar a inser��o de arquivos na nova postagem
$("#attach-files").bootstrapSwitch({
  onText: "sim",
  offText: "n&atilde;o",
  size: "normal",
  state: false,
});

//Valida��o do campo ap�s inser��o do(s) arquivo(s)
$("#file").on("change", function () {
  $(this).valid();
})

//Valida��o do campo sele��o dos setores
$("#multiselect-department").on("change", function () {
  $(this).valid();
})

//Marca��o em azul quando uma op��o do menu lateral esquerdo � selecionada
$("#nav li").on("click", function (event) {
  var li = $(event.target.parentElement);
  if (li.hasClass("current")) {
    li.removeClass("current");
  } else {
    var allLi = $("#nav").children();
    allLi.each(function (index, element) {
      $(this).removeClass("current");
    })
    li.addClass("current");
  }
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