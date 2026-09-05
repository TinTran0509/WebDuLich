var pageIndex = 1;
var pageSize = 20;
var Product = function () {
    return {
        init: function () {
            Product.loadData();
        },
        loadData: function () { 
            let cateType = $('#cate-type').val();
            let cateId = $('#cate-id').val();
            $.get("danhsachsanpham",
                {
                    cateType: cateType,
                    cateId: cateId,
                    pageIndex: pageIndex,
                    pageSize: pageSize
                },
                function (res) {
                $('#gridData').html(res.viewContent);
                if (res.totalPages > 1) {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    $('#paginationholder').html('<ul id="pagination" class="pagination-sm"></ul>');
                    $('#pagination').twbsPagination({
                        startPage: self.pageIndex,
                        totalPages: res.totalPages,
                        visiblePages: 5,
                        onPageClick: function (event, page) {
                            self.pageIndex = page;
                            Product.loadData(self.pageIndex);
                        }
                    });
                } else {
                    $('#paginationholder').html('');
                }
            });
        }
    }
}();
$(function () {
    Product.init();
})