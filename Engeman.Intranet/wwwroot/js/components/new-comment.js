$('.tags').tagsInput({
  'height': 'auto',
  'width': '100%',
  'defaultText': '',
});

$("#tab_1_3").removeClass("active");

$("#comment-form").validate({
  rules: {
    description: {
      required: true
    }
  },
  ignore: []
});

richTextBox();

$("#comment-form").on("submit", function (event) {
  //ignora o submit padrão do formulário
  event.preventDefault();
  if ($("#comment-form").valid()) {
    //usado para receber além dos dados texto, o arquivo também
    var formData = new FormData(this);
    formData.append("postId", sessionStorage.getItem("postId"));
    //contentType e processData são obrigatórios
    $.ajax({
      type: "POST",
      url: "/posts/newcomment",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        if (response == -1) {
          toastr.error("Formulário inválido", "Erro!");
        } else {
          toastr.success("O comentário foi salvo", "Sucesso!");
          $.ajax({
            type: "POST",
            url: "/posts/backtolist",
            success: function (response) {
              $(".body-content").empty();
              $(".body-content").html(response);
            },
            error: function () {
              toastr.error("Não foi possível voltar", "Erro!");
            }
          });
        }
      },
      error: function (response) {
        toastr.error("O comentário não foi salvo", "Erro!");
      }
    })
  }
})

function richTextBox() {

  const { createEditor, createToolbar } = window.wangEditor

  wangEditor.i18nChangeLanguage("en");

  const editorConfig = {
    placeholder: 'Digite aqui',
    MENU_CONF: {
      uploadImage: {
        fieldName: 'your-fileName',
        base64LimitSize: 5 * 1024 * 1024 // 5M 以下插入 base64
      }
    },
    onChange(editor) {
      const html = editor.getHtml()

      if (html != "<p><br></p>") {
        $("#comment-description").val(html);
      } else {
        $("#comment-description").val("");
      }
      if (document.getElementById('preview')) {
        document.getElementById('preview').innerHTML = html
      }
      Prism.highlightAll();
    }
  }

  editorConfig.MENU_CONF['fontSize'] = {
    fontSizeList: ['10px', '12px', '20px', '30px', '50px']
  }

  editorConfig.MENU_CONF['fontFamily'] = {
    fontFamilyList: ['Arial', 'Times', 'Tahoma', 'Verdana', 'Sans-serif']
  }

  editorConfig.MENU_CONF['codeSelectLang'] = {
    codeLangs: [
      { text: 'CSS', value: 'css' },
      { text: 'HTML', value: 'html' },
      { text: 'C#', value: 'csharp' },
      { text: 'SQL', value: 'sql' },
      { text: 'JSON', value: 'json' },
      // others...
    ]
  }

  const toolbarConfig = {
    toolbarKeys: [
      'headerSelect',
      'fontSize',
      'fontFamily',
      'lineHeight',
      '|',
      'bold',
      'underline',
      'italic',
      'sup',
      'color',
      'bgColor',
      '|',
      'bulletedList',
      'numberedList',
      'todo',
      '|',
      'justifyLeft',
      'justifyCenter',
      'justifyRight',
      'justifyJustify',
      '|',
      'emotion',
      'insertLink',
      'uploadImage',
      'insertTable',
      'blockquote',
      'codeBlock',
      'divider',
      '|',
      'undo',
      'redo',
      'clearStyle',
    ]
  }

  const editor = createEditor({
    selector: '#editor-text-area',
    html: '<p><br></p>',
    config: editorConfig,
    mode: 'default', // or 'simple'
  })

  const toolbar = createToolbar({
    editor,
    selector: '#editor-toolbar',
    config: toolbarConfig,
    mode: 'default', // or 'simple'
  })
}