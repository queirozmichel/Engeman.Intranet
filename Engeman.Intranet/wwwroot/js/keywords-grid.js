$(window).on("load", function () {
  stopSpinner();
})

var keywordsGrid = $("#keywords-grid").bootgrid({
  ajax: true,
  url: "/keywords/datagrid",
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
      "<button id=\"btn-new-keyword\" class=\"btn btn-default pull-left \" type=\"button\"><i class=\"fa-solid fa-plus\"></i> Nova palavra-chave </button>" +
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
      var btn1 = "<button title=\"Editar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-keyword-id=\"" + row.id + "\"\"><i class=\"fa fa-pencil\"></i></button> ";
      var btn2 = "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-keyword-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      buttons = btn1 + btn2;
      return buttons;
    },
  }
})

//Após carregar o grid
keywordsGrid.on("loaded.rs.jquery.bootgrid", function () {
  keywordsGrid.find("button.btn").each(function (index, element) {
    var actionButtons = $(element);
    var action = actionButtons.data("action");
    var keywordId = actionButtons.data("keyword-id");
    actionButtons.on("click", function () {
      if (action == "edit") {
        editKeyword(keywordId);
      } else if (action == "delete") {
        showConfirmationModal("Apagar a palavra-chave?", "Esta operação não pode ser desfeita", "delete-keyword", keywordId);
        elementAux = $(this).parents("tr");
      }
    })
  });
})

$(".btn-yes, .btn-no").on("click", function () {
  if ($(this).attr("id") == "delete-keyword") {
    deleteKeyword($(this).attr("data-id"), elementAux)
    hideConfirmationModal();
  }
  else {
    hideConfirmationModal();
  }
})

function editKeyword(keywordId) {
  $.ajax({
    type: "GET",
    data: { "keywordId": keywordId },
    dataType: "html",
    url: "/keywords/editkeyword",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#edit-keyword-modal-body").empty();
      $("#edit-keyword-modal-body").html(response);
      $("#edit-keyword-modal").modal("show");
    },
    error: function () {
      toastr.error("Não foi possível carregar a tela de edição de palavra-chave", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
}

function deleteKeyword(keywordId, elementAux) {
  $.ajax({
    type: "DELETE",
    data: { 'keywordId': keywordId },
    url: "/keywords/deletekeyword",
    dataType: "json",
    success: function (response) {
      if (response.result == 200) {
        $(elementAux).fadeOut(700);
        setTimeout(() => {
          $(elementAux).remove();
          $("#keywords-grid").bootgrid("reload");
        }, 700);
        toastr.success("A palavra-chave foi apagada.", "Sucesso!");
      } else if (response.result == 500) {
        showAlertModal("Não foi possível apagar a palavra-chave!", response.message);
      } else {
        showAlertModal("Erro!", "Ocorreu um erro indefinido ao tentar processar a solicitação.");
      }
    },
    error: function (response) {
      toastr.error("Ocorreu um erro ao tentar enviar a requisição.", "Erro!");
    },
  });
}

$("#btn-new-keyword").on("click", function () {
  $.ajax({
    type: "GET",
    url: "/keywords/newkeyword",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#new-keyword-modal-body").empty();
      $("#new-keyword-modal-body").html(response);
      $("#new-keyword-modal").modal("show");
    },
    error: function () {
      toastr.error("Não possível renderizar o fomulário de nova palavra-chave", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
})