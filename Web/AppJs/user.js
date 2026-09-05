var User = function () {
    return {
        init: function () { 
            User.loadData(1);
        },
        loadData: function (page) {
            $("#loading").show();
            AjaxService.POST("/Admin/User/ListData", { page: page}, function (res) {
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
            AjaxService.POST("/Admin/User/UpdatePosition", { value: strValue }, function (res) {
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
                    AjaxService.POST("/Admin/User/ChangeStatus", { id: id }, function (res) {
                        self.pageIndex = 1;
                        self.loadData(self.pageIndex);
                        alertmsg.success(res.Messenger);
                    });
                }
                $("#loading").hide();
            });
        },
        loadfrmAdd: function () {
            modal.Render("/Admin/User/Add", "Thêm mới người hỗ trợ", "modal-lg");
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                User.loadData(1);
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/User/Edit/" + id, "Cập nhật người hỗ trợ", "modal-lg");
        },
        loadfrmUserTrans: function (id) {
            sessionStorage.setItem('USERID', id);
            modal.Render("/Admin/User/Translate/" + id, "Thông tin", "modal-lg");
        },
        addRowTrans: function () {
            if ($('#tbl-trans input').length) {
                return false;
            }
            $.get("/Admin/User/GetLanguages", {}, function (res) {
                let option = '';
                res.Data.forEach((item, index, arr) => {
                    option += `<option value="${item.LangCode}">${item.LangName}</option>`;
                });
                let htmlSelect = `<select class="form-control">
                                     ${option}
                                  </select>`;
                let rowAdd = `<tr> 
                                <td style="vertical-align: middle;width:65%">
                                   <input onkeyup='User.showBtnAddRow(this)' class="form-control row-add" type="text"/>
                                </td>
                                 <td style="vertical-align: middle;">
                                    ${htmlSelect}
                                </td>
                                <td>
                                   <button style="float: right;display:none" type="button" class="btn btn-primary waves-effect waves-light btn-save-row" onclick="User.saveRowTrans(this)">Lưu</button>
                                </td>
                            </tr>`;
                $('.row-add').focus();
                $('#tbl-trans').append(rowAdd)
            });
        },
        showBtnAddRow: function (btn) {
            if ($(btn).val().trim() !== '') {
                $('.btn-save-row').css('display', 'block');
            }
            else {
                $('.btn-save-row').css('display', 'none');
            }
        },
        saveRowTrans: function (btn) {
            let description = $('#tbl-trans input').val();
            let langcode = $('#tbl-trans select').val();
            let language = $('#tbl-trans select option:selected').text();
            if (description != undefined && description.trim() === '') {
                alertmsg.error('Vui lòng nhập mô tả ngắn');
            }
            let id = sessionStorage.getItem('USERID'); 
            let MenuTrans = {
                Description: description,
                LangCode: langcode,
                UserID: id
            }
            let menutrans = JSON.stringify(MenuTrans);
            AjaxService.POST("/Admin/User/SaveTrans", { menutrans: menutrans }, function (res) {
                if (res.IsSuccess) {
                    let row = `<tr>
                                <td style="vertical-align: middle;width:65%">
                                   ${description}
                                </td>
                                <td style="vertical-align: middle;">${language}</td>
                                <td style="text-align: right;"> 
                                    <a style="color:red" href="javascript:void(0)" class="btn-bordered" onclick="User.deleteUserTrans(${res.Id}, this) " title="Xóa">
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
        deleteUserTrans: function (id, btn) {
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
                    AjaxService.POST("/Admin/User/DeleteUserTrans", { id: id }, function (res) {
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
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let page = $('.pagination .active a').text();
                $('.pagination .active a').trigger('click');
                User.loadData(parseInt(page));
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
                    AjaxService.POST("/Admin/User/Delete", { id: id }, function (res) {
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
                alertmsg.error("Bạn cần chọn ít nhất bản ghi cần xóa");
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
                        AjaxService.POST("/Admin/User/DeleteAll", { lstid: $("#hdfID").val() }, function (res) {
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
        }
    };
}();
$(function () { User.init(); });


