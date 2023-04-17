$(window).on("load", function () {
  stopSpinner();
})

var blacklistTermsGrid = $("#blacklist-terms-grid").bootgrid({
  ajax: true,
  css: {
    dropDownMenuItems: "dropdown-menu pull-right dropdown-menu-grid",
    left: "text-center",
  },
  url: "/blacklistterms/datagrid",
  labels: {
    all: "Tudo",
    infos: "Exibindo {{ctx.start}} até {{ctx.end}} de {{ctx.total}} registros",
    loading: "Carregando dados...",
    noResults: "Não há dados para exibir",
    refresh: "Atualizar",
    search: "Pesquisar"
  },
  searchSettings: {
    characters: 1,
  },
  templates: {
    header:
      "<div id=\"{{ctx.id}}\" class=\"{{css.header}}\">" +
      "<div class=\"row\">" +
      "<div class=\"col-sm-12 actionBar\">" +
      "<button id=\"btn-new-term\" class=\"btn btn-default pull-left \" type=\"button\"><i class=\"fa-solid fa-plus\"></i> Novo termo </button>" +
      "<p class=\"{{css.search}}\"></p>" +
      "<p class=\"{{css.actions}}\"></p>" +
      "</div>" +
      "</div>" +
      "</div>",
    row: "<tr {{ctx.attr}}>{{ctx.cells}}></tr>"
  },
  formatters: {
    //Por padrão as chaves do json retornado são no formato camelCase (id, postType, changeDate e etc.)
    id: function (column, row) {
      return row.id;
    },
    description: function (column, row) {
      return row.description;
    },
    changeDate: function (column, row) {
      return row.changeDate;
    },
    action: function (column, row) {
      var buttons;
      var btn1 = "<button title=\"Editar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-term-id=\"" + row.id + "\"\"><i class=\"fa fa-pencil\"></i></button> ";
      var btn2 = "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-term-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      buttons = btn1 + btn2;
      return buttons;
    },
  }
})

//Após carregar o grid
blacklistTermsGrid.on("loaded.rs.jquery.bootgrid", function () {
  blacklistTermsGrid.find("button.btn").each(function (index, element) {
    var actionButtons = $(element);
    var action = actionButtons.data("action");
    var termId = actionButtons.data("term-id");
    actionButtons.on("click", function () {
      if (action == "edit") {
        editTerm(termId);
      } else if (action == "delete") {
        showConfirmationModal("Apagar o termo?", "Esta operação não pode ser desfeita", "delete-term", termId);
        elementAux = $(this).parents("tr");
      }
    })
  });
})

$(".btn-yes, .btn-no").on("click", function () {
  if ($(this).attr("id") == "delete-term") {
    deleteTerm($(this).attr("data-id"), elementAux)
    hideConfirmationModal();
  }
  else {
    hideConfirmationModal();
  }
})

function deleteTerm(termId, elementAux) {
  $.ajax({
    type: "DELETE",
    data: { 'termId': termId },
    url: "/blacklistterms/deleteterm",
    dataType: "json",
    success: function (response) {
      if (response.result == 200) {
        $(elementAux).fadeOut(700);
        setTimeout(() => {
          $(elementAux).remove();
          $("#blacklist-terms-grid").bootgrid("reload");
        }, 700);
        toastr.success("O termo foi apagado.", "Sucesso!");
      } else if (response.result == 500) {
        showAlertModal("Não foi possível apagar o termo!", response.message);
      } else {
        showAlertModal("Erro!", "Ocorreu um erro indefinido ao tentar processar a solicitação.");
      }
    },
    error: function (response) {
      toastr.error("Ocorreu um erro ao tentar enviar a requisição.", "Erro!");
    },
  });
}

function editTerm(termId) {
  $.ajax({
    type: "GET",
    data: { "termId": termId },
    dataType: "html",
    url: "/blacklistterms/editterm",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#edit-term-modal-body").empty();
      $("#edit-term-modal-body").html(response);
      $("#edit-term-modal").modal("show");
    },
    error: function () {
      toastr.error("Não foi possível carregar a tela de edição de termo", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
}

$("#btn-new-term").on("click", function () {
  $.ajax({
    type: "GET",
    url: "/blacklistterms/newterm",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#new-term-modal-body").empty();
      $("#new-term-modal-body").html(response);
      $("#new-term-modal").modal("show");
    },
    error: function () {
      toastr.error("Não possível renderizar o fomulário de novo termo", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
})