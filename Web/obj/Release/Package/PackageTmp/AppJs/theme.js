var Theme = function () {
    return {
        init: function () { 
            Theme.loadData(1);
        },
        loadData: function (page) {
            $("#loading").show();
            AjaxService.POST("/Admin/Theme/ListData", { page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
        },  
        loadfrmAdd: function () {
            modal.Render("/Admin/Theme/Add", "Thêm mới chủ đề", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                Theme.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        }, 
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Theme/Edit/" + id, "Cập nhật chủ đề", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let page = $('.pagination .active a').text();
                $('.pagination .active a').trigger('click');
                Theme.loadData(1);
               /* Theme.loadData(parseInt(page));*/
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
                    AjaxService.POST("/Admin/Theme/Delete", { id: id }, function (res) {
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
            $.get("/Admin/Theme/GetMenuTransByLangCode", { langCode: langCode }, function (res) {
                let option = '';
                res.Data.forEach((item, index, arr) => {
                    option += `<option value="${item.LinkSeo}">${item.Name}</option>`;
                });
                 
                $('#slMenuTrans').html(option)
            }); 
        },
        loadfrmTopicTrans: function (id, name) { 
            modal.Render("/Admin/Theme/Translate/" + id, "Translate ", "modal-lg");
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
                    AjaxService.POST("/Admin/Theme/DeleteTopicTrans", { id: id }, function (res) {
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
        },
        getMenuTransByLang: function (langCode) {
            $("#loading").show();
            $.get("/Admin/Theme/GetMenuTransByLangCode", { langCode: langCode }, function (res) {
                let option = '<option value="">--- Chọn ---</option>';
                res.Data.forEach((item, index, arr) => {
                    option += `<option value="${item.LinkSeo}">${item.Name}</option>`;
                });

                $('#slMenuHome').html(option);
                $("#loading").hide();
            });
        },
    };
}();
$(function () { Theme.init(); });


