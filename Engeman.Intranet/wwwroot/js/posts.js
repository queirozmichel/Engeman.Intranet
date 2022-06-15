$(document).ready(function () {

  $("#grid-data").bootgrid({
    ajax: true,
    css: {
      left: "text-left",
    },
    url: "getdatagrid",
    labels: {
      infos: "Exibindo {{ctx.start}} até {{ctx.end}} de {{ctx.total}} registros",
      loading: "Carregando dados...",
      noResults: "Não há dados para exibir",
      refresh: "Atualizar",
      search: "Pesquisar"
    },
    searchSettings: {
      characters: 1,
    },
    formatters: {
      "id": function (column, row) {
        return row.id;
      },
      "userAccountName": function (column, row) {
        return row.userAccountName;
      },
      "departmentDescription": function (column, row) {
        return row.departmentDescription;
      },
      "subject": function (column, row) {
        return "<span title=\"" + row.subject + "\">" + row.subject + "</span>";
      },
      "cleanDescription": function (column, row) {
        return "<span title=\"" + row.cleanDescription + "\">" + row.cleanDescription + "</span>";
      },
      "changeDate": function (column, row) {
        return row.changeDate;
      },
      "action": function (column, row) {
        return "<button type=\"button\" class=\"btn btn-xs btn-default action-details\" data-row-id=\"" + row.id + "\"><i class=\"fa-regular fa-file-lines\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default action-edit\" data-row-id=\"" + row.id + "\"><i class=\"fa fa-pencil\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default action-delete\" data-row-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      },
    }
  }).on("loaded.rs.jquery.bootgrid", function () {
    $(function () {
      $("#grid-data").tooltip();
    });
  }) 
});



