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
      characters: 1
    }
  });
});