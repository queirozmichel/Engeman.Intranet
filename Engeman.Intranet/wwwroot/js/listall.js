//variáveis para armazenar o id da postagem e o elemento linha
var idPostAux;
var elementAux;

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
        return "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"details\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"><i class=\"fa-regular fa-file-lines\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"><i class=\"fa fa-pencil\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      },
    }
  })

  //Após carregar o grid
  postGrid.on("loaded.rs.jquery.bootgrid", function () {
    $("#post-grid").tooltip();
    postGrid.find("button.btn").each(function (index, element) {
      var actionButtons = $(element);
      var action = actionButtons.data("action");
      var idPost = actionButtons.data("row-id");
      var userIdPost = actionButtons.data("user-id");
      actionButtons.on("click", function () {
        if (action == "details") {
          postDetails(idPost);
        } else if (action == "edit") {
          postEdit();
        } else if (action == "delete") {
          getSessionUserId(userIdPost);
          elementAux = element;
          idPostAux = idPost;
        }
      })
    });
  })
});

$("#confirm-delete").on("click", function () {
  postDelete(idPostAux, elementAux);
  toastr.success("A postagem foi apagada", "Sucesso!");
  $("#confirm-modal").modal("toggle");
})

$("#cancel-delete").on("click", function () {
  $("#confirm-modal").modal("toggle");
})

function postDetails(idPost) {
  $.ajax({
    type: "POST",
    data: { "idPost": idPost },
    dataType: "html",
    url: "/posts/questiondetails",
    error: function () {
      toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
    },
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    }
  })
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
    success: function (result) {
      $(element).parent().parent().fadeOut(700);
      setTimeout(() => {
        $(element).parent().parent().remove();
      }, 700);
    },
    error: function (result) {
    },
    complete: function () {
      setTimeout(() => {
        $("#post-grid").bootgrid("reload");
      }, 700);
    }
  });
}

function getSessionUserId(userIdPost) {
  $.ajax({
    type: "GET",
    data: {
      'userAccountIdPost': userIdPost
    },
    dataType: "text",
    url: "/login/GetSessionUserIdByAjax",
    success: function (response) {
      if (response == "false") {
        $("#restricted-modal").modal("toggle");
      } else if (response == "true") {
        $("#confirm-modal").modal("toggle");
      }
    }
  });
}