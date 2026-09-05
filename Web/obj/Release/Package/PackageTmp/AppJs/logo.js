var Logo = function () {
    return {
        init: function () {
            Logo.loadData();
        },
        loadData: function () {
            var self = this;
            $("#loading").show();
            AjaxService.POST("/Admin/Logo/ListData", null, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();

            });
        },
       
        loadfrmAdd: function () {
            modal.Render("/Admin/Logo/Add", "Thêm mới logo", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Logo.loadData();
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Logo/Edit/" + id, "Cập nhật logo", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Logo.loadData();
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        active: function (id, status) {
            var active = 0;
            var titleActive = 'Vô hiệu';
            if (status !="Vô hiệu") {
                active = 1;
                titleActive = "Kích hoạt";
            }
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có muốn "+titleActive+" không?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.POST("/Admin/Logo/Active", { id: id, active: active }, function (res) {
                        Logo.loadData();
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
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
                    AjaxService.POST("/Admin/Logo/Delete", { id: id }, function (res) {
                        Logo.loadData();
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        }
    };
}();
$(function () { Logo.init(); });
