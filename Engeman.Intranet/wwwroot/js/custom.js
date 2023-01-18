$(document).ready(function () {

  $(document).tooltip();
  toastrConfig();

});

//Lista de restrição de setores
$("#multiselect-department").multiselect({
  nonSelectedText: 'Nenhum ',
  includeSelectAllOption: true,
  allSelectedText: 'Todos ',
  buttonTitle: function () { },
});
$(".multiselect-container").find('label').each(function () {
  $(this).removeAttr("title");
});

//botão para habilitar a restrição
$("#restricted").bootstrapSwitch({
  onText: "sim",
  offText: "n&atilde;o",
  size: "normal",
  state: false,
});

//botão para habilitar a inserção de arquivos na nova postagem
$("#attach-files").bootstrapSwitch({
  onText: "sim",
  offText: "n&atilde;o",
  size: "normal",
  state: false,
});

//Validação do campo após inserção do(s) arquivo(s)
$("#file").on("change", function () {
  $(this).valid();
})

//Validação do campo seleção dos setores
$("#multiselect-department").on("change", function () {
  $(this).valid();
})

//Marcação em azul quando uma opção do menu lateral esquerdo é selecionada
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

//configuração do Toastr
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

//voltar para a página anterior
function previousPage() {
  event.preventDefault();
  window.history.back();
}