//Remove as Tags HTML
function removeHTMLTags(value) {
  return value.replace(/(<p.*?>|<\/p>)|(<h\d>|<\/h\d>)|(<span.*?>|<\/span>)|(style=".*?")|(<strong>|<\/strong>)|(<em>|<\/em>)|(<sup>|<\/sup>)|(<br>)|(&nbsp;)|(\s+)/g, '');
}

//Criptografa uma cadeia de caracteres utlizando o algoritimo AES
function encryptData(data) {
  if (data === null || data === undefined || data === '') {
    throw new Error("Texto para encriptação, nulo, vazio ou não definido.");
  } else {
    return fetch('/root/getcryptosecretkey')
      .then(response1 => {
        if (!response1.ok) throw new Error();
        return response1.text();
      }).then(secretKey => {
        return CryptoJS.AES.encrypt(data.toString(), secretKey).toString();
      }).catch(error => {
        toastr.error("Não foi possível obter a chave secreta", "Erro!");
      });
  }
}

//Descriptografa uma cadeia de caracteres que tenha sido criptografada utlizando o algoritimo AES
function decryptData(data) {
  if (data === null || data === undefined || data === '') {
    throw new Error("Texto para decriptação, nulo, vazio ou não definido.");
  }
  else {
    return fetch('/root/getcryptosecretkey')
      .then(response1 => {
        if (!response1.ok) throw new Error();
        return response1.text();
      }).then(secretKey => {
        return CryptoJS.AES.decrypt(data, secretKey).toString(CryptoJS.enc.Utf8);
      }).catch(error => {
        toastr.error("Não foi possível obter a chave secreta", "Erro!");
      });
  }
}

///Renderiza um conteúdo HTML em uma página e depois insere as tags <script> no final do corpo.
function renderHTML(html, containerId) {
  let scriptElement;
  let parser = new DOMParser();
  let doc = parser.parseFromString(html, 'text/html');
  let scriptTags = doc.querySelectorAll('script');

  document.getElementById(containerId).innerHTML = "";
  document.getElementById(containerId).innerHTML = html;

  for (var i = 0; i < scriptTags.length; i++) {
    scriptElement = document.createElement('script');
    scriptElement.src = scriptTags[i].src;
    document.body.appendChild(scriptElement);
  }
}

//Mostra os detalhes de determinada postagem
function postDetails(postId) {
  startSpinner();
  fetch(`/posts/postdetails?postId=${postId}`, {
    method: 'GET',
    headers: {
      'X-Requested-With': 'fetch'
    },
  }).then(response => {
    const url = response.url;
    if (!response.ok) {
      throw new Error();
    }
    return response.text().then(html => {
      return { html, url };
    });
  }).then(({ html, url }) => {
    renderHTML(html, "render-body");
    window.history.pushState(url, null, url);
    stopSpinner();
  }).catch(error => {
    console.error('Erro na requisição:');
    stopSpinner();
  });
}

//Mostra os detalhes de determinada postagem pelo id de um dos seus comentários
function postDetailsByCommentId(commentId) {
  fetch(`/comments/getpostidbycommentid?commentId=${commentId}`, {
    headers: {
      'X-Requested-With': 'fetch'
    },
  }).then(response => {
    if (!response.ok) {
      throw new Error();
    } else {
      return response.text()
    }
  }).then(postId => {
    if (postId != 0) {
      postDetails(postId);
    } else {
      toastr.error("A postagem em questão não existe mais, foi excluída.", "Erro!");
    }
  })
}

//Apaga uma postagem e pode ou não redirecionar para o grid caso necessário
function deletePost(args) {

  const { postId, redirectTo } = args;

  if (redirectTo === undefined) {
    $.ajax({
      type: "DELETE",
      data: {
        'postId': postId
      },
      url: "/posts/deletepost",
      dataType: "text",
      success: function (result) {
        toastr.success("A postagem foi apagada.", "Sucesso!");
      },
      error: function (result) {
        toastr.error("Erro", "Erro!");
      },
      complete: function () {
        $("#posts-grid").bootgrid("reload");
      }
    });
  } else if (redirectTo === "postGrid") {
    $.ajax({
      type: "DELETE",
      data: {
        'postId': postId
      },
      url: "/posts/deletepost",
      dataType: "text",
      success: function (result) {
        $.ajax({
          type: "GET",
          dataType: "html",
          url: "/posts/grid" + "?filter=" + sessionStorage.getItem("filterGrid"),
          beforeSend: function () {
            startSpinner();
          },
          success: function (response) {
            $("#render-body").empty();
            $("#render-body").html(response);
            window.history.pushState("/posts/grid?filter=" + sessionStorage.getItem("filterGrid"), null, "/posts/grid?filter=" + sessionStorage.getItem("filterGrid"));
            toastr.success("A postagem foi apagada.", "Sucesso!");
          },
          error: function () {
            toastr.error("Não foi possível voltar", "Erro!");
          },
          complete: function () {
            stopSpinner();
          }
        });
      },
      error: function (result) {
        toastr.error("Erro", "Erro!");
      },
      complete: function () {
        $("#posts-grid").bootgrid("reload");
      }
    });
  }
}