function startSpinner() {
  EasyLoading.show({
    type: EasyLoading.TYPE.BALL_SPIN_FADE_LOADER,
    background_color: "rgba(0, 0, 0, .5)",
  });
}

function closeSpinner() {
  EasyLoading.hide();
}