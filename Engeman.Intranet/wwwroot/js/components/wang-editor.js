﻿$(document).ready(function () {

  richTextBox();

});

function richTextBox() {

  const { createEditor, createToolbar } = window.wangEditor

  wangEditor.i18nChangeLanguage("en");  

  var wangEditors = $(".wang-editor");

  $(wangEditors).each(function () {
    var toolbarId = $(this).find(".wang-editor-toolbar").attr("id");
    var editorId = $(this).find(".wang-editor-editor").attr("id");
    var editorDescription = $(this).find(".wang-editor-description").attr("id");
    var editorPreview = $(this).find(".editor-content-view").attr("id");
    var editor = createE(editorId, editorDescription, editorPreview);
    createT(toolbarId, editor);  
  })

  function createE(editorId, editorDescription, editorPreview) {

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
          $('#' + editorDescription).val(html);
        } else {
          $('#' + editorDescription).val("");
        }
        if (document.getElementById(editorPreview)) {
          document.getElementById(editorPreview).innerHTML = html
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

    const editor = createEditor({
      selector: '#' + editorId,
      html: '<p><br></p>',
      config: editorConfig,
      mode: 'default'
    })

    return editor;
  }

  function createT(toolbarId, editor) {
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

    const toolbar = createToolbar({
      editor: editor,
      selector: '#' + toolbarId,
      config: toolbarConfig,
      mode: 'default'
    })
  }
}