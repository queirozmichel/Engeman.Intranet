function startSpinner() {
  $('body').loadingModal({
    color: '#fff',
    opacity: '0.5',
    backgroundColor: 'rgb(0,0,0)',
    animation: 'circle',
  });
}

function closeSpinner() {
  $('body').loadingModal('hide');
}