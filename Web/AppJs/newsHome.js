var pageIndex = 1;
var pageSize = 10;
var News = function () {
    return {
        init: function () {
            News.loadData();
        },
        loadData: function () { 
            $.get("danhsachtintuc", { pageIndex: pageIndex, pageSize: pageSize }, function (res) {
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
                            News.loadData(self.pageIndex);
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
    News.init();
})