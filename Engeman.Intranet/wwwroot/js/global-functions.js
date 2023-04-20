function RemoveHTMLTags(value) {
  return value.replace(/(<p.*?>|<\/p>)|(<h\d>|<\/h\d>)|(<span.*?>|<\/span>)|(style=".*?")|(<strong>|<\/strong>)|(<em>|<\/em>)|(<sup>|<\/sup>)|(<br>)|(&nbsp;)|(\s+)/g, '');
}