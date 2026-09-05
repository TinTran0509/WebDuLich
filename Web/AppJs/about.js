var pageIndex = 1;
var pageSize = 10;
var about = function () {
    return {
        init: function () {
            about.loadData();
        },
        loadData: function () {
            $.get("/Admin/About/ListData", { pageIndex: pageIndex}, function (res) {
                $('#gridData').html(res.viewContent);
            });
        },
        ondelete: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có chắc chắn không?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.POST("/Admin/About/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.IsSuccess(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
      
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                about.loadData(this.pageIndex);
                location.href = "/admin/about";
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
    
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                about.loadData(this.pageIndex);
                location.href = "/admin/about";
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
         
    };
}();
$(function () {
    about.init();
});
