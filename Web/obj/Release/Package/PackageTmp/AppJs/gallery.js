var Gallery = function () {
    return {
        init: function () {
            this.pageIndex = 1;
            this.FirstLoad = true;
            this.DataSearch = {};
        },
        loadData: function (pageindex) {
            var self = this;
            $("#loading").show();
            var NgayDang = $("#NgayDang").val();
            var NgayKet = $("#NgayKet").val();
            Gallery.DataSearch = { NgayDang: NgayDang, NgayKet: NgayKet }
            AjaxService.POST("/Admin/Gallery/ListData?page=" + pageindex, Gallery.DataSearch, function (res) {
                $("#gridData").html(res.viewContent);
                if (res.totalPages > 1) {
                    $('#paginationholder').html('');
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: self.pageIndex,
                        totalPages: res.totalPages,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            self.pageIndex = page;
                            Gallery.loadData(self.pageIndex);
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
            Gallery.DataSearch = {
                LangCode: $("#frmSearch #LangCode").val(),
                PageElementId: $("#frmSearch #PageElementId").val(),
                NgayDang: $("#NgayDang").val(),
                NgayKet: $("#NgayKet").val()
            };
            if (res.totalPages > 1) {
                $('#pagination').twbsPagination({
                    startPage: 1,
                    totalPages: res.totalPages,
                    visiblePages: 5,
                    onPageClick: function (event, page) {
                        Gallery.pageIndex = page;
                        Gallery.loadData(page);
                    }
                });
            }
            self.FirstLoad = false;
            $("#loading").hide();
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/Gallery/Add", "Thêm mới ảnh slide", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Gallery.loadData(this.pageIndex);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmedit: function (id) {
            modal.Render("/Admin/Gallery/Edit/" + id, "Cập nhật ảnh slide", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Gallery.loadData(this.pageIndex);
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
                    AjaxService.POST("/Admin/Gallery/ChangeStatus", { id: id }, function (res) {
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
                    AjaxService.POST("/Admin/Gallery/Delete", { id: id }, function (res) {
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
                alertmsg.error("Bạn cần chọn ít nhất một ảnh cần xóa");
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
                        AjaxService.POST("/Admin/Gallery/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
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
                var id = $(this).find("#item_ID").val();
                var ordering = $(this).find("input[type=text]").val();
                var str = id + ":" + ordering;
                arrValue.push(str);
            });
            var strValue = arrValue.join("|");
            AjaxService.POST("/Admin/Gallery/UpdatePosition", { value: strValue }, function (res) {
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
$(function () { Gallery.init(); });


