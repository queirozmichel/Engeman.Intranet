//variáveis para armazenar o id da postagem e o elemento linha
var idPostAux;
var elementAux;

$(window).on("load", function () {
  closeSpinner();
});

$(document).ready(function () {

  var postGrid = $("#post-grid").bootgrid({
    ajax: true,
    //columnSelection: false,
    css: {
      dropDownMenuItems: "dropdown-menu pull-right dropdown-menu-grid",
      left: "text-left",
    },
    url: "/posts/getdatagrid",
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
      var filterElements = $(".filter-type");
      filterElements.each(function () {
        if ($(this).data("filter") != null) {
          request.filter = $(this).data('filter');
        }
      })
      return request;
    },
    responseHandler: function (response) {
      return response;
    },
    templates: {
      header:
        "<div id=\"{{ctx.id}}\" class=\"{{css.header}}\">" +
        "<div class=\"row\">" +
        "<div class=\"col-sm-12 actionBar\">" +
        "<p class=\"{{css.search}}\"></p>" +
        "<p class=\"{{css.actions}}\"></p>" +
        "<div id=\"filter\" class=\"{{css.dropDownMenu}}\" data-filter=\"all\">" +
        "<button id=\"filter-button\" class=\"btn btn-default dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\">" +
        "<span class=\"{{css.dropDownMenuText}}\">Todas as postagens</span> " +
        "<span class=\"caret\"></span>" +
        "</button>" +
        "<ul class=\"{{css.dropDownMenuItems}}\" role=\"menu\">" +
        "<li>" +
        "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"all\" data-filter=\"all\">Todas as postagens</a>" +
        "<li>" +
        "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"my\">Minhas postagens</a>" +
        "</li>" +
        "</li>" +
        "<li>" +
        "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"question\">Perguntas</a>" +
        "</li>" +
        "<li>" +
        "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"document\">Documentos</a>" +
        "</li>" +
        "<li>" +
        "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"manual\">Manuais</a>" +
        "</li>" +
        "</ul>" +
        "</div>" +
        "</div>" +
        "</div>" +
        "</div>"
    },
    formatters: {
      "id": function (column, row) {
        return row.id
      },
      "postType": function (column, row) {
        if (row.postType === "A") {
          if (row.fileType === "D") {
            return "<i title=\"Documento\" class=\"fa-regular fa-file-lines\"></i>"
          } else if (row.fileType === "M") {
            return "<i title=\"Manual\" class=\"fa-solid fa-list-check\"></i>"
          }
        } else if (row.postType === "Q") {
          return "<i title=\"Pergunta\" class=\"fa-regular fa-circle-question\"></i>"
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
        return "<button title=\"Detalhes\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"details\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa-regular fa-file-lines\"></i></button> " +
          "<button title=\"Editar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-pencil\"></i></button> " +
          "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-row-id=\"" + row.id + "\"data-user-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      },
    }
  })

  $(".filter-type").on("click", function () {
    $(".filter-type").removeAttr("data-filter");
    $(".filter-type").removeData("filter");
    if ($(this).data("value") == "all") {
      $(this).attr("data-filter", "all");
      $("#filter").attr("data-filter", "all");
    } else if ($(this).data("value") == "question") {
      $(this).attr("data-filter", "question");
      $("#filter").attr("data-filter", "question");
    } else if ($(this).data("value") == "document") {
      $(this).attr("data-filter", "document");
      $("#filter").attr("data-filter", "document");
    } else if ($(this).data("value") == "manual") {
      $(this).attr("data-filter", "manual");
      $("#filter").attr("data-filter", "manual");
    } else if ($(this).data("value") == "my") {
      $(this).attr("data-filter", "my");
      $("#filter").attr("data-filter", "my");
    }
    $("#post-grid").bootgrid("reload");
  });

  $("#filter").on("click", function () {
    var aux = $(this).attr("data-filter");
    var filterOptions = $(".filter-type");
    filterOptions.each(function () {
      if ($(this).attr("data-filter") == aux) {
        $(this).hide();
      } else {
        $(this).show();
      }
    })
  });

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
          sessionStorage.setItem("postId", idPost);
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

  $(".filter-type").on("click", function () {
    $(this).parents("div#filter").find("span.dropdown-text").text($(this).text());
  });
});

function dropdownHideItens() {
  if ($(".dropdown-menu-grid").length) {
    var attachment = $("input[name = 'postType']");
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
        $(".body-content").empty();
        $(".body-content").html(response);
      }
    })
  } else if (postType === 'A') {
    $.ajax({
      type: "GET",
      data: { "idPost": idPost },
      dataType: "html",
      url: "/posts/documentmanualdetails",
      error: function () {
        toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
      },
      success: function (response) {
        $(".body-content").empty();
        $(".body-content").html(response);
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
      $(".body-content").empty();
      $(".body-content").html(response);
    }
  })
}

function postFileEdit(idPost) {
  $.ajax({
    type: "GET",
    data: { "idPost": idPost },
    dataType: "html",
    url: "/posts/documentmanualedit",
    error: function () {
      toastr.error("Não foi possível editar a postagem", "Erro!");
    },
    success: function (response) {
      $(".body-content").empty();
      $(".body-content").html(response);
    }
  })
}


function postDelete(idElement, element) {
  $.ajax({
    type: "DELETE",
    data: {
      'idPost': idElement
    },
    url: "/posts/removepost",
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
            postFileEdit(idPost);
          }
        }
      }
    }
  });
}