$(document).ready(function () {
  "use strict";
  App.init(); // Init layout and core plugins
  Plugins.init(); // Init all plugins
  setInterval(exclamation, 1000);

  //Define em um cookie, as permissões de usuário de forma criptografada. 
  fetch("/useraccount/getpermissions")
    .then(response => {
      if (!response.ok) throw new Error();
      return response.text();
    })
    .then(permissions => {
      encryptData(permissions).then(response => {
        Cookies.set("Permissions", response);
      });
    })
    .catch(error => {
      toastr.error(error.message, "Erro!");
    });

  $(window).on("popstate", function (event) {
    $.ajax({
      type: "GET",
      url: event.originalEvent.state,
      dataType: "html",
      beforeSend: function () {
        startSpinner();
      },
      success: function (response) {
        $("#render-body").empty();
        $("#render-body").html(response);
      },
      error: function () {
        toastr.error("Não foi possível voltar à tela anterior", "Erro!");
      },
      complete: function () {
        stopSpinner();
      },
    })
  })
})

$(".dashboard-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/dashboard",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function (response) {
      toastr.error("Não foi possível acessar a tela de dashboard", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
})

$("#new-post-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/posts/newpost",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, "Nova postagem", this.url);

    },
    error: function (response) {
      if (response.status == 401) {
        showAlertModal("Erro " + response.status, "Você não tem permissão para criar uma nova postagem.");
      }
    },
    complete: function () {
      stopSpinner();
    }
  })
})

$(".user-profile-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/useraccount/edituserprofile",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, "Perfil de usuário", this.url);
    },
    error: function () {
      toastr.error("Não foi possível acessar o perfil de usuário", "Erro!");
    },
    complete: function () {
      stopSpinner();
    }
  })
})

$(".users-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/useraccount/grid",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function () {
      toastr.error("Não foi possivel carregar a tela de usuários", "Erro!");
    },
    complete: function () {
      stopSpinner();
    },
  })
})

$(".logs-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/logs/grid",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function () {
      toastr.error("Não foi possivel acessar a tela de logs.", "Erro!");
    },
    complete: function () {
      stopSpinner();
    },
  })
})

$(".blacklist-terms-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/blacklistterms/grid",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function () {
      toastr.error("Não foi possivel acessar a tela de termos proibidos.", "Erro!");
    },
    complete: function () {
      stopSpinner();
    },
  })
})

$(".keywords-btn").on("click", function (event) {
  event.preventDefault();
  $.ajax({
    type: "GET",
    url: "/keywords/grid",
    dataType: "html",
    beforeSend: function () {
      startSpinner();
    },
    success: function (response) {
      $("#render-body").empty();
      $("#render-body").html(response);
      window.history.pushState(this.url, null, this.url);
    },
    error: function () {
      toastr.error("Não foi possivel acessar a tela de palavras-chave.", "Erro!");
    },
    complete: function () {
      stopSpinner();
    },
  })
})

//aguardando revisão
function exclamation() {
  var icon;
  icon = $(".blinked-exclamation");
  setTimeout(function () {
    icon.css("opacity", "0");
  }, 0);
  setTimeout(function () {
    icon.css("opacity", "1");
  }, 500);
}

//Marcação em azul quando uma opção do menu lateral esquerdo é selecionada
$("#nav li").on("click", function (event) {
  var li = $(event.target.parentElement);
  var allLi = $("#nav").children();
  allLi.each(function (index, element) {
    $(this).removeClass("current");
  })
  if (!li.hasClass("current")) {
    li.addClass("current");
  }
})