//variáveis para armazenar o id da postagem e o elemento linha
var idPostAux;
var elementAux;

$(document).ready(function () {
  var postGrid = $("#post-grid").bootgrid({
    ajax: true,
    //columnSelection: false,
    css: {
      dropDownMenuItems: "dropdown-menu pull-right dropdown-menu-grid",
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
        return row.id
      },
      "attachment": function (column, row) {
        if (row.postType === "A") {
          return "<i class=\"fa-solid fa-paperclip\"></i>";
        }
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
        return "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"details\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa-regular fa-file-lines\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-pencil\"></i></button> " +
          "<button type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      },
    }
  })

  //Após carregar o grid
  postGrid.on("loaded.rs.jquery.bootgrid", function () {
    $("#post-grid").tooltip();
    dropdownHideItens();
    postGrid.find("button.btn").each(function (index, element) {
      var actionButtons = $(element);
      var action = actionButtons.data("action");
      var idPost = actionButtons.data("row-id");
      var userIdPost = actionButtons.data("user-id");
      var postType = actionButtons.data("post-type");
      actionButtons.on("click", function () {
        if (action == "details") {
          postDetails(idPost, postType);
        } else if (action == "edit") {
          confirmSessionUser(userIdPost, idPost, postType, action);
        } else if (action == "delete") {
          confirmSessionUser(userIdPost, idPost, postType, action);
          elementAux = element;
          idPostAux = idPost;
        }
      })
    });
  })
});

function dropdownHideItens() {
  if ($(".dropdown-menu-grid").length) {
    var attachment = $("input[name = 'attachment']");
    var action = $("input[name = 'action']")
    attachment.parent().css("display", "none");
    action.parent().css("display", "none");    
  }
}

$("#confirm-delete").on("click", function () {
  postDelete(idPostAux, elementAux);
  toastr.success("A postagem foi apagada", "Sucesso!");
  $("#confirm-modal").modal("toggle");
})

$("#cancel-delete").on("click", function () {
  $("#confirm-modal").modal("toggle");
})

function postDetails(idPost, postType) {
  if (postType === 'Q') {
    $.ajax({
      type: "GET",
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
  } else if (postType === 'A') {
    $.ajax({
      type: "GET",
      data: { "idPost": idPost },
      dataType: "html",
      url: "/posts/archivepostdetails",
      error: function () {
        toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
      },
      success: function (response) {
        $("#question-details").empty();
        $("#question-details").html(response);
      }
    })
  }
}

function postEdit(idPost) {
  $.ajax({
    type: "GET",
    data: { "idPost": idPost },
    dataType: "html",
    url: "/posts/questionedit",
    error: function () {
      toastr.error("Não foi possível editar a postagem", "Erro!");
    },
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    }
  })
}

function archivePostEdit(idPost) {
  $.ajax({
    type: "GET",
    data: { "idPost": idPost },
    dataType: "html",
    url: "/posts/archivepostedit",
    error: function () {
      toastr.error("Não foi possível editar a postagem", "Erro!");
    },
    success: function (response) {
      $("#question-details").empty();
      $("#question-details").html(response);
    }
  })
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

function confirmSessionUser(userIdPost, idPost, postType, action) {
  $.ajax({
    type: "GET",
    data: {
      'userAccountIdPost': userIdPost
    },
    dataType: "text",
    url: "/login/confirmSessionUserByAjax",
    success: function (response) {
      if (response == "false") {
        if (action == "delete") {
          $("#restrict-delete-modal").modal("toggle");
        } else if (action == "edit") {
          $("#restrict-edit-modal").modal("toggle");
        }
      }
      if (response == "true") {
        if (action == "delete") {
          $("#confirm-modal").modal("toggle");
        } else if (action == "edit") {
          if (postType === "Q") {
            postEdit(idPost);
          } else if (postType === "A") {
            archivePostEdit(idPost);
          }
        }
      }
    }
  });
}