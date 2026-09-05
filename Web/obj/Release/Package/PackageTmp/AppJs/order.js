var pageIndex = 1;
var Order = function () {
    return {
        init: function () {
            this.FirstLoad = true; 
            Order.loadData();
            $('#customerName,#customerPhone').keypress(function (e) {
                if (e.which === 13) {
                    Order.loadData();
                    return false;
                }
            });
            $('#denNgay').keypress(function (e) {
                if (e.which === 13) {
                    Order.loadData();
                    return false;
                }
            });
        },
        loadData: function () {
            var self = this;
            if (this.FirstLoad) {
                $("#loading").show();
            }
            let name = $('#customerName').val().trim();
            let phone = $('#customerPhone').val().trim();
            let status = $('#status').val();
            let tungay = '';
            //let tungay = $('#tuNgay').val();
            //if (tungay != '') {
            //    let arr = tungay.split('/');
            //    tungay = arr.reverse().join('/');
            //}
            //let denngay = $('#denNgay').val();
            //if (denngay != '') {
            //    let arr = denngay.split('/');
            //    denngay = arr.reverse().join('/');
            //}
            let denngay = '';
            AjaxService.POST("/Admin/Order/ListData", { status: status, name: name, phone: phone,tungay:tungay,denngay:denngay, page: pageIndex }, function (res) {
                $("#gridData").html(res.viewContent);
                if (res.totalPages > 1) {
                    $('#paginationholder').html('');
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: self.pageIndex,
                        totalPages: res.totalPages,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            Order.pageIndex = page;
                            Order.loadData(page);
                        }
                    });
                }
                self.FirstLoad = false;
                $("#loading").hide();
            });
        },
        validateDate: function (value) {
            let fromDate = $('#tuNgay').val();
            if (fromDate.trim() == ''){
                alertmsg.error("Vui lòng chọn ngày bắt đầu");
                return false;
            }
            if(fromDate.trim() != '' && value.trim()!=''){
                fromDate = fromDate.split('/').reverse().join('');
                value = value.split('/').reverse().join('');
                if (parseInt(value) < parseInt(fromDate)) {
                    alertmsg.error("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
                    $('#denNgay').val('');
                }
            }
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Order.loadData(this.pageIndex);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Order/Edit/" + id, "Cập nhật trạng thái đơn hàng", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Order.loadData(this.pageIndex);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        onChangePassSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Order.loadData(this.pageIndex);
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
                    AjaxService.POST("/Admin/Order/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        viewDetail: function (id) {
            modal.Render("/Admin/Order/Detail?id="+ id, "", "modal-lg");
        }
    };
}();
$(function () { Order.init(); });


