/*
** Onload
*/
$(function () {
    // Enable the entire row click to show a dataset
    $('#datasets tbody tr').click(function () {
        window.location = $(this).find('a').attr('href');
    });

    // Expand/Collapse filters
    $('.showAll').click(function () {
        var filterList = $(this).parents('.filterBlock').find('.filterList');
        if ($(this).html() == Global.Hide) {
            filterList.animate({ 'maxHeight': '270px' }, 1000);
            $(this).html(Global.DisplayAll);
        } else {
            filterList.animate({ 'maxHeight': filterList[0].scrollHeight }, 1000);
            $(this).html(Global.Hide);
        }
    });
});
