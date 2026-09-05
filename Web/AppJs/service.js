var service = function () {
    return {
        init: function () {
            var keyWord = $('#txtSearch').val();
            if (keyWord != undefined) {
                service.loadData(1);
            }
            $("#txtSearch").keypress(function (e) {
                if (e.which === 13) {
                    service.loadData(1);
                    return false;
                }
            });
        },
        loadData: function (pageIndex) { 
            var keyWord = $('#txtSearch').val();
            $.get("/Admin/service/ListData", { keyWord: keyWord, pageIndex: pageIndex}, function (res) {
                $('#loadData').html(res.viewContent);
                if (res.totalPages > 1) {
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: self.pageIndex,
                        totalPages: res.totalPages,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            self.pageIndex = page;
                            service.loadData(self.pageIndex);
                        }
                    });
                } else {
                    $('#paginationholder').html('');
                }
            });
        }, 
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                if (res.Close == "close") {
                    let url = "/admin/service";
                    location.href = url;
                } else {
                    location.href = "/admin/service/add";
                }
            }
            else {
                alertmsg.error(res.Messenger);
            }
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let url = "/admin/service";
                location.href = url;
            }
            else {
                alertmsg.error(res.Messenger);
            }
        },
        handleDeleteItem: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có chắc chắn xóa không?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.GET("/Admin/service/Delete", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        handlePublish: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có muốn đăng bài viết?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.GET("/Admin/service/Publish/", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        handleUnPublish: function (id) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có muốn hủy đăng bài viết?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.GET("/Admin/service/UnPublish/", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        handleViewDetail: function (id) {
            modal.Render("/Admin/service/Detail/"+ id, "Chi tiết tin tức", "modal-lg");
        }
    }
}();
$(function () {
    service.init();
})