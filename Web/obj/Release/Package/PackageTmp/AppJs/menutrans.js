var MenuTrans = function () {
    return {
        init: function () {
        },
        loadData: function (pageindex) {
            var self = this;
            $("#loading").show();
            AjaxService.POST("/Admin/MenuTrans/ListData", {page: pageindex}, function (res) {
                $("#gridData").html(res.viewContent);
               
                self.FirstLoad = false;
                $("#loading").hide();
            });
        },
        onSearchSuccess: function (res) {
            var self = this;
            $('#paginationholder').html('');
            $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
            $("#gridData").html(res.viewContent);
            MenuTrans.DataSearch = {
                Name: $("#frmSearch #Name").val(),
                LangCode: $("#frmSearch #LangCode").val(),
                PageElementId: $("#frmSearch #PageElementId").val()
            };
            if (res.totalPages > 1) {
                $('#pagination').twbsPagination({
                    startPage: 1,
                    totalPages: res.totalPages,
                    visiblePages: 5,
                    onPageClick: function (event, page) {
                        MenuTrans.pageIndex = page;
                        MenuTrans.loadData(page);
                    }
                });
            }
            self.FirstLoad = false;
            $("#loading").hide();
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/MenuTrans/Add", "Thêm mới MenuTrans", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                MenuTrans.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmedit: function (id) {
            modal.Render("/Admin/MenuTrans/Edit/" + id, "Cập nhật MenuTrans", "modal-lg");
        },
        onEditSuccess: function (res) {
            let page = $('.pagination .active a').text();
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                MenuTrans.loadData(parseInt(page));
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        active: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Thay đổi trạng thái?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.POST("/Admin/MenuTrans/EditComment", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
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
                    AjaxService.POST("/Admin/MenuTrans/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        onmultidelete: function () {
            var self = this;
            if ($("table tbody").find("input[type=checkbox]:checked").length == 0) {
                alertmsg.error("Bạn cần chọn ít nhất một MenuTrans cần xóa");
            } else {
                $("#loading").show();
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
                        AjaxService.POST("/Admin/MenuTrans/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
                            self.pageIndex = 1;
                            self.loadData(self.pageIndex);
                            alertmsg.success(res.Messenger);
                        });
                    }
                    $("#loading").hide();
                });
            }
        },
        onupdateposittion: function () {
            var self = this;
            $("#loading").show();
            var arrValue = [];
            $("table tbody tr").each(function () {
                var id = $(this).find(".item_ID").val();
                var ordering = $(this).find("input[type=text]").val();
                var str = id + ":" + ordering;
                arrValue.push(str);
            });
            var strValue = arrValue.join("|");
            AjaxService.POST("/Admin/MenuTrans/UpdatePosition", { value: strValue }, function (res) {
                if (res.IsSuccess == true) {
                    alertmsg.success(res.Messenger);
                    $("#gridData").html(res.ViewContent);
                } else {
                    alertmsg.error(res.Messenger);
                }
                self.loadData(self.pageIndex);
                $("#loading").hide();
            });
        }
    };
}();
$(function () { MenuTrans.init(); });


