$(document).ready(function () {

  $("#grid-data").bootgrid({
    ajax: true,
    url: "QuestionsAnswers/GetDataToGrid",
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
      "action": function (column, row) {
        return "<button type=\"button\" class=\"btn btn-xs btn-default action-details\" data-row-id=\"" + row.id + "\"><i class=\"fa-regular fa-file-lines\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default action-edit\" data-row-id=\"" + row.id + "\"><i class=\"fa fa-pencil\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default action-delete\" data-row-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      }
    }
  }).on("loaded.rs.jquery.bootgrid", function () {})
});