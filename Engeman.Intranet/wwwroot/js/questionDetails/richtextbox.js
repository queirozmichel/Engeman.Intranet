function richTextBox() {

  const { createEditor, createToolbar } = window.wangEditor

  wangEditor.i18nChangeLanguage("en");

  const editorConfig = {
    placeholder: 'Digite aqui',
    MENU_CONF: {
      uploadImage: {
        fieldName: 'your-fileName',
        base64LimitSize: 5 * 1024 * 1024 // 10M 以下插入 base64
      }
    },
    onChange(editor) {
      var aux = editor.getHtml()
      const selectionText = editor.getSelectionText()
      document.getElementById('selected-length').innerHTML = selectionText.length
      const text = editor.getText().replace(/\n|\r/mg, '')
      document.getElementById('total-length').innerHTML = text.length

      if (aux != "<p><br></p>") {
        $("#description").val(aux);
      } else {
        $("#description").val("");
      }
    }
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
      'indent',
      'delIndent',
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