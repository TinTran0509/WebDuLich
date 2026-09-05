var Product = function () {
    return {
        init: function () { 
           
        },
        loadData: function (page) {
            $("#loading").show();
            AjaxService.POST("/Admin/Product/ListData", { page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
        },  
        loadfrmAdd: function () {
            modal.Render("/Admin/Product/Add", "Thêm mới", "modal-lg");
        }, 
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                window.location.href = "/admin/Product" 
                modal.hide();
            } else { 
                $('html, body').animate({ scrollTop: 0 }, 500);
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Product/Edit/" + id, "Cập nhật", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let page = $('.pagination .active a').text();
                $('.pagination .active a').trigger('click');
                window.location.href = "/admin/Product" 
               /* Product.loadData(parseInt(page));*/
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
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
                    AjaxService.POST("/Admin/Product/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        if (res.IsSuccess == true) {
                            alertmsg.success(res.Messenger); 
                        } else {
                            alertmsg.error(res.Messenger);
                        } 
                    });
                }
                $("#loading").hide();
            });
        }
    };
}();
$(function () { Product.init(); });


