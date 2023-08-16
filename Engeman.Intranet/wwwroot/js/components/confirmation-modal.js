
function showConfirmationModal(confirmCallback, callbackArguments) {

  let header;
  let body;

  if (confirmCallback.name === "deletePost") {
    header = "<h5>Apagar a postagem?</h5>";
    body = '<p>Se houver quaisquer arquivos associados à postagem, <br> eles também serão excluídos</p>';
  } else if (confirmCallback.name === "deleteTerm") {
    header = "<h5>Apagar o termo?</h5>";
    body = '<p>Esta operação não pode ser desfeita</p>';
  } else if (confirmCallback.name === "aproveComment") {
    header = "<h5>Aprovar o comentário?</h5>";
    body = '<p>Esta ação não poderá ser revertida.</p>';
  }
  else if (confirmCallback.name === "deleteComment") {
    header = "<h5>Apagar o comentário?</h5>";
    body = '<p>Se houver quaisquer arquivos associados ao comentário, eles também serão excluídos.</p>';
  }
  else if (confirmCallback.name === "deleteKeyword") {
    header = "<h5>Apagar a palavra-chave?</h5>";
    body = '<p>Esta operação não pode ser desfeita</p>';
  }
  else if (confirmCallback.name === "aprovePost") {
    header = "<h5>Aprovar a postagem?</h5>";
    body = '<p>Esta ação não poderá ser revertida.</p>';
  }
  else if (confirmCallback.name === "deleteUser") {
    header = "<h5>Apagar o usuário?</h5>";
    body = '<p>Se houver quaisquer postagens ou comentários associados a este usuário, também serão excluídos</p>';
  }

  document.getElementById('confirmation-modal').querySelector(".modal-header").innerHTML = header
  document.getElementById('confirmation-modal').querySelector(".modal-body").innerHTML = body

  // Define a ação a ser executada quando o botão de confirmação for clicado
  document.getElementById('confirm-button').onclick = function () {
    confirmCallback(callbackArguments);
    $('#confirmation-modal').modal('hide');
  };

  $('#confirmation-modal').modal('show');
}