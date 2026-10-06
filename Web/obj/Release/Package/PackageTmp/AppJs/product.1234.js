var Product = function () {
    return {
        init: function () { 
            $('.package-price').on('change', function () {
                Product.getPackagePrice();
            });
            document.querySelectorAll('.language-tab-inc').forEach(function (tab) {
                tab.addEventListener('click', function () {
                    const lang = this.dataset.lang;
                    document.querySelectorAll('.language-tab-inc').forEach(function (item) {
                        item.classList.remove('active');
                    }); this.classList.add('active');
                    document.querySelectorAll('.language-pane-inc').forEach(function (pane) {
                        pane.classList.remove('active');
                        if (pane.dataset.lang === lang) {
                            pane.classList.add('active');
                        }
                    });
                });
            });
            document.querySelectorAll('.language-tab-exc').forEach(function (tab) {
                tab.addEventListener('click', function () {
                    const lang = this.dataset.lang;
                    document.querySelectorAll('.language-tab-exc').forEach(function (item) {
                        item.classList.remove('active');
                    }); this.classList.add('active');
                    document.querySelectorAll('.language-pane-exc').forEach(function (pane) {
                        pane.classList.remove('active');
                        if (pane.dataset.lang === lang) {
                            pane.classList.add('active');
                        }
                    });
                });
            });
            document.querySelectorAll('.language-tab-imp').forEach(function (tab) {
                tab.addEventListener('click', function () {
                    const lang = this.dataset.lang;
                    document.querySelectorAll('.language-tab-imp').forEach(function (item) {
                        item.classList.remove('active');
                    }); this.classList.add('active');
                    document.querySelectorAll('.language-pane-imp').forEach(function (pane) {
                        pane.classList.remove('active');
                        if (pane.dataset.lang === lang) {
                            pane.classList.add('active');
                        }
                    });
                });
            });
            document.querySelectorAll('.language-tab-hig').forEach(function (tab) {
                tab.addEventListener('click', function () {
                    const lang = this.dataset.lang;
                    document.querySelectorAll('.language-tab-hig').forEach(function (item) {
                        item.classList.remove('active');
                    }); this.classList.add('active');
                    document.querySelectorAll('.language-pane-hig').forEach(function (pane) {
                        pane.classList.remove('active');
                        if (pane.dataset.lang === lang) {
                            pane.classList.add('active');
                        }
                    });
                });
            });
            document.querySelectorAll('.language-tab-inc').forEach(function (tab) {
                tab.addEventListener('click', function () {
                    const lang = this.dataset.lang;
                    document.querySelectorAll('.language-tab-inc').forEach(function (item) {
                        item.classList.remove('active');
                    }); this.classList.add('active');
                    document.querySelectorAll('.language-pane-inc').forEach(function (pane) {
                        pane.classList.remove('active');
                        if (pane.dataset.lang === lang) {
                            pane.classList.add('active');
                        }
                    });
                });
            }); 

            let locationIDsData = $('#LocationID').val();
        },
        loadData: function (page) {
            $("#loading").show(); 
            let code = $("#txtCode").val(); 
            let title = $("#txtName").val(); 
            let type = $("#type").val(); 
            AjaxService.POST("/Admin/Product/ListData", { code: code, title: title, type: type, page: page}, function (res) {
                $("#gridData").html(res.viewContent);
                $("#loading").hide();
            });
        },  
        loadfrmAddItinerary: function (id) {
            modal.Render("/Admin/Product/AddItinerary/" + id, "Thêm lịch trình", "modal-lg");
        }, 
        onAddSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                window.location.href = "/admin/Product" 
                modal.hide();
            } else { 
                let height = 0;
                if (res.Session != null)
                    height = res.Session;
                $('html, body').animate({ scrollTop: height }, 500);
                alertmsg.error(res.Messenger);
            }
            $("#loading").hide();
        },
        loadfrmEdit: function (id) {
            modal.Render("/Admin/Product/Edit/" + id, "Cập nhật", "modal-lg");
        },
        onEditSuccess: function (res) {
            if (res.IsSuccess == true) {
                alertmsg.success(res.Messenger);
                let page = $('.pagination .active a').text();
                $('.pagination .active a').trigger('click');
                window.location.href = "/admin/Product" 
               /* Product.loadData(parseInt(page));*/
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
                    AjaxService.POST("/Admin/Product/Delete", { id: id }, function (res) {
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
        getLocationByCountry: function (id) {
            $.get("/Admin/Product/GetLocationByCountry", { id: id }, function (res) {
                let option = '';
                res.Data.forEach((item, index, arr) => {
                    option += `<option value="${item.LocationID}">${item.Name}</option>`;
                });

                $('#LocationIDs').html(option)
            }); 
        },
        selectCountryAdd: function () {
            let ids = $('input[name="country"]:checked').map(function () {
                return this.value;
            }).get(); 
            Product.setDataCountry(ids);
        },
        selectCountryEdit: function () {
            let ids = $('input[name="country"]:checked').map(function () {
                return $(this).data('country');
            }).get();
            Product.setDataCountry(ids);
        },
        setDataCountry: function (ids) {
            let container = $('#countryIds');
            if (ids.length > 0) {
                $.get("/Admin/Product/GetLocationByCountry", { ids: ids.join(',') }, function (res) {
                    let option = '';
                    res.Data.forEach((item, index, arr) => {
                        option += `<div class="location-item" data-location-id="${item.LocationID}">
                                    <span class="drag">☰</span>
                                    <span class="order">${index + 1}</span>
                                    <span class="location-name">${item.Name}</span>
                                    <button class="remove">×</button>
                                </div>`;
                    });
                    $('#locationList').html(option);
                });
                container.empty();
                ids.forEach(function (id) {
                    container.append(
                        $('<input>', {
                            type: 'hidden',
                            name: 'CountryID',
                            value: id
                        })
                    );
                });
            } else {
                $('#locationList').html('');
                container.empty();
            }
        },
        selectHotelAdd: function () {
            let ids = $('input[name="hotel"]:checked').map(function () {
                return this.value;
            }).get(); 
            Product.setDataHotel(ids);
        },
        selectHotelEdit: function () {
            let ids = $('input[name="hotel"]:checked').map(function () {
                return $(this).data('hotel');
            }).get();
            Product.setDataHotel(ids);
        },
        setDataHotel: function (ids) { 
            let container = $('#hotelIds');
            container.empty();
            ids.forEach(function (id) {
                container.append(
                    $('<input>', {
                        type: 'hidden',
                        name: 'HotelID',
                        value: id
                    })
                );
            });
        },
        selectLocationEdit: function () { 
            let container = $('#LocationID');
            container.empty();
            let value = '';
            $('#LocationIDs option:selected').each(function () {
                let id = $(this).data('location'); 
                value += value == '' ? id : ',' + id;
            }); 
            container.val(value);
        } ,
        getPackagePrice: function () {
            let packages = [];
            $('#package-3 .package-price').each(function (i, obj) {
                let pax_from = 0;
                let pax_to = 0;
                let id = $(obj).data('id');
                let pax = $(obj).data('pax');
                if (Product.isInt(pax)) {
                     pax_from = pax;
                     pax_to = pax;
                }
                else {
                    let arrPax = pax.split('-')
                     pax_from = arrPax[0];
                     pax_to = arrPax[1];
                }
                let price = $(obj).val();
                packages.push({
                    ID: id,
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 3
                })
            });

            $('#package-4 .package-price').each(function (i, obj) {
                let pax_from = 0;
                let pax_to = 0;
                let id = $(obj).data('id');
                let pax = $(obj).data('pax'); 
                if (Product.isInt(pax)) {
                     pax_from = pax;
                     pax_to = pax;
                }
                else {
                    let arrPax = pax.split('-')
                     pax_from = arrPax[0];
                     pax_to = arrPax[1];
                }
                let price = $(obj).val();
                packages.push({
                    ID: id,
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 4
                })
            });

            $('#package-5 .package-price').each(function (i, obj) {
                let pax_from = 0;
                let pax_to = 0;
                let id = $(obj).data('id');
                let pax = $(obj).data('pax');
                if (Product.isInt(pax)) {
                     pax_from = pax;
                     pax_to = pax;
                }
                else {
                    let arrPax = pax.split('-')
                     pax_from = arrPax[0];
                     pax_to = arrPax[1];
                }
                let price = $(obj).val();
                packages.push({
                    ID: id,
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 5
                })
            });

            $('#package-6 .package-price').each(function (i, obj) {
                let pax_from = 0;
                let pax_to = 0;
                let id = $(obj).data('id');
                let pax = $(obj).data('pax');
                if (Product.isInt(pax)) { 
                     pax_from = pax;
                     pax_to = pax;
                }
                else {
                    let arrPax = pax.split('-')
                     pax_from = arrPax[0];
                     pax_to = arrPax[1];
                }
                let price = $(obj).val();
                packages.push({
                    ID: id,
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 6
                })
            });
            let data = JSON.stringify(packages);
            $('#Package_Price').val(data);
        },
        toggleBlock: function (header) {
            const block = header.closest('.info-block');
            block.classList.toggle('collapsed');
        },
        removeDay: function (dayId) {
            if (!confirm('Bạn có chắc muốn xóa ngày này?')) {
                return;
            }
            const index = days.findIndex(x => x.day === dayId);
            if (index === -1) {
                return;
            }
            const dayData = days[index];
            if (dayData.editor) {
                dayData.editor.destroy()
                    .then(function () {
                        console.log('Editor destroyed:', dayId);
                    })
                    .catch(function (error) {
                        console.error(error);
                    });
            }
            const dayElement = document.querySelector(`[data-day="${dayId}"]`);

            if (dayElement) {
                dayIndex = dayIndex - 1;
                dayElement.remove();
            }

            days.splice(index, 1);
        },
        isNumeric: function (str) { 
            return str.trim() !== "" && Number.isFinite(Number(str));
        },
        isInt: function (value) {
            const number = Number(value);
            return Number.isInteger(number);
        }
    };
}();
$(function () { Product.init(); });


