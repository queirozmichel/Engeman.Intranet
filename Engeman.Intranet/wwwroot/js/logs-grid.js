var elementAux;

$(window).on("load", function () {
  stopSpinner();
})

var logsGrid = $("#logs-grid").bootgrid({
  ajax: true,
  url: "/logs/datagrid",
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
  requestHandler: function (request) {
    request.username = $("#logs-grid").data("username");
    return request;
  },
  templates: {
    header:
      "<div id=\"{{ctx.id}}\" class=\"{{css.header}}\">" +
      "<div class=\"row\">" +
      "<div class=\"col-sm-12 actionBar\">" +
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
    operation: function (column, row) {
      return row.operation;
    },
    description: function (column, row) {
      return "<span title=\"" + row.operation + " " + row.description + "\">" + row.description + "</span>";
      //return row.description;
    },
    referenceId: function (column, row) {
      return row.referenceId;
    },
    changeDate: function (column, row) {
      return row.changeDate;
    },
    
    action: function (column, row) {
      var buttons;
      var btn1 = "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-log-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      buttons = btn1;
      return buttons;
    },
  }
})

//Após carregar o grid
logsGrid.on("loaded.rs.jquery.bootgrid", function () {
  logsGrid.find("button.btn").each(function (index, element) {
    var actionButtons = $(element);
    var action = actionButtons.data("action");
    var logId = actionButtons.data("log-id");
    actionButtons.on("click", function () {
      if (action == "delete") {
        showConfirmationModal("Apagar o Log?", "Esta ação não poderá ser revertida", "delete-log", logId);
        elementAux = $(this).parents("tr");
      }
    })
  });
})