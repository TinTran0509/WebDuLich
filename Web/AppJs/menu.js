var Menu = function () {
    return {
        init: function () {
            
        },
        loadData: function (pageindex) {
            var self = this;
            let langCode = $('.tab-buttons').find('.active').data('langcode');
            $("#loading").show();
            AjaxService.POST("/Admin/Menu/ListData", { langCode: langCode, page: pageindex}, function (res) {
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
            Menu.DataSearch = {
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
                        Menu.pageIndex = page;
                        Menu.loadData(page);
                    }
                });
            }
            self.FirstLoad = false;
            $("#loading").hide();
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/Menu/Add", "Thêm mới menu", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                location.href = "/admin/menu";
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmedit: function (id) {
            modal.Render("/Admin/Menu/Edit/" + id, "Cập nhật menu", "modal-lg");
        }, 
        saveRowTrans: function (btn) {
            let name = $('#tbl-trans input').val();
            let langcode = $('#tbl-trans select').val();
            let language = $('#tbl-trans select option:selected').text();
            if (name != undefined && name.trim() === '') {
                alertmsg.error('Vui lòng nhập tên menu');
            }
            let id = sessionStorage.getItem('MENUID');
            let linkseo = RemoveMarkStr(name);
            let MenuTrans = {
                Name : name,
                LangCode : langcode,
                MenuID : id,
                LinkSeo : linkseo
            }
            let menutrans = JSON.stringify(MenuTrans);
            AjaxService.POST("/Admin/Menu/SaveTrans", { menutrans: menutrans }, function (res) {
                if (res.IsSuccess) {
                    let row = `<tr>
                                <td style="vertical-align: middle;width:65%">
                                   ${name}
                                </td>
                                <td style="vertical-align: middle;">${language}</td>
                                <td style="text-align: right;"> 
                                    <a style="color:red" href="javascript:void(0)" class="btn-bordered" onclick="Menu.deleteMenuTrans(${res.Id}, this) " title="Xóa">
                                        Xóa
                                    </a>
                                </td>
                            </tr>`;
                    $(btn).closest('tr').before(row);
                    $('#tbl-trans input').val('');
                    $(btn).css('display', 'none');
                    $("#tbl-trans select").prop("selectedIndex", 0);
                    alertmsg.success(res.Messenger);
                }   
                else
                    alertmsg.error(res.Messenger);
            });
        },
        onEditSuccess: function (res) { 
            let page = $('.pagination .active a').text();
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                location.href = "/admin/menu";
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
                    AjaxService.POST("/Admin/Menu/EditComment", { id: id }, function (res) {
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
                    AjaxService.POST("/Admin/Menu/Delete", { id: id }, function (res) {
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
                alertmsg.error("Bạn cần chọn ít nhất một menu cần xóa");
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
                        AjaxService.POST("/Admin/Menu/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
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
            AjaxService.POST("/Admin/Menu/UpdatePosition", { value: strValue }, function (res) {
                if (res.IsSuccess == true) {
                    alertmsg.success(res.Messenger);
                    $("#gridData").html(res.ViewContent);
                } else {
                    alertmsg.error(res.Messenger);
                }
                self.loadData(self.pageIndex);
                $("#loading").hide();
            });
        },
        deleteMenuTrans: function (id, btn) {
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
                    AjaxService.POST("/Admin/Menu/DeleteMenuTrans", { id: id }, function (res) {
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
            $.get("/Admin/Menu/GetMenuTransByLangCode", { langCode: langCode }, function (res) {
                let option = '<option data-parent="0" value="0">--- Chọn ---</option>';
                res.Data.forEach((item, index, arr) => {
                    option += `<option data-parent="${item.ParentID}" value="${item.ID}">${item.Name}</option>`;
                });

                $("select[name='MenuTrans']").html(option); 
                $("#loading").hide();
            });
        },
    };
}();
$(function () { Menu.init(); });


