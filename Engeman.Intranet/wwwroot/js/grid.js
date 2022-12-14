﻿//variáveis para armazenar o id da postagem e o elemento linha
var idPostAux;
var elementAux;
var isModerator;

$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  var postGrid = $("#post-grid").on("initialize.rs.jquery.bootgrid", function (e) {
    /* your code after grid initialize goes here */
  }).bootgrid({
    ajax: true,
    //columnSelection: false,
    css: {
      dropDownMenuItems: "dropdown-menu pull-right dropdown-menu-grid",
      left: "text-left",
    },
    url: "/posts/datagrid",
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
          request.filterHeader = $(this).data('filter');
        }
      })
      request.filterGrid = $("#post-grid").data("filter-grid");
      return request;
    },
    responseHandler: function (response) {
      isModerator = response.isModerator;
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
      //Por padrão as chaves do json retornado são no formato camelCase (id, postType, changeDate e etc.)
      id: function (column, row) {
        return row.id
      },
      postType: function (column, row) {
        if ((row.revised == false || row.unrevisedComments == true) && (row.userAccountId == Cookies.get('_UserId') || isModerator == true)) {
          return "<i title=\"Pendente de revisão\" class=\"not-revised fa-solid fa-asterisk\"></i>";
        }
        if (row.revised == true) {
          if (row.postType === "D") {
            return "<i title=\"Documento\" class=\"fa-regular fa-file-lines\"></i>"
          }
          else if (row.postType === "M") {
            return "<i title=\"Manual\" class=\"fa-solid fa-list-check\"></i>"
          }
          else if (row.postType === "N") {
            return "<i title=\"Pergunta\" class=\"fa-regular fa-circle-question\"></i>";
          }
        }
      },
      userAccountName: function (column, row) {
        return row.userAccountName;
      },
      department: function (column, row) {
        return row.department;
      },
      subject: function (column, row) {
        return "<span title=\"" + row.subject + "\">" + row.subject + "</span>";
      },
      description: function (column, row) {
        return "<span>" + row.description + "</span>";
      },
      changeDate: function (column, row) {
        return row.changeDate;
      },
      action: function (column, row) {
        var buttons;
        var btn1 = btn1 = "<button title=\"Detalhes\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"details\" data-post-id=\"" + row.id + "\"data-author-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa-regular fa-file-lines\"></i></button> ";;
        var btn2 = "<button title=\"Editar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-post-id=\"" + row.id + "\"data-author-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-pencil\"></i></button> ";
        var btn3 = "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-post-id=\"" + row.id + "\"data-author-id=\"" + row.userAccountId + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-trash-o\"></i></button> ";
        buttons = btn1 + btn2 + btn3;
        return buttons;
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
    sessionStorage.setItem("filterGrid", $("#post-grid").attr("data-filter-grid"));
    sessionStorage.removeItem("editAfterDetails");
    dropdownHideItens();
    postGrid.find("button.btn").each(function (index, element) {
      var actionButtons = $(element);
      var action = actionButtons.data("action");
      var postId = actionButtons.data("post-id");
      var authorId = actionButtons.data("author-id");
      var postType = actionButtons.data("post-type");
      actionButtons.on("click", function () {
        if (action == "details") {
          sessionStorage.setItem("postId", postId);
          sessionStorage.setItem("postType", postType);
          postDetails(postId, postType);
        } else if (action == "edit") {
          postPermissions(authorId, postId, action);
        } else if (action == "delete") {
          postPermissions(authorId, postId, action);
          elementAux = element;
          idPostAux = postId;
        } else if (action == "aprove") {
          showConfirmationModal("Aprovar a postagem?", "Esta ação não poderá ser revertida.", "aprove", postId);
        }
      })
    });
  })

  function postPermissions(authorId, postId, method) {
    $.get("/useraccount/checkpermissions", { authorId, method })
      .done(function (response) {
        if (method == "edit") {
          if (response == "EditAnyPost" || response == "EditOwnerPost") {
            editPost(postId);
          }
          else if (response == "CannotEditAnyonePost") {
            showAlertModal("Operação não suportada!", "Você não tem permissão para editar uma postagem de outra pessoa");
          }
          else if (response == "CannotEditAnyPost") {
            showAlertModal("Operação não suportada!", "Você não tem permissão para editar uma postagem");
          }
          else if (response == "CannotEditOwnerPost") {
            showAlertModal("Operação não suportada!", "Você não tem permissão para editar uma postagem");
          }
        }
        else if (method == "delete") {
          if (response == "DeleteAnyPost") {
            showConfirmationModal("Apagar a postagem?", "Se houver quaisquer arquivos associados à postagem, eles também serão excluídos", "delete-post", postId);
          }
          else if (response == "CannotDeleteAnyonePost") {
            showAlertModal("Operação não suportada!", "Você não tem permissão para apagar uma postagem de outra pessoa");
          }
          else if (response == "DeleteOwnerPost") {
            showConfirmationModal("Apagar a postagem?", "Se houver quaisquer arquivos associados à postagem, eles também serão excluídos", "delete-post", postId);
          }
          else if (response == "CannotDeleteAnyPost") {
            showAlertModal("Operação não suportada!", "Você não tem permissão para apagar uma postagem de outra pessoa");
          }
          else if (response == "CannotDeleteOwnerPost") {
            showAlertModal("Operação não suportada!", "Você não tem permissão para apagar uma postagem");
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

$(".btn-yes, .btn-no").on("click", function () {
  if ($(this).attr("id") == "delete-post") {
    deletePost(idPostAux, elementAux);
    hideConfirmationModal();
    toastr.success("A postagem foi apagada", "Sucesso!");
  } else if ($(this).attr("id") == "aprove") {
    idPostAux = $(this).attr("data-id");
    aprovePost(idPostAux);
    hideConfirmationModal();
  }
  else {
    hideConfirmationModal();
  }
})

function postDetails(postId) {
  $.ajax({
    type: "GET",
    data: { "postId": postId },
    dataType: "html",
    url: "/posts/postdetails",
    beforeSend: function () {
      startSpinner();
    },
    error: function () {
      toastr.error("Não foi possível mostrar os detalhes da postagem", "Erro!");
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    complete: function () {
      stopSpinner();
    }
  })
}

function editPost(postId) {
  $.ajax({
    type: "GET",
    data: { "postId": postId },
    dataType: "html",
    url: "/posts/editpost",
    beforeSend: function () {
      startSpinner();
    },
    error: function () {
      toastr.error("Não foi possível editar a postagem", "Erro!");
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState({}, {}, this.url);
    },
    complete: function () {
      stopSpinner();
    }
  })
}

function deletePost(postId, element) {
  $.ajax({
    type: "DELETE",
    data: {
      'postId': postId
    },
    url: "/posts/removepost",
    dataType: "text",
    success: function (result) {
      $(element).parent().parent().fadeOut(700);
      setTimeout(() => {
        $(element).parent().parent().remove();
      }, 700);
      $.ajax({
        type: "GET",
        url: "/posts/unrevisedlist",
        dataType: "html",
        success: function (result) {
          $(".sub-menu > li.all-posts").remove();
          $(".sub-menu > li.unrevised-posts").remove();
          $(".sub-menu > li.unrevised-comments").remove();
          $(".aprove-post-button").remove();
          $("#list-posts-content").html(result);
        },
        error: function (result) {
          toastr.error("Não foi possível atualizar o menu de postagens", "Erro!");
        },
      })
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