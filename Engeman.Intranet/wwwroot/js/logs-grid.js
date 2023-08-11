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
    },
    referenceId: function (column, row) {
      return row.referenceId;
    },
    changeDate: function (column, row) {
      return row.changeDate;
    },
  }
})