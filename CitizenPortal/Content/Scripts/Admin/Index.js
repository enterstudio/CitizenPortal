/*
** OnLoad
*/
$(function () {
    // Remove confirm message when submiting form
    $('#saveConfig').submit(function () {
        Admin.MustConfirm = false;
    });

    // Highlight save button on any change
    $('input, textarea').change(Admin.ActivateSave);

    // Handle image suppression
    $('.deleteImage').click(Admin.DeleteImage);

    // Live render for License section
    $('#licenseText').keyup(function () {
        Admin.Render('license');
    });

    // Live render for Approach section
    $('#approachText').keyup(function () {
        Admin.Render('approach');
    });

    // Handle application modals
    $('#validAddApp').click(Applications.Add);
    $('#validEditApp').click(Applications.Edit);

    // Load application list
    Applications.Load();

    // Render for the first time
    Admin.Render('license');
    Admin.Render('approach');
});

/*
** OnBeforeUnload
*/
window.onbeforeunload = function () {
    if (Admin.MustConfirm) {
        return Global.ConfirmMessage;
    }
}

/*
** Admin object
*/
var Admin = {
    MustConfirm: false,
    Render: function (section) {
        var imageId = '#' + section + 'Image';
        var textId = '#' + section + 'Text';
        var containerId = '#' + section + 'Render';

        $(containerId).empty();

        if ($(imageId).attr('type') == 'hidden') {
            var imageArea = $('<div/>').addClass('centered').html($('<img/>').attr('src', $(imageId).val()));
            $(containerId).append(imageArea);
        }

        $(containerId).append($('<div/>').html($(textId).val()));
    },
    DeleteImage: function () {
        var label = $(this).attr('data-label');
        var inputId = $(this).attr('data-input-id');
        var inputName = $(this).attr('data-input-name');
        var containerId = '#' + inputId + 'Container';

        $(containerId).empty();
        $(containerId).append($('<label/>').attr('for', inputId).html(label));
        $(containerId).append($('<input/>').attr('type', 'file').attr('id', inputId).attr('name', inputName));

        Admin.ActivateSave();
    },
    ActivateSave: function () {
        Admin.MustConfirm = true;
        $('#submit').removeClass('secondary');
    }
}


/*
** Application object
*/
var Applications = {
    CurEditingRow: null,
    Load: function () {
        if ($('#applicationsInput').val().length == 0) {
            return;
        }

        var appsObj = jQuery.parseJSON($('#applicationsInput').val());

        $('#applicationList tbody tr.app').remove();
        $.each(appsObj, function (ndx, app) {
            Applications.AddAppRow(app);
        });
    },
    Save: function () {
        var appsObj = [];
        $.each($('#applicationList tbody tr.app'), function (ndx, item) {
            var app = {}

            app.Name = $(item).find('.colName').html();
            app.Description = $(item).find('.colDescription').html();
            app.Link = $(item).find('.colLink').html();
            app.ImageUrl = $(item).find('.colImageUrl').html();

            appsObj.push(app);
        });

        $('#applicationsInput').val(appsObj.length > 0 ? JSON.stringify(appsObj.sort(Applications.CompareRoutine)) : null);
    },
    Add: function () {
        var addModal = $(this).parents('#addAppModal');
        var app = {}

        app.Name = $(addModal).find('.appName').val();
        app.Description = $(addModal).find('.appDescription').val();
        app.Link = $(addModal).find('.appLink').val();
        app.ImageUrl = $(addModal).find('.appImageUrl').val();

        if (app.Name.length == 0 || app.Link.length == 0) {
            $('#addAppModal .alert-box').show();
            return;
        }

        $('#addAppModal .alert-box').hide();
        $('#addAppModal input[type=text]').val('');
        $('#addAppModal').trigger('reveal:close');

        Applications.AddAppRow(app);
        Applications.Save();
        Applications.Load();
    },
    Edit: function () {
        var editModal = $(this).parents('#editAppModal');
        var app = {}

        app.Name = $(editModal).find('.appName').val();
        app.Description = $(editModal).find('.appDescription').val();
        app.Link = $(editModal).find('.appLink').val();
        app.ImageUrl = $(editModal).find('.appImageUrl').val();

        if (app.Name.length == 0 || app.Link.length == 0) {
            $('#editAppModal .alert-box').show();
            return;
        }

        $(Applications.CurEditingRow).remove();
        Applications.AddAppRow(app);
        Applications.Save();
        Applications.Load();

        $('#editAppModal .alert-box').hide();
        $('#editAppModal input[type=text]').val('');
        $('#editAppModal').trigger('reveal:close');
    },
    Delete: function () {
        $(this).parents('.app').remove();

        if ($('#applicationList tbody tr.app').length == 0) {
            $('#noApplication').show();
            $('#applicationList').hide();
        }

        Applications.Save();
        Admin.ActivateSave();
    },
    AddAppRow: function (app) {
        $('#noApplication').hide();
        $('#applicationList').show();

        var appRow = $('#applicationList tbody tr.model').clone();

        appRow.find('.colAction.remove').click(Applications.Delete);
        appRow.find('.colAction.edit').click(Applications.DisplayEditModal);
        appRow.find('.colName').html(app.Name);
        appRow.find('.colDescription').html(app.Description);
        appRow.find('.colLink').html(app.Link);
        appRow.find('.colImageUrl').html(app.ImageUrl);
        appRow.removeClass('model');
        appRow.addClass('app');

        $('#applicationList tbody').append(appRow);
    },
    DisplayEditModal: function () {
        Applications.CurEditingRow = $(this).parents('.app');

        $('#editAppModal').find('input.appName').val($(Applications.CurEditingRow).find('.colName').html());
        $('#editAppModal').find('input.appDescription').val($(Applications.CurEditingRow).find('.colDescription').html());
        $('#editAppModal').find('input.appLink').val($(Applications.CurEditingRow).find('.colLink').html());
        $('#editAppModal').find('input.appImageUrl').val($(Applications.CurEditingRow).find('.colImageUrl').html());

        $('#editAppModal').reveal();
    },
    CompareRoutine: function (a, b) {
        if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
            return -1;
        }

        if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
            return 1;
        }

        return 0;
    }
}
