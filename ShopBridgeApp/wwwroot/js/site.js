function ProductListDatatable() {
    $('#tblProductList').dataTable({
        "processing": true, // control the processing indicator.
        "serverSide": true, // recommended to use serverSide when data is more than 10000 rows for performance reasons
        "info": true,   // control table information display field  //"pageLength": 1, // "sDom": "Rtlftip",
        "bFilter": false,  //"aaSorting": [ sorting ],
        "bPaginate": true,
        "stateSave": true,  //restore table state on page reload,
        //"ajax": "url/to/ajax/function",
        "iDisplayLength": 10,
        // "cache": false,
        "bAutoWidth": false,
        "scrollX": true,
        "lengthMenu": [[5, 10, 20, 50, 100], [5, 10, 20, 50, 100]],    // use the first inner array as the page length values and the second inner array as the displayed options
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            oSettings.jqXHR = $.ajax({
                "url": "/Product/DataTableProductList",
                "data": aoData,
                "cache": false,
                "success": function (json) {
                    if (json.sError) {
                        oSettings.oApi._fnLog(oSettings, 0, json.sError);
                    }
                    $(oSettings.oInstance).trigger('xhr', [oSettings, json]);
                    fnCallback(json);
                },
            });
        },
        'fnCreatedRow': function (nRow, aData, iDataIndex) {
            $(nRow).attr('itemid', aData['ID']); // or whatever you choose to set as the id
        },
        "drawCallback": function (oSettings) {
        },
        "columns": [
            { "data": "ProductName", "orderable": false },
            { "data": "Price", "orderable": false },
            {
                "data": function (aData) {
                    var imgstring = '';
                    var mainimgstr = '';
                    if (aData['FileName'] != "") {
                        imgstring = '<img src="/Product Images/' + aData['FileName'] + '"style="height: 100px;width: 150px;" />';
                        mainimgstr += imgstring;
                    }
                    else {
                        mainimgstr = "Image Not Available"
                    }
                    return mainimgstr;
                }, "orderable": false
            },
            {
                "data": function (aData) {
                    var editstring = '';
                    var deletestring = '';
                    editstring = '<a href="javascript:void(0)" onclick = "GetProductsDetails(' + aData['Id'] + ');">Edit</a> | ';
                    deletestring = '<a href="javascript:void(0)" onclick = "DeleteProduct(' + aData['Id'] + ');">Delete</a>';
                    var mainstr = '';
                    mainstr += editstring;
                    mainstr += deletestring;
                    return mainstr;
                }, "orderable": false
            },
        ],
        "order": [],
    });
}

$('#btnSubmit').click(function () {

    var file = $('#ImgProductImage')[0];
    if (file.files[0] != null) {
        var fileext = "." + file.files[0].name.split(".")[1];
        if (fileext != ".png" && fileext != ".jpg" && fileext != ".jpeg") {
            alert("Product Image has not valid extension.");
            return false;
        }
    }

    if ($('#txtProductName').val() == "" || $('#txtProductPrice').val() == "") {
        alert("Product Name and Price is Mandatory");
        return false;
    }

    var data = {
        ProductName: $('#txtProductName').val(),
        Description: $('#txtProductDescription').val(),
        Price: $('#txtProductPrice').val(),
        Id: $('#hdnId').val() > 0 ? $('#hdnId').val() : 0,
        FileName: $('#hdnFileName').val() != "" ? $('#hdnFileName').val() : ""
    };

    var formData = new FormData();
    formData.append("data", JSON.stringify(data));
    formData.append('Imagefiles', file.files[0]);

    $.ajax({
        url: '/Product/ProductOperation',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result == -1) {
                alert('Something went wrong please try again');
            }
            else if (result == 0) {
                alert('Record is already exists');
            }
            else {
                window.location.reload();
            }
        },
        error: function () {
            $('#ImgProductImage').val(null);
            alert('Something went wrong please try again');
        }
    });
});

function GetProductsDetails(Id) {
    if (Id != "" && Id > 0) {
        $.ajax({
            url: '/Product/GetProduct',
            data: { Id: Id },
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $('#txtProductName').val(data.ProductName);
                    $('#txtProductDescription').val(data.Description);
                    $('#txtProductPrice').val(data.Price);
                    $('#hdnId').val(data.Id);
                    $('#hdnFileName').val(data.FileName);
                    if (data.FileName != "") {
                        $('#GetImg').show();
                        $('#GetImg').attr('src', '/Product Images/' + data.FileName);
                    }
                    $('#ProductModal').modal('show');
                }
            },
            error: function (xhr) {
                alert("Something went wrong while get record for edit!");
            }
        });
    }
}

function DeleteProduct(Id) {
    if (confirm("Are you sure you want to delete this Product?")) {
        $.ajax({
            url: '/Product/DeleteProduct',
            data: { Id: Id },
            type: "POST",
            success: function (data) {
                if (data > 0) {
                    alert("Record Deleted successfully");
                    window.location.href = "/Product/ProductList";
                }
            },
            error: function (xhr) {
                alert("Something went wrong while delete record!");
            }
        });
    }
    return false;
}

//Add only decimal value to price field
$("#txtProductPrice").on("input", function (evt) {
    var self = $(this);
    self.val(self.val().replace(/[^0-9\.]/g, ""));
    if ((evt.which != 46 || self.val().indexOf('.') != -1) && (evt.which < 48 || evt.which > 57)) {
        return false;
    }
});


$('#btnAddNewProduct').click(function () {
    $('#txtProductName').val('');
    $('#txtProductDescription').val('');
    $('#txtProductPrice').val('');
    $('#hdnId').val('');
    $('#hdnFileName').val('');
    $('#ImgProductImage').val(null);
    $('#GetImg').hide();
});