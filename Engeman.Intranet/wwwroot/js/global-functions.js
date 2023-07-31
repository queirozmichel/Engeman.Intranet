//Remove as Tags HTML
function RemoveHTMLTags(value) {
  return value.replace(/(<p.*?>|<\/p>)|(<h\d>|<\/h\d>)|(<span.*?>|<\/span>)|(style=".*?")|(<strong>|<\/strong>)|(<em>|<\/em>)|(<sup>|<\/sup>)|(<br>)|(&nbsp;)|(\s+)/g, '');
}

//Criptografa uma cadeia de caracteres utlizando o algoritimo AES
function EncryptData(data) {
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
function DecryptData(data) {
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