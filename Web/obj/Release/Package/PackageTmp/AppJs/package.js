var pageIndex = 1;
var Package = function () {
    return {
        init: function () { 
        }, 
        saveprice: function () {
            $("#loading").show();
            let packages = [];
            $('#package-3 .package-price').each(function (i, obj) {
                let pax = $(obj).data('pax');
                let arrPax = pax.split('-')
                let pax_from = arrPax[0];
                let pax_to = arrPax[1];
                let price = $(obj).val();  
                packages.push({
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 3
                })
            });
          
            $('#package-4 .package-price').each(function (i, obj) {
                let pax = $(obj).data('pax');
                let arrPax = pax.split('-')
                let pax_from = arrPax[0];
                let pax_to = arrPax[1];
                let price = $(obj).val();
                packages.push({
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 4
                })
            });
            
            $('#package-5 .package-price').each(function (i, obj) {
                let pax = $(obj).data('pax');
                let arrPax = pax.split('-')
                let pax_from = arrPax[0];
                let pax_to = arrPax[1];
                let price = $(obj).val();
                packages.push({
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 5
                })
            });
            
            $('#package-6 .package-price').each(function (i, obj) {
                let pax = $(obj).data('pax');
                let arrPax = pax.split('-')
                let pax_from = arrPax[0];
                let pax_to = arrPax[1];
                let price = $(obj).val();
                packages.push({
                    Pax_From: pax_from,
                    Pax_To: pax_to,
                    Price: price,
                    PackageID: 6
                })
            });
            let data = JSON.stringify(packages)
            AjaxService.POST("/Admin/Package/SavePrice", { packages: data }, function (res) { 
                $("#loading").hide();
                alertmsg.success(res.Messenger); 
            });
        }
    };
}();
$(function () { Package.init(); });


