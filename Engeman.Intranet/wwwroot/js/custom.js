$(document).ready(function () {

  $(document).tooltip();

  //Lista de restrição de setores
  $("#multiselect-department").multiselect({
    nonSelectedText: 'Nenhum ',
    includeSelectAllOption: true,
    allSelectedText: 'Todos ',
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

  //===== Sidebar Search (Demo Only) =====//
  $('.sidebar-search').submit(function (e) {
    //e.preventDefault(); // Prevent form submitting (browser redirect)

    $('.sidebar-search-results').slideDown(200);
    return false;
  });

  $('.sidebar-search-results .close').click(function () {
    $('.sidebar-search-results').slideUp(200);
  });

  //===== .row .row-bg Toggler =====//
  $('.row-bg-toggle').click(function (e) {
    e.preventDefault(); // prevent redirect to #

    $('.row.row-bg').each(function () {
      $(this).slideToggle(200);
    });
  });

  //===== Sparklines =====//

  $("#sparkline-bar").sparkline('html', {
    type: 'bar',
    height: '35px',
    zeroAxis: false,
    barColor: App.getLayoutColorCode('red')
  });

  $("#sparkline-bar2").sparkline('html', {
    type: 'bar',
    height: '35px',
    zeroAxis: false,
    barColor: App.getLayoutColorCode('green')
  });

  //===== Refresh-Button on Widgets =====//

  $('.widget .toolbar .widget-refresh').click(function () {
    var el = $(this).parents('.widget');

    App.blockUI(el);
    window.setTimeout(function () {
      App.unblockUI(el);
      noty({
        text: '<strong>Widget updated.</strong>',
        type: 'success',
        timeout: 1000
      });
    }, 1000);
  });

  //===== Fade In Notification (Demo Only) =====//
  setTimeout(function () {
    $('#sidebar .notifications.demo-slide-in > li:eq(1)').slideDown(500);
  }, 3500);

  setTimeout(function () {
    $('#sidebar .notifications.demo-slide-in > li:eq(0)').slideDown(500);
  }, 7000);
});

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