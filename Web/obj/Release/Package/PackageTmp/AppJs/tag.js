var Tag = function () {
    return {
        init: function () {
            this.pageIndex = 1;
        },
        loadData: function (pageindex) {
            var self = this;
            $("#loading").show();
            AjaxService.POST("/Admin/Tag/ListData", {page: pageindex}, function (res) {
                $("#gridData").html(res.viewContent);
                if (res.totalPages > 1) {
                    $('#paginationholder').html('');
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: self.pageIndex,
                        totalPages: res.totalPages,
                        visiblePages:5,
                        onPageClick: function (event, page) {
                            self.pageIndex = page;
                            Tag.loadData(self.pageIndex);
                        }
                    });
                }
                self.FirstLoad = false;
                $("#loading").hide();
            });
        },
        onSearchSuccess: function (res) {
            var self = this;
            $('#paginationholder').html('');
            $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
            $("#gridData").html(res.viewContent);
            Tag.DataSearch = {
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
                        Tag.pageIndex = page;
                        Tag.loadData(page);
                    }
                });
            }
            self.FirstLoad = false;
            $("#loading").hide();
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/Tag/Add", "Thêm mới thẻ tag", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Tag.loadData(this.pageIndex);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmedit: function (id) {
            modal.Render("/Admin/Tag/Edit/" + id, "Cập nhật thẻ tag", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Tag.loadData(this.pageIndex);
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
                    AjaxService.POST("/Admin/Tag/EditComment", { id: id }, function (res) {
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
                    AjaxService.POST("/Admin/Tag/Delete", { id: id }, function (res) {
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
                alertmsg.error("Bạn cần chọn ít nhất một thẻ tag cần xóa");
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
                        AjaxService.POST("/Admin/Tag/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
                            self.pageIndex = 1;
                            self.loadData(self.pageIndex);
                            alertmsg.success(res.Messenger);
                        });
                    }
                    $("#loading").hide();
                });
            }
        }
    };
}();
$(function () { Tag.init(); });


