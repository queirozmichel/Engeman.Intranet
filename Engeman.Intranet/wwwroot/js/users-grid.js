var elementAux;

$(window).on("load", function () {
  stopSpinner();
})

$(document).ready(function () {


})

var usersGrid = $("#users-grid").bootgrid({
  ajax: true,
  css: {
    dropDownMenuItems: "dropdown-menu pull-right dropdown-menu-grid",
    left: "text-left",
  },
  url: "/useraccount/datagrid",
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
    return request;
  },
  templates: {
    header:
      "<div id=\"{{ctx.id}}\" class=\"{{css.header}}\">" +
      "<div class=\"row\">" +
      "<div class=\"col-sm-12 actionBar\">" +
      "<button id=\"btn-new-user\" class=\"btn btn-default pull-left \" type=\"button\"><i class=\"fa-solid fa-user-plus\"></i> Novo usuário </button>" +
      "<p class=\"{{css.search}}\"></p>" +
      "<p class=\"{{css.actions}}\"></p>" +
      "<div id=\"filter\" class=\"{{css.dropDownMenu}}\" data-filter=\"all\">" +
      "<button id=\"filter-button\" class=\"btn btn-default dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\">" +
      "<span class=\"{{css.dropDownMenuText}}\">Todos</span> " +
      "<span class=\"caret\"></span>" +
      "</button>" +
      "<ul class=\"{{css.dropDownMenuItems}}\" role=\"menu\">" +
      "<li>" +
      "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"all\" data-filter=\"all\">Todos</a>" +
      "<li>" +
      "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"actives\">Ativos</a>" +
      "</li>" +
      "</li>" +
      "<li>" +
      "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"moderators\">Moderadores</a>" +
      "</li>" +
      "<li>" +
      "<a class=\"{{css.dropDownItem}} {{css.dropDownItemButton}} filter-type\" data-value=\"novices\">Novatos</a>" +
      "</li>" +
      "</ul>" +
      "</div>" +
      "</div>" +
      "</div>" +
      "</div>",
    row: "<tr {{ctx.attr}}>{{ctx.cells}}></tr>"
  },
  formatters: {
    //Por padrão as chaves do json retornado são no formato camelCase (id, postType, changeDate e etc.)
    id: function (column, row) {
      return row.id;
    },
    name: function (column, row) {
      return row.name;
    },
    username: function (column, row) {
      return row.username;
    },
    department: function (column, row) {
      return row.department;
    },
    moderator: function (column, row) {
      return row.moderator;
    },
    novice: function (column, row) {
      return row.novice;
    },
    active: function (column, row) {
      return row.active;
    },
    action: function (column, row) {
      var buttons;
      var btn1 = "<button title=\"Editar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"edit\" data-user-id=\"" + row.id + "\"\"><i class=\"fa fa-pencil\"></i></button> ";
      var btn2 = "<button title=\"Apagar\" type=\"button\" class=\"btn btn-xs btn-default\" data-action=\"delete\" data-user-id=\"" + row.id + "\"><i class=\"fa fa-trash-o\"></i></button> ";
      buttons = btn1 + btn2;
      return buttons;
    },
  }
})

//Após carregar o grid
usersGrid.on("loaded.rs.jquery.bootgrid", function () {
  usersGrid.find("button.btn").each(function (index, element) {
    var actionButtons = $(element);
    var action = actionButtons.data("action");
    var userId = actionButtons.data("user-id");
    actionButtons.on("click", function () {
      if (action == "edit") {
        EditUserAccount(userId);
      } else if (action == "delete") {
        showConfirmationModal("Apagar o usuário?", "Se houver quaisquer postagens ou comentários associados a este usuário, também serão excluídos", "delete-user", userId);
        elementAux = $(this).parents("tr");
      }
    })
  });
})

$(".filter-type").on("click", function () {
  $(".filter-type").removeAttr("data-filter");
  $(".filter-type").removeData("filter");
  if ($(this).data("value") == "all") {
    $(this).attr("data-filter", "all");
    $("#filter").attr("data-filter", "all");
  } else if ($(this).data("value") == "actives") {
    $(this).attr("data-filter", "actives");
    $("#filter").attr("data-filter", "actives");
  } else if ($(this).data("value") == "moderators") {
    $(this).attr("data-filter", "moderators");
    $("#filter").attr("data-filter", "moderators");
  } else if ($(this).data("value") == "novices") {
    $(this).attr("data-filter", "novices");
    $("#filter").attr("data-filter", "novices");
  }
  $("#users-grid").bootgrid("reload");
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

$(".filter-type").on("click", function () {
  $(this).parents("div#filter").find("span.dropdown-text").text($(this).text());
});

$("#btn-new-user").on("click", function () {
  $.ajax({
    type: "GET",
    url: "/useraccount/newuser",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#new-user-modal-body").empty();
      $("#new-user-modal-body").html(response);
      $("#new-user-modal").modal("show");
    },
    error: function () {
      toastr.error("Não possível renderizar o fomulário de Novo Usuário", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
})

$(".btn-yes, .btn-no").on("click", function () {
  if ($(this).attr("id") == "delete-user") {
    deleteUser($(this).attr("data-id"), elementAux)
    hideConfirmationModal();
  }
  else {
    hideConfirmationModal();
  }
})

function EditUserAccount(userId) {
  $.ajax({
    type: "GET",
    data: { "userId": userId },
    dataType: "html",
    url: "/useraccount/edituseraccount",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function () {
      toastr.error("Não foi possível carregar a tela de edição de usuário", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
}

function deleteUser(userId, elementAux) {
  $.ajax({
    type: "DELETE",
    data: { 'userId': userId },
    url: "/useraccount/deleteuser",
    dataType: "json",
    success: function (response) {
      if (response.result == 200) {
        $(elementAux).fadeOut(700);
        setTimeout(() => {
          $(elementAux).remove();
          $("#users-grid").bootgrid("reload");
        }, 700);
        toastr.success("O usuário foi apagado.", "Sucesso!");
      } else if (response.result == 500) {
        showAlertModal("Não foi possível apagar o usuário!", response.message);
      } else {
        showAlertModal("Erro!", "Ocorreu um erro indefinido ao tentar processar a solicitação.");
      }
    },
    error: function (response) {
      toastr.error("Ocorreu um erro ao tentar enviar a requisição.", "Erro!");
    },
  });
}