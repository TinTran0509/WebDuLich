var Topic = function () {
    return {
        init: function () { 
            Topic.loadData(1);
        },
        loadData: function (page) {
            $("#loading").show();
            AjaxService.POST("/Admin/Topic/ListData", { page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
        },  
        loadfrmAdd: function () {
            modal.Render("/Admin/Topic/Add", "Thêm mới chủ đề", "modal-lg");
        },
        loadfrmAddTrans: function () {
            modal.Render("/Admin/Topic/AddTrans", "Thêm mới bản dịch", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Topic.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        onAddTopicTransSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                window.location.href ="/admin/topic" 
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Topic/Edit/" + id, "Cập nhật chủ đề", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let page = $('.pagination .active a').text();
                $('.pagination .active a').trigger('click');
                Topic.loadData(1);
               /* Topic.loadData(parseInt(page));*/
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        }, 
        onEditTopicTransSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                 
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
                    AjaxService.POST("/Admin/Topic/Delete", { id: id }, function (res) {
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
        getMenuTransByLang: function (langCode) {
            $.get("/Admin/Topic/GetMenuTransByLangCode", { langCode: langCode }, function (res) {
                let option = '';
                res.Data.forEach((item, index, arr) => {
                    option += `<option value="${item.LinkSeo}">${item.Name}</option>`;
                });
                 
                $('#slMenuTrans').html(option)
            }); 
        },
        loadfrmTopicTrans: function (id, name) { 
            modal.Render("/Admin/Topic/Translate/" + id, "Translate ", "modal-lg");
        },
        deleteTopicTrans: function (id, btn) {
            $("#loading").show();
            var self = this;
            swal({
                title: "Bạn có muốn xóa?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Có",
                cancelButtonText: "không",
            }, function (isConfirm) {
                if (isConfirm) {
                    AjaxService.POST("/Admin/Topic/DeleteTopicTrans", { id: id }, function (res) {
                        if (res.IsSuccess) {
                            $(btn).closest('tr').remove();
                            alertmsg.success(res.Messenger);
                        }
                        else
                            alertmsg.error(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        }
    };
}();
$(function () { Topic.init(); });


