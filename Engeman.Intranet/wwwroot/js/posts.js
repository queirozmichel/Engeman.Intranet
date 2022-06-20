$(document).ready(function () {

  var postGrid = $("#post-grid").bootgrid({
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
        return "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"details\" data-row-id=\"" + row.id + "\"><i class=\"fa-regular fa-file-lines\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-row-id=\"" + row.id + "\"><i class=\"fa fa-pencil\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-row-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      },
    }
  })

  postGrid.on("loaded.rs.jquery.bootgrid", function () {
    $("#post-grid").tooltip();
    postGrid.find("button.btn").each(function (index, element) {
      var actionButtons = $(element);
      var action = actionButtons.data("action");
      var idElement = actionButtons.data("row-id");

      actionButtons.on("click", function () {
        if (action == "details") {
          postDetails();
        } else if (action == "edit") {
          postEdit();
        } else if (action == "delete") {
          postDelete(idElement, element);
        }
      })

    });
  })
});

function postDetails() {
  console.log("details");
}

function postEdit() {
  console.log("edit");
}

function postDelete(idElement, element) {
  $.ajax({
    type: "DELETE",
    data: {
      'idPost': idElement
    },
    url: "removepost",
    dataType: "text",
    success: function () {
      $(element).parent().parent().fadeOut(700);
      setTimeout(() => {
        $(element).parent().parent().remove();
      }, 700);
    },
    error: function () {
      console.log("erro");
    },
    complete: function () {
      setTimeout(() => {
        $("#post-grid").bootgrid("reload");
      }, 700);
    }
  });
}