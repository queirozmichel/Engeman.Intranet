$(document).ready(function () {

})

$.ajax({
  type: "GET",
  url: "/dashboard/postscommentschart",
  dataType: "json",
  success: function (response) {
    let dataLabel = [];
    let dataPosts = [];
    let dataComments = [];
    response.forEach(function (obj) {
      dataLabel.push(obj.label);
      dataPosts.push(obj.posts);
      dataComments.push(obj.comments);
    });
    new Chart($("#posts-comments-chart"), {
      type: 'bar',
      data: {
        labels: dataLabel,
        datasets: [{
          label: 'postagens',
          data: dataPosts,
          borderWidth: 1,
          borderColor: 'brown',
          backgroundColor: 'brown',
        },
        {
          label: 'comentários',
          data: dataComments,
          borderWidth: 1,
          borderColor: '#6dadbd',
          backgroundColor: '#6dadbd'
        }]
      },
      options: {
        maintainAspectRatio: false,
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              precision: 0
            }
          }
        }
      }
    });
  },
  erro: function (data) {
    toastr.error("Não foi possível concluir a solicitação.", "Erro!");
  }
})