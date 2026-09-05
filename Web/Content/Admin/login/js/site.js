function StartLoadingScreen() {
  $('body').loading({zIndex: 999999, message: 'System in progress...'});
}

function EndLoadingScreen() {
  $('body').loading('stop');
}

function StartLoadingElement(element) {
  $(element).loading({zIndex: 999999, message: 'System in progress...'});
}

function EndLoadingElement(element) {
  $(element).loading('stop');
}

function PopoverImage() {
  $('span[data-toggle=popover-image]').popover({
    html: true,
    trigger: 'hover',
    content: function () {
      return '<img class="img-responsive img-rounded" src="' + $(this).data('img') + '"  alt=""/>';
    }
  });
}

function GetDateRangePickerSetup(start, end) {
  return {
    format: 'MM/DD/YYYY',
    startDate: moment(start),
    endDate: moment(end),
    showDropdowns: true,
    showWeekNumbers: true,
    timePicker: false,
    timePickerIncrement: 1,
    timePicker12Hour: true,
    ranges: {
      'Today': [moment(), moment()],
      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
      'Last 7 Days': [moment().subtract(6, 'days'), moment()],
      'Last 30 Days': [moment().subtract(29, 'days'), moment()],
      'This Month': [moment().startOf('month'), moment().endOf('month')],
      'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
    },
    opens: 'right',
    drops: 'down',
    buttonClasses: ['btn', 'btn-sm'],
    applyClass: 'btn-primary',
    cancelClass: 'btn-default'
  };
}

function GetSingleDatePickerSetup(date) {
  return {
    format: 'MM/DD/YYYY',
    startDate: date === '' ? moment() : moment(date),
    singleDatePicker: true,
    timePicker: false,
    showDropdowns: true,
    dayHighlight: true,
    clearBtn: true,
    autoApply: true,
    drops: "auto"
  };
}

function GetDataTableSetup(config) {
  let pageLength = 10;
  let footerCallback = function () {
  };
  if (config !== undefined) {
    if (config.pageLength !== undefined && config.pageLength !== null) pageLength = config.pageLength;
    if (config.footerCallback !== undefined && config.footerCallback !== null) footerCallback = config.footerCallback;
  }

  return {
    colReorder: true,
    responsive: {
      details: {
        renderer: function (api, rowIdx, columns) {
          let count = 1;
          let data = $.map(columns, function (col, i) {
            console.log(col.data);
            return col.hidden && !col.data.includes("<input") ?
              '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
              '<td colspan="2" style="' + (count++ === 1 ? 'border-top: 0' : '') + '">' +
              '<dl class="dl-horizontal" style="margin-bottom: 0 !important;">' +
              '<dt>' + col.title + ' :</dt>' +
              '<dd>' + col.data + '</dd>' +
              '</dl>' +
              '</td>' +
              '</tr>' :
              '';
          }).join('');
          return data ? $('<table/>').append(data).addClass('table') : false;
        }
      }
    },
    order: [],
    lengthMenu: [[10, 20, 50, 100, 150, 200, -1], [10, 20, 50, 100, 150, 200, "All"]],
    pageLength: pageLength,
    dom:
      "<'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r>" +
      "<'hr-line-dashed'>" +
      "<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>" +
      "<'table-scrollable't>" +
      "<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
    columnDefs: [{}]
  };
}

function GetDataTableSimpleSetup(config) {
  let pageLength = 20;
  let footerCallback = function () {
  };
  if (config !== undefined) {
    if (config.pageLength !== undefined && config.pageLength !== null) pageLength = config.pageLength;
    if (config.footerCallback !== undefined && config.footerCallback !== null) footerCallback = config.footerCallback;
  }

  return {
    colReorder: true,
    responsive: {
      details: {
        renderer: function (api, rowIdx, columns) {
          let count = 1;
          let data = $.map(columns, function (col, i) {
            return col.hidden && !col.data.includes("<input") ?
              '<tr data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
              '<td colspan="2" style="' + (count++ === 1 ? 'border-top: 0' : '') + '">' +
              '<dl class="dl-horizontal" style="margin-bottom: 0 !important;">' +
              '<dt>' + col.title + ' :</dt>' +
              '<dd>' + col.data + '</dd>' +
              '</dl>' +
              '</td>' +
              '</tr>' :
              '';
          }).join('');
          return data ? $('<table/>').append(data).addClass('table') : false;
        }
      }
    },
    lengthMenu: [[10, 20, 50, 100, 150, 200, -1], [10, 20, 50, 100, 150, 200, "All"]],
    pageLength: pageLength,
    dom: "<'table-scrollable't>"
  };
}

function GetBootstrapMultiSelectSetup() {
  return {
    nonSelectedText: 'Select...',
    allSelectedText: 'All',
    nSelectedText: ' - Selected',
    selectAllText: 'All',
    enableClickableOptGroups: true,
    enableCollapsibleOptGroups: true,
    enableFiltering: true,
    includeSelectAllOption: true,
    maxHeight: 140,
    buttonWidth: '100%',
    dropUp: false
  };
}


function JqueryConfirm(content, funcConfirm, funcCancel) {
  $.confirm({
    title: 'Are you sure ?',
    content: "<div style='font-size: 14px'>" + content + "<br/>" + "</div>",
    icon: 'fa fa-question',
    theme: 'material',
    animation: 'scale',
    type: 'red',
    columnClass: 'xlarge',
    draggable: true,
    buttons: {
      confirm: {
        text: 'Agree !',
        btnClass: 'btn-danger',
        keys: ['enter', 'space'],
        action: funcConfirm
      },
      cancel: {
        text: 'Cancel',
        btnClass: 'btn-default',
        keys: ['esc'],
        action: funcCancel
      }
    }
  });
}

function JqueryConfirmPrompt(title, content, confirmFunc, cancelFunc, onOpenFunc) {
  $.confirm({
    title: title,
    content: "<div style='font-size: 14px'>" + content + "<br/>" + "</div>",
    icon: 'fa fa-question',
    theme: 'material',
    animation: 'scale',
    type: 'red',
    columnClass: 'xlarge',
    draggable: true,
    buttons: {
      formSubmit: {
        text: 'Agree !',
        btnClass: 'btn-danger',
        keys: ['enter', 'space'],
        action: confirmFunc
      },
      cancel: {
        text: 'Cancel',
        btnClass: 'btn-default',
        keys: ['esc'],
        action: cancelFunc === undefined ? () => {
        } : cancelFunc
      }
    },
    onOpen: onOpenFunc === undefined ? () => {
    } : onOpenFunc
  });
}

function JqueryAlert(title, content, typeStatus) {
  if (typeStatus) {
    toastr.success(content, title);
    toastr.options = {
      "closeButton": true,
      "debug": false,
      "progressBar": true,
      "preventDuplicates": false,
      "positionClass": "toast-bottom-right",
      "onclick": null,
      "showDuration": "400",
      "hideDuration": "1000",
      "timeOut": "9000",
      "extendedTimeOut": "1000",
      "showEasing": "swing",
      "hideEasing": "linear",
      "showMethod": "fadeIn",
      "hideMethod": "fadeOut"
    };
  } else {
    $.confirm({
      title: title,
      content: "<div style='font-size: 14px'>" + content + "<br/>" + "</div>",
      type: typeStatus ? 'green' : 'red',
      icon: 'fa fa-warning',
      theme: 'material',
      animation: 'scale',
      typeAnimated: true,
      columnClass: 'xlarge',
      draggable: true,
      buttons: {
        tryAgain: {
          text: 'Try again',
          btnClass: 'btn-red',
          action: function () {
          }
        },
      }
    });

    EndLoadingScreen();
  }
}

function JqueryNotification(title, content, style) {
  toastr[style](content, title);
  toastr.options = {
    "closeButton": true,
    "debug": false,
    "progressBar": true,
    "preventDuplicates": true,
    "positionClass": "toast-bottom-right",
    "onclick": null,
    "showDuration": "400",
    "hideDuration": "1000",
    "timeOut": "105000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
  };
}

function JqueryGetJSON(url, data, callback) {
  StartLoadingScreen();
  $.getJSON(url, data, callback)
    .success(() => EndLoadingScreen())
    .complete(() => EndLoadingScreen())
    .error((jqXHR, textStatus) => {
      JqueryAlert("Error !", textStatus, false);
      EndLoadingScreen();
    })
    .always(() => EndLoadingScreen());
}

function JqueryPostJSON(url, data, callback) {
  StartLoadingScreen();
  $.ajax({
    type: "POST",
    url: url,
    data: data,
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: callback,
    failure: (response) => {
      JqueryAlert("Error !", response.responseText, false);
      EndLoadingScreen();
    },
    error: (response) => {
      JqueryAlert("Error !", response.responseText, false);
      EndLoadingScreen();
    }
  }).always(() => EndLoadingScreen());
}

function JqueryGet(url, data, callback) {
  StartLoadingScreen();
  $.get(url, data, callback)
    .done(() => EndLoadingScreen())
    .fail(() => EndLoadingScreen())
    .always(() => EndLoadingScreen());
}

function JqueryPost(url, data, callback) {
  StartLoadingScreen();
  $.ajax({
    type: "POST",
    url: url,
    data: data,
    contentType: "application/json; charset=utf-8",
    dataType: "html",
    success: callback,
    failure: (response) => {
      JqueryAlert("Error !", response.responseText, false);
      EndLoadingScreen();
    },
    error: (response) => {
      JqueryAlert("Error !", response.responseText, false);
      EndLoadingScreen();
    }
  }).always(() => EndLoadingScreen());
}

function GetDataTablePagingSetup(configs) {
  return {
    // language: { url: "/libs/dataTables/i18n/Vietnamese.json" },
    colReorder: true,
    responsive: {
      details: {
        renderer: function (api, rowIdx, columns) {
          let count = 1;
          let data = $.map(columns, function (col, i) {
            console.log(col.data);
            return col.hidden
              ? '<tr data-dt-row="' +
              col.rowIndex +
              '" data-dt-column="' +
              col.columnIndex +
              '">' +
              '<td colspan="2" style="' +
              (count++ === 1 ? "border-top: 0" : "") +
              '">' +
              '<dl class="dl-horizontal" style="margin-bottom: 0 !important;">' +
              '<dt class="col-sm-3">' +
              col.title +
              " :</dt>" +
              '<dd class="col-sm-9">' +
              col.data +
              "</dd>" +
              "</dl>" +
              "</td>" +
              "</tr>"
              : "";
          }).join("");
          return data ? $("<table/>").append(data).addClass("table") : false;
        },
      },
    },
    fixedHeader: {
      header: true,
      headerOffset: $("#fixed").height(),
      footer: true,
    },
    lengthMenu: [
      [10, 20, 50, 100, 150, 200],
      [10, 20, 50, 100, 150, 200],
    ],
    pageLength: 10,
    dom:
      "<'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r>" +
      "<'hr-line-dashed'>" +
      "<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>" +
      "<'table-scrollable't>" +
      "<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
    processing: true,
    serverSide: true,
    ajax: configs.ajax,
    ordering: configs.ordering,
    columns: configs.columns
  };
}