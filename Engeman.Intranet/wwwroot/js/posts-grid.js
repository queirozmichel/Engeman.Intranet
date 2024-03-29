﻿var isModerator;

$(window).on("load", function () {
  stopSpinner();
});

$(document).ready(function () {

  sessionStorage.removeItem("editAfterDetails");

  var postGrid = $("#posts-grid").on("initialize.rs.jquery.bootgrid", function (e) {
    /* your code after grid initialize goes here */
  }).bootgrid({
    ajax: true,
    url: "/posts/datagrid",
    labels: {
      all: "Tudo",
      infos: "Exibindo {{ctx.start}} até {{ctx.end}} de {{ctx.total}} registros",
      loading: "Carregando dados...",
      noResults: "Não há dados para exibir",
      refresh: "Atualizar",
      search: "Pesquisar por palavra completa"
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
        "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"informative\">Informativas</a>" +
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
        "</li>" +
        "</ul>" +
        "</div>" +
        "</div>" +
        "</div>" +
        "</div>",
    },
    formatters: {
      //Por padrão as chaves do json retornado são no formato camelCase (id, postType, changeDate e etc.)
      postType: function (column, row) {
        if (row.postType === "D") {
          return "<i title=\"Documento\" class=\"fa-solid fa-file\"></i>"
        }
        else if (row.postType === "M") {
          return "<i title=\"Manual\" class=\"fa-regular fa-file-lines\"></i>"
        }
        else if (row.postType === "Q") {
          return "<i title=\"Pergunta\" class=\"fa-regular fa-circle-question\"></i>";
        }
        else if (row.postType === "I") {
          return "<i title=\"Informativa\" class=\"fa-solid fa-circle-exclamation\"></i>";
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
      changeDate: function (column, row) {
        return row.changeDate;
      },
      action: function (column, row) {
        var buttons;
        var btn1 = btn1 = "<button title=\"Detalhes\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"details\" data-post-id=\"" + row.id + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa-solid fa-magnifying-glass\"></i></button> ";;
        var btn2 = "<button title=\"Editar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-post-id=\"" + row.id + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-pencil\"></i></button> ";
        var btn3 = "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-post-id=\"" + row.id + "\"data-post-type=\"" + row.postType + "\"><i class=\"fa fa-trash-o\"></i></button> ";
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
    }
    else if ($(this).data("value") == "informative") {
      $(this).attr("data-filter", "informative");
      $("#filter").attr("data-filter", "informative");
    }
    else if ($(this).data("value") == "question") {
      $(this).attr("data-filter", "question");
      $("#filter").attr("data-filter", "question");
    }
    else if ($(this).data("value") == "document") {
      $(this).attr("data-filter", "document");
      $("#filter").attr("data-filter", "document");
    }
    else if ($(this).data("value") == "manual") {
      $(this).attr("data-filter", "manual");
      $("#filter").attr("data-filter", "manual");
    }
    else if ($(this).data("value") == "my") {
      $(this).attr("data-filter", "my");
      $("#filter").attr("data-filter", "my");
    }
    $("#posts-grid").bootgrid("reload");
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
    //sessionStorage.setItem("filterGrid", $("#posts-grid").attr("data-filter-grid"));
    postGrid.find("button.btn").each(function (index, element) {
      var actionButtons = $(element);
      var action = actionButtons.data("action");
      var postId = actionButtons.data("post-id");
      var postType = actionButtons.data("post-type");
      actionButtons.on("click", function () {
        if (action == "details") {
          sessionStorage.setItem("postId", postId);
          sessionStorage.setItem("postType", postType);
          postDetails(postId);
        } else if (action == "edit") {
          editPost(postId);
        } else if (action == "delete") {
          postDeleteAuthorization(postId, this.closest('tr'));
        }
      })
    });
  })

  function postDeleteAuthorization(postId, element) {
    fetch(`/posts/postdeleteauthorization?postId=${postId}`)
      .then(response => {
        if (!response.ok) throw new Error();
        return response.text().then(
          permission => {
            if (permission == "DeletePost") {
              showConfirmationModal(deletePost, { postId: postId });
            }
            else if (permission == "NotAnyPost") {
              showAlertModal("Ação não suportada!", "Você não tem permissão para apagar uma postagem de terceiros.");
            }
            else if (permission == "NotInformativePost") {
              showAlertModal("Ação não suportada!", "Você não tem permissão para apagar uma postagem do tipo 'Informativa' de terceiros.");
            }
            else if (permission == "NotQuestionPost") {
              showAlertModal("Ação não suportada!", "Você não tem permissão para apagar uma postagem do tipo 'Pergunta' de terceiros.");
            }
            else if (permission == "NotDocumentPost") {
              showAlertModal("Ação não suportada!", "Você não tem permissão para apagar uma postagem do tipo 'Documento' de terceiros.");
            }
            else if (permission == "NotManualPost") {
              showAlertModal("Ação não suportada!", "Você não tem permissão para apagar uma postagem do tipo 'Manual' de terceiros.");
            }
          }
        );
      })
  }

  $(".filter-type").on("click", function () {
    $(this).parents("div#filter").find("span.dropdown-text").text($(this).text());
  });
});

function editPost(postId) {
  $.ajax({
    type: "GET",
    data: { "postId": postId },
    dataType: "html",
    url: "/posts/editpost",
    beforeSend: function () {
      startSpinner();
    },
    error: function (response) {
    },
    success: function (response) {
      if (response == 401) {
        showAlertModal("Ação não suportada!", "Você não tem permissão para editar esta postagem.");
      } else {
        $("#render-body").empty();
        $("#render-body").html(response);
        window.history.pushState(this.url, null, this.url);
      }
    },
    complete: function () {
      stopSpinner();
    }
  })
}