$(document).ready(function () {
  $("#grid-data").bootgrid({
    ajax: true,
    post: function () {
      return {
        id: "b0df282a-0d67-40e5-8558-c9e93b7befed"
      };
    },
    url: "QuestionsAnswers/GetDataToGrid",
    formatters: {}
  });
});