var Hotel = function () {
    return {
        init: function () { 
          
        },
        loadData: function (page) {
            $("#loading").show();
            AjaxService.POST("/Admin/Hotel/ListData", { page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
        },
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                location.href = "/admin/hotel";
                modal.hide();
            } else {
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger); 
                location.href = "/admin/hotel";
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
                    AjaxService.POST("/Admin/Hotel/Delete", { id: id }, function (res) {
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
$(function () { Hotel.init(); });


