var Booking = function () {
    return {
        init: function () { 
            Booking.loadData(1);
        },
        loadData: function (page) {
            let keySearch = $('#txtSearch').val();
            let status = 0;
            $("#loading").show();
            AjaxService.POST("/Admin/Home/ListData", { keySearch: keySearch, status: status, page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
        },  
        loadfrmProductDetail: function (id) {
            modal.Render("/Admin/Product/ProductDetail/" + id, "Chi tiết sản phẩm", "modal-lg");
        },
        loadfrmBookingDetail: function (id) {
            modal.Render("/Admin/Booking/Detail/" + id, "Chi tiết đặt hàng", "modal-lg");
        },
        ondelete: function (id) {
            alertmsg.error('Bạn không thể xóa đơn đặt hàng này');
            return false;
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
                    AjaxService.POST("/Admin/Country/Delete", { id: id }, function (res) {
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
        }, 
    };
}();
$(function () { Booking.init(); });


