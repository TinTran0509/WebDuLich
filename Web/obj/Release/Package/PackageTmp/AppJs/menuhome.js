 var MenuHome = function () {
    return {
        init: function () { 
            MenuHome.loadData(1);
        },
        loadData: function (page) {
            $("#loading").show();
            let langCode = $('#slLangCode').val();
            AjaxService.POST("/Admin/MenuHome/ListData", { langCode: langCode, page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
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
            AjaxService.POST("/Admin/MenuHome/UpdatePosition", { value: strValue }, function (res) {
                if (res.IsSuccess == true) {
                    alertmsg.success(res.Messenger);
                    $("#gridData").html(res.ViewContent);
                    displayadminmenu();
                } else {
                    alertmsg.error(res.Messenger);
                }
                self.loadData(self.pageIndex);
                $("#loading").hide();
            });
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
                    AjaxService.POST("/Admin/MenuHome/ChangeStatus", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/MenuHome/Add", "Thêm mới danh mục", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                MenuHome.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/MenuHome/Edit/" + id, "Cập nhật danh mục", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let page = $('.pagination .active a').text();
                $('.pagination .active a').trigger('click');
                MenuHome.loadData(parseInt(page));
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
                    AjaxService.POST("/Admin/MenuHome/Delete", { id: id }, function (res) {
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
        onmultidelete: function () {
            var self = this;
            if ($("table tbody").find("input[type=checkbox]:checked").length == 0) {
                alertmsg.error("Bạn cần chọn ít nhất danh mục dùng cần xóa");
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
                        AjaxService.POST("/Admin/MenuHome/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
                            self.pageIndex = 1;
                            self.loadData(self.pageIndex);
                            alertmsg.success(res.Messenger);
                        });
                    }
                    $("#loading").hide();
                });
            }
        },
        initcheckall: function () {
            var countall = $(".chkelement").length;
            var countchecked = 0;
            $(".chkelement").each(function () {
                if ($(this).find($("input[type=checkbox]")).is(':checked')) {
                    countchecked++;
                }
            });
            if (countall == countchecked) {
                $("#basicForm #chkall").prop('checked', true);
            } else {
                $("#basicForm #chkall").prop('checked', false);
            }
            $("#basicForm #chkall").click(function () {
                if ($(this).is(':checked')) {
                    $(".chkelement").each(function () {
                        $(this).find($("input[type=checkbox]")).prop('checked', true);
                    });
                } else {
                    $(".chkelement").each(function () {
                        $(this).find($("input[type=checkbox]")).prop('checked', false);
                    });
                }
            });
            $(".chkelement").click(function () {
                countchecked = 0;
                $(".chkelement").each(function () {
                    if ($(this).find($("input[type=checkbox]")).is(':checked')) {
                        countchecked++;
                    }
                });
                if (countall == countchecked) {
                    $("#basicForm #chkall").prop('checked', true);
                } else {
                    $("#basicForm #chkall").prop('checked', false);
                }
            });
        },
        getMenuByLangCode: function (langCode) {
            $.get("/Admin/MenuHome/GetMenuByLangCode", { langCode: langCode }, function (res) {
                let option = '<option value="">--- Chọn ---</option>';
                res.Data.forEach((item, index, arr) => {
                    option += `<option value="${item.ID}">${item.Name}</option>`;
                });
                 
                $('#selParentId').html(option)
            }); 
        }
    };
}();
$(function () { MenuHome.init(); });


