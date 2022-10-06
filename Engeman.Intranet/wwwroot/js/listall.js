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
          postPermissions(userIdPost, idPost, postType, action);
        } else if (action == "delete") {
          postPermissions(userIdPost, idPost, postType, action);
          elementAux = element;
          idPostAux = idPost;
        }
      })
    });
  })

  function postPermissions(userIdPost, idPost, postType, method) {
    $.get("/useraccount/checkpermissions", { userIdPost, method })
      .done(function (response) {
        if (method == "edit") {
          if (response == "EditAnyPost") {
            if (postType === 'Q') {
              postEdit(idPost);
            }
            else {
              postFileEdit(idPost);
            }
          }
          else if (response == "EditOwnerPost") {
            if (postType === 'Q') {
              postEdit(idPost);
            }
            else {
              postFileEdit(idPost);
            }
          }
          else if (response == "CannotEditAnyonePost") {
            showDangerModal("Operação não suportada!", "Você não tem permissão para editar uma postagem de outra pessoa");
          }
          else if (response == "CannotEditAnyPost") {
            showDangerModal("Operação não suportada!", "Você não tem permissão para editar uma postagem");
          }
          else if (response == "CannotEditOwnerPost") {
            showDangerModal("Operação não suportada!", "Você não tem permissão para editar uma postagem");
          }
        }
        else if (method == "delete") {
          if (response == "DeleteAnyPost") {
            showConfirmModal("Apagar a postagem?", "Se houver quaisquer arquivos associados à postagem, eles também serão excluídos");
          }
          else if (response == "CannotDeleteAnyonePost") {
            showDangerModal("Operação não suportada!", "Você não tem permissão para apagar uma postagem de outra pessoa");
          }
          else if (response == "DeleteOwnerPost") {
            showConfirmModal("Apagar a postagem?", "Se houver quaisquer arquivos associados à postagem, eles também serão excluídos");
          }
          else if (response == "CannotDeleteAnyPost") {
            showDangerModal("Operação não suportada!", "Você não tem permissão para apagar uma postagem de outra pessoa");
          }
          else if (response == "CannotDeleteOwnerPost") {
            showDangerModal("Operação não suportada!", "Você não tem permissão para apagar uma postagem");
          }
        }
      });
  }

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

$(".btn-yes-post").on("click", function () {
  postDelete(idPostAux, elementAux);
  toastr.success("A postagem foi apagada", "Sucesso!");
  hideConfirmModal();
})

$(".btn-no-post").on("click", function () {
  hideConfirmModal();
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