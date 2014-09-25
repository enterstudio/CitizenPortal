/*
** Onload
*/
$(function () {
    // Handle expanders
    $('.toggleExpander').click(ToggleExpander);

    // Handle add comment
    $('#addComment').submit(AddComment);

    // Handle rating
    Rating.Init();

    // Load data extract
    DataExtract.Load(0);

    // Load data comments
    DataComments.Load();

    // Load data visualization
    DataVisualization.Load(false);
});

/*
** Expanders
*/
function ToggleExpander() {
    var toggleExpander = $(this);
    var expander = toggleExpander.parents('.block').find('.content');

    toggleExpander.find('i').toggleClass('icon-general-plus icon-general-minus');

    expander.slideToggle(400, function () {
        if (expander.is(':visible')) {
            toggleExpander.attr('title', Global.HideText);
            if (expander.parent().is('#dataVisualization')) {
                DataVisualization.Load(true);
            }
        }
        else {
            toggleExpander.attr('title', Global.DisplayText);
        }
    });
}

/*
** AddComment
*/
function AddComment() {
    var url = '/Data/AddComment';
    var comment = $('#addComment').serialize();

    $('#addCommentAjaxLoader').show();
    $('#addCommentError').hide();

    $.ajax({
        type: 'POST',
        url: url,
        data: comment,
        dataType: 'json',
        traditional: true,
        success: function (data) {
            // Hide AJAX loader
            $('#addCommentAjaxLoader').hide();

            if (!data.Error) {
                // Close modal
                $('#addCommentModal').trigger('reveal:close');

                // Display comments
                if (!$('#dataComments').is(':visible')) {
                    $('#dataComments').show();
                }

                // Expand comments
                if (!$('#dataComments > .content').is(':visible')) {
                    $('#dataComments > .title > a').trigger('click');
                }

                // Add comment
                var d = eval('new ' + data.PostedOn.slice(1, -1));
                data.PostedOn = d.getFullYear() + '-' + (('' + d.getMonth()).length < 2 ? '0' : '') + (d.getMonth() + 1) + '-' + (('' + d.getDate()).length < 2 ? '0' : '') + d.getDate() + ' ' + (('' + d.getHours()).length < 2 ? '0' : '') + d.getHours() + ':' + (('' + d.getMinutes()).length < 2 ? '0' : '') + d.getMinutes();
                DataComments.AddComment(data);

                // Clear form
                $('#addComment').find('input[type=text], textarea').val('');
            } else {
                // Displays error
                $('#addCommentError > .content').html(data.Error);
                $('#addCommentError').show();
            }
        },
        error: function (data) {
            // Hide AJAX loader
            $('#addCommentAjaxLoader').hide();

            // Displays error
            $('#addCommentError > .content').html(data.responseText);
            $('#addCommentError').show();
        }
    });

    Recaptcha.reload();

    return false;
}

/*
** Rating object
*/
var Rating = {
    _starIndex: [-3, -1, 1, 3],
    Init: function () {
        if ($('#addRemoveRate').attr('action') == '/Data/AddRate') {
            // Set click events
            $('#upVote').off().click(function () { Rating.Add(1); });
            $('#downVote').off().click(function () { Rating.Add(-1); });
        } else {
            // Set click events
            $('#upVote, #downVote').off().click(function () { Rating.Remove(); });

            // Set mouse over & out events
            $('#upVote, #downVote').mouseover(Rating.DisplayRemoveButton).mouseout(Rating.HideRemoveButton);
        }
    },
    Add: function (rate) {
        $('#addRemoveRate #RateValue').val(rate);
        $('#addRemoveRate img').show();

        $.ajax({
            type: 'POST',
            url: '/Data/AddRate',
            data: $('#addRemoveRate').serialize(),
            dataType: 'json',
            traditional: true,
            success: Rating.SuccessCallback,
            error: Rating.ErrorCallback
        });
    },
    Remove: function () {
        $('#addRemoveRate img').show();

        $.ajax({
            type: 'POST',
            url: '/Data/RemoveRate',
            data: $('#addRemoveRate').serialize(),
            dataType: 'json',
            traditional: true,
            success: Rating.SuccessCallback,
            error: Rating.ErrorCallback
        });
    },
    SuccessCallback: function (data) {
        if (data.Error) {
            Rating.ErrorCallback(data.Error);
            return;
        }

        $('#addRemoveRate img').hide();

        if ($('#addRemoveRate').attr('action') == '/Data/AddRate') {
            // Animate buttons
            if (data.RateValue == 1) {
                $('#downVote').fadeOut();
            } else {
                $('#upVote').fadeOut();
            }

            // Disable buttons
            $('#upVote, #downVote').addClass('disabled');

            // Set correct title
            $('#upVote, #downVote').attr('title', Global.RateCancel);

            // Set correct form action
            $('#addRemoveRate').attr('action', '/Data/RemoveRate');

            // Update rating
            Global.Rating += data.RateValue;
        } else {
            // Display buttons
            $('#upVote').fadeIn();
            $('#downVote').fadeIn();

            // Set correct class
            $('#upVote').removeClass('secondary disabled').addClass('success');
            $('#downVote').removeClass('secondary disabled').addClass('alert');

            // Set correct icon
            $('#upVote').find('i').removeClass().addClass('icon-general-plus');
            $('#downVote').find('i').removeClass().addClass('icon-general-minus');

            // Set correct title
            $('#upVote').attr('title', Global.RatePositive);
            $('#downVote').attr('title', Global.RateNegative);

            // Set correct form action
            $('#addRemoveRate').attr('action', '/Data/AddRate');

            // Update rating
            Global.Rating -= data.RateValue;
        }

        // Update rating value
        $('#rating > i').each(function (ndx, item) {
            $(item).removeClass('on');
            if (Global.Rating >= Rating._starIndex[ndx]) {
                $(item).addClass('on');
            }
        });

        // Re-init form
        Rating.Init();
    },
    ErrorCallback: function (error) {
        $('#addRemoveRate img').hide();
        console.error(error);
    },
    DisplayRemoveButton: function () {
        $(this).removeClass('success alert').addClass('secondary');
        $(this).find('i').removeClass().addClass('icon-general-remove');
        $(this).attr('title', Global.RateCancel);
    },
    HideRemoveButton: function () {
        if ($(this).attr('id') == 'upVote') {
            $(this).removeClass('secondary').addClass('success');
            $(this).find('i').removeClass().addClass('icon-general-plus');
            $(this).attr('title', Global.RatePositive);
        } else {
            $(this).removeClass('secondary').addClass('alert');
            $(this).find('i').removeClass().addClass('icon-general-minus');
            $(this).attr('title', Global.RateNegative);
        }
    }
};

var _columns = null;
var headers = [];
/*
** DataExtract object
*/
var DataExtract = {
    _page: 0,
    _resultPerPage: 12,
    _ignoredKeys: ['PartitionKey', 'RowKey', 'Timestamp', 'entityid'],
    Load: function (pageOperation) {
        // Show ajax loader
        $('#dataExtract .ajaxLoader').show();

        // Set correct page
        if (this._page + pageOperation >= 0) {
            this._page += pageOperation;
        }


        //Init Columns state
        if (_columns == null) {
            _columns = new Array();
            $.getJSON(Global.DatasetUrl + '?format=json&callback=?&$top=100', null, function (data) {
                // Parse headers
                $.each(data.d, function (ndx, entity) {
                    $.each(entity, function (key, val) {
                        var header = $.trim(key);
                        if ($.inArray(header, DataExtract._ignoredKeys) == -1 && $.inArray(header, headers) == -1) {
                            headers.push(header);
                        }
                    });
                });

                //Init _Columns
                $.each(headers, function (ndx, header) {
                    $("#column_p").append($('<a id=\"' + ndx + '\" class=\"round secondary-hold label\" onclick=\"btn_click_column(this.id)\">').html(header));
                    _columns.push(true);
                });

                // Hide ajax loader
                $('#dataExtract .ajaxLoader').hide();

                DataExtract.UpdateTable();
            });


        }
            //Display buttons columns selected
        else {
            $('#column_p').empty();
            $.each(headers, function (ndx, header) {
                if (_columns[ndx] == true)
                    $("#column_p").append($('<a id=\"' + ndx + '\" class=\"round secondary-hold label\" onclick=\"btn_click_column(this.id)\">').html(header));
                else
                    $("#column_p").append($('<a id=\"' + ndx + '\" class=\"round secondary label\" onclick=\"btn_click_column(this.id)\">').html(header));
            });

            // Hide ajax loader
            $('#dataExtract .ajaxLoader').hide();

            DataExtract.UpdateTable();
        }
    },
    UpdateTable: function () {
        // Build URL
        var jsonUrl = Global.DatasetUrl + '?format=json&callback=?&$top=' + this._resultPerPage + '&$skip=' + (this._page * 12);
        $.getJSON(jsonUrl, null, function (data) {
            // Display headers
            $('#dataExtract table thead').empty();

            var thead = $('<tr>');
            $.each(headers, function (ndx, header) {
                if (_columns[ndx] == true)
                    thead.append($('<th name="column' + ndx + '">').html(header));
                else
                    thead.append($('<th name="column' + ndx + '" style="display:none">').html(header));
            });
            $('#dataExtract table thead').append(thead);

            // Display content
            $('#dataExtract table tbody').empty();
            $.each(data.d, function (ndx, content) {
                var tbody = $('<tr>');
                $.each(headers, function (ndx, header) {
                    if (_columns[ndx] == true)
                        tbody.append($('<td name="column' + ndx + '">').html(content[header]));
                    else
                        tbody.append($('<td name="column' + ndx + '" style="display:none">').html(content[header]));
                });
                $('#dataExtract table tbody').append(tbody);
            });

            // Show/Hide prev buttons
            if (DataExtract._page > 0) {
                $('#dataExtract #previousData').show();
            } else {
                $('#dataExtract #previousData').hide();
            }

            // Show/Hide next buttons
            if (data.d.length == DataExtract._resultPerPage) {
                $('#dataExtract #nextData').show();
            } else {
                $('#dataExtract #nextData').hide();
            }
        });
    }
};


function btn_click_column(column) {
    if (isHold(column)) {
        document.getElementById(column).className = "round secondary label";
        _columns[column] = false;
        $("[name=column" + column + "]").each(function (ndx) {
            this.style.display = "none";
        });
    }
    else {
        document.getElementById(column).className = "round secondary-hold label";
        _columns[column] = true;
        $("[name=column" + column + "]").each(function (ndx) {
            this.style.display = "table-cell";
        });
    }

}

function isHold(data) {
    if (document.getElementById(data).className.indexOf("hold", 0) != -1)
        return true;
    else return false;
}


var _map = null;
/*
** DataVisualization object
*/
var DataVisualization = {
    _url: Global.DatasetUrl + "?format=json&$top=100",
    _isLoaded: false,
    Load: function (mustReload) {
        if (!this._isLoaded) {
            // Create map object
            if (_map == null) { var bc = document.getElementById("bmc"); _map = new Microsoft.Maps.Map(document.getElementById('map_canvas'), { credentials: bc.textContent, enableSearchLogo: false, height: 450, width: 862 }); }
            $.getJSON(this._url + '&callback=?', function (data) {
                if (data.d.length > 0 && data.d[0].latitude && data.d[0].longitude) {
                    if (whatDecimalSeparator(data.d[0].latitude) == ',')
                        for (var i = 0; i < data.d.length; i++) {
                            _map.entities.push(new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(parseFloat(data.d[i].latitude.replace(',', '.')).toFixed(10), parseFloat(data.d[i].longitude.replace(',', '.')).toFixed(10))));
                        }
                    else
                        for (var i = 0; i < data.d.length; i++) {
                            _map.entities.push(new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(parseFloat(data.d[i].latitude).toFixed(10), parseFloat(data.d[i].longitude).toFixed(10))));
                        }
                }
                DataVisualization.SetCustomView(_map.entities);
            });
        }
        this._isLoaded = mustReload;
    },
    SetCustomView: function (data) {
        var shapeCount = data.getLength();
        _map.setView({ center: data.get(0).getLocation(), zoom: 11 });
        if (shapeCount > 0) {
            $('#dataVisualization').show();
            $('.dlBloc').css('padding-left', '4px').css('padding-right', '4px');
            $('#dlKML').css('display', 'inline-block');
        }
        else {
            $('#dataVisualization').hide();
            $('.dlBloc').css('padding-left', '10px').css('padding-right', '10px');
            $('#dlKML').hide();
        }
    }
};

/// <summary>
/// This function find the decimal separator used in the json file
/// </summary>
function whatDecimalSeparator(data) {
    if (data.indexOf(',') != -1)
        return ',';
    else return '.';
}


/*
** DataComments object
*/
var DataComments = {
    _url: Global.DataServiceUrl + "/Comments?format=json&callback=?&$filter=DatasetId%20eq%20'" + Global.Dataset + "'&$orderby=PostedOn",
    Load: function () {
        $.getJSON(this._url, null, function (data) {
            if (data.d.length > 0) {
                $('#dataComments').show();
                $.each(data.d, function (ndx, commentObj) {
                    DataComments.AddComment(commentObj);
                });
            }
            else {
                $('#dataComments').hide();
            }
        });
    },
    AddComment: function (commentObj) {
        var date = 'le ' + commentObj.PostedOn.substr(0, 10) + ' à ' + commentObj.PostedOn.substr(11, 5);

        var comment = $('<div>').addClass('comment');
        var info = $('<div>').addClass('info');
        var text = $('<div>').addClass('text');

        info.append($('<div>').addClass('name').html(commentObj.Username)).append($('<div>').addClass('date').html(date));
        text.append($('<div>').addClass('subject').html(commentObj.Subject)).append($('<div>').html(commentObj.Comment));
        comment.append(info).append(text);

        $('#dataComments > .content').prepend(comment);

        $(comment).effect('highlight', { color: '#d0ffc9' }, 3000);
    }
};
