customAlert();
var BISHallmark = angular.module("BISHallmark", []);

BISHallmark.directive('validNumber', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            // if (attrs.numeric == true) {
            //alert(scope.CurrentProductAssumptionModel.IsNumeric)
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }

                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }

                }

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
        // }
    };
});
BISHallmark.directive('allowOnlyNumbers', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event) {
                if (event.which == 64 || event.which == 16) {
                    // to allow numbers  
                    return false;
                } else if (event.which >= 48 && event.which <= 57) {
                    // to allow numbers  
                    return true;
                } else if (event.which >= 96 && event.which <= 105) {
                    // to allow numpad number  
                    return true;
                } else if (event.which == 9) {
                    // to allow tab  
                    return true;
                }

                else if ([8, 13, 27, 37, 38, 39, 40].indexOf(event.which) > -1) {
                    // to allow backspace, enter, escape, arrows  
                    return true;
                } else {
                    event.preventDefault();
                    // to stop others  
                    return false;
                }
            });
        }
    }
});

BISHallmark.filter('filterByProperty', function () {

    /* array is first argument, each addiitonal argument is prefixed by a ":" in filter markup*/
    return function (dataArray, searchTerm, propertyName) {

        if (!dataArray) return;
        /* when term is cleared, return full array*/
        if (!searchTerm) {
            return dataArray
        } else {
            / otherwise filter the array /
            var term = searchTerm.toString().toLowerCase();
            return dataArray.filter(function (item) {
                var valid = false;
                for (var i = 0; i < propertyName.split(',').length; i++) {
                    if (propertyName.split(',')[i].split('.').length <= 1) {
                        valid = item[propertyName.split(',')[i]].toString().toLowerCase().indexOf(term) > -1;
                    }
                    else {
                        var arr = propertyName.split(',')[i].split('.');
                        var obj = null;
                        for (var i = 0; i < arr.length; i++) {
                            if (obj == null) {
                                obj = item[arr[i]];
                            }
                            else {
                                obj = obj[arr[i]];
                            }
                        }
                        valid = obj.toString().toLowerCase().indexOf(term) > -1;

                    }
                    if (valid) {
                        return valid;
                    }
                }
                return valid;

            });
        }
    }
});

BISHallmark.controller('AdminController', ['$scope', function ($scope) {
    ModelDefinations();

    function ModelDefinations() {
        $scope.AccountListModel = {};
        $scope.AccountModel = {
            'AccountID': '',
            'AccountName': '',
            'Description': '',
            'Status': '',
            'UpdatedDate': ''
        };

        $scope.HallmarkCentreListModel = {};
        $scope.HallmarkCentreModel = {
            'CentreID': '',
            'CentreName': '',
            'CentreLogo': '',
            'Address1': '',
            'Address2': '',
            'Address3': '',
            'PINCode': '',
            'PhoneNumber': '',
            'CellNumber': '',
            'Email': '',
            'BISLicenseNumber': '',
            'GSTIN': '',
            'BISLogo': '',
            'Status': '',
            'AccountID': '',
            'AccountName': '',
            'UpdatedDate': ''
        };

        $scope.EmployeeTypeListModel = {};
        $scope.EmployeeTypeModel = {
            'EmployeeTypeID': '',
            'EmployeeTypeName': '',
            'Status': ''
        };

        $scope.EmployeeListModel = {};
        $scope.EmployeeModel = {
            'EmployeeID': '',
            'EmployeeName': '',
            'EmployeeTypeID': '',
            'HallmarkCenterID': '',
            'Address1': '',
            'Address2': '',
            'Address3': '',
            'PINCode': '',
            'PhoneNumber': '',
            'CellNumber': '',
            'Email': '',
            'Password': '',
            'Status': '',
            'UpdatedDate': '',
            'EmployeeTypeName': '',
            'CentreName': ''
        };

        $scope.CustomerListModel = {};
        $scope.CustomerModel = {
            'CustomerID': '',
            'HallmarkCenterID':'',
            'CustomerName': '',
            'DBAName': '',
            'CustomerCode': '',
            'Address1': '',
            'Address2': '',
            'Address3': '',
            'PINCode': '',
            'PhoneNumber': '',
            'Email': '',
            'Logo': '',
            'GSTIN': '',
            'ExpiryDate': '',
            'CertificateNumber': '',
            'Status': '',
            'UpdatedBy': '',
            'ExpiryDate': '',
            'CertificateNumber': ''
        };

        $scope.CustomerPOCListModel = {};
        $scope.CustomerPOCModel = {
            'CustomerPOCID': '',
            'CustomerPOCName': '',
            'CustomerID': '',
            'Address1': '',
            'Address2': '',
            'Address3': '',
            'PINCode': '',
            'PhoneNumber': '',
            'Email': '',
            'Status': '',
            'UpdatedBy': ''
        };

        $scope.sortpropertyName = 'Title';
        $scope.sortorder = false;
    }

    //Accounts Start
    GetAccountsList();
    ViewEditUserByID();
    function GetAccountsList() {
        var url = $("#GetAccountList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.AccountListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.AccountListModel.length; items++) {
                            $scope.AccountListModel[items].LastModifiedDate = GetLastModifiedDate($scope.AccountListModel[items].LastModifiedDate);
                            if ($scope.AccountListModel[items].Status == true) {
                                $scope.AccountListModel[items].Status = 'Active';
                            }
                            else {
                                $scope.AccountListModel[items].Status = 'Inactive';
                            }
                        }
                        $scope.sortpropertyName = 'AccountName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.SubmitAccount = function () {
        var url = $("#SaveAccount").val();
        if ($scope.AccountModel.AccountID == "") {
            var AccountID = 0;
        }
        else {
            var AccountID = $scope.AccountModel.AccountID;
        }
        $scope.AccountModel.AccountID = AccountID;

        if (AccountValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsAccountArray':" + JSON.stringify($scope.AccountModel) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("Account Created Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    };
    function AccountValidation() {
        var flag = true;
        var AccountName = $scope.AccountModel.AccountName;

        if (AccountName == '') {
            alert('Account Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        return flag;
    }
    function ViewEditUserByID() {
        var url = $("#ViewAccount").val();
        var hdnFlagVal = angular.element("#hdnAccountId").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'AccountID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.AccountModel.AccountID = data[i]["AccountID"];
                            $scope.AccountModel.AccountName = data[i]["AccountName"];
                            $scope.AccountModel.Description = data[i]["Description"];
                            $scope.AccountModel.Status = data[i]["Status"];
                            $('input[type="text"],select').attr("disabled", true);
                            $('input[type="checkbox"],select').attr("disabled", true);
                            $scope.UpdateAccountButton = false;
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.ViewAccountNext = function () {
        $scope.UpdateAccountButton = true;
        $('input[type="text"],select').attr("disabled", false);
        $('input[type="checkbox"]').attr("disabled", false);
        $("#ButtonEdit").hide();
        $("#accountName").attr("disabled", true);
    };
    $scope.CancelAccountUpdate = function () {
        window.location.href = window.location.href;
    };
    $scope.UpdateAccount = function () {
        var url = $("#UpdateAccount").val();
        if ($scope.AccountModel.AccountID == "") {
            var AccountID = 0;
        }
        else {
            var AccountID = $scope.AccountModel.AccountID;
        }
        $scope.AccountModel.AccountID = AccountID;
        if (AccountValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsAccountArray':" + JSON.stringify($scope.AccountModel) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("Account Updated Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        })
                    }
                }, error: function (data) {
                }
            });
        }
    };
    $scope.DeleteAccount = function (account) {
        var url = $("#DeleteAccount").val();
        if (url != undefined) {
            $scope.AccountModel.AccountID = account.AccountID;
            confirm("Do you want to Delete Account ?", function (done) {
                if (done) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;",
                        url: url,
                        data: "{'AccountDetails':" + JSON.stringify($scope.AccountModel) + "}",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert('Account Deleted Successfully', {
                                    "title": "",
                                    "button": "OK"
                                }, function () {
                                    window.location.href = window.location.href;
                                });
                            }
                        }, error: function (data) {
                            alert('Error:' + data, {
                                "title": "",
                                "button": "OK"
                            }, function () {
                            });
                        }
                    });
                }
                else {
                    return false;
                }
            }, {
                "title": "",
                "cancel": {
                    "text": "No",
                    "default": false
                },
                "done": {
                    "text": "Yes"
                }
            })
        }
    };
    //Accounts End

    //Hallmark Centre Start
    GetHallmarkCentreList();
    ViewEditHallmarkByID();
    function GetHallmarkCentreList() {
        var url = $("#GetHallmarkCentreList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.HallmarkCentreListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.HallmarkCentreListModel.length; items++) {
                            $scope.HallmarkCentreListModel[items].UpdatedDate = GetLastModifiedDate($scope.HallmarkCentreListModel[items].UpdatedDate);
                            if ($scope.HallmarkCentreListModel[items].Status == true) {
                                $scope.HallmarkCentreListModel[items].Status = 'Active';
                            }
                            else {
                                $scope.HallmarkCentreListModel[items].Status = 'Inactive';
                            }
                        }
                        $scope.sortpropertyName = 'CentreName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.SubmitHallmarkCentre = function () {
        var url = $("#SaveHallmarkCentre").val();
        var fdata = new FormData();
        var centreLogo = $("#CentreLogoupload").get(0);
        var files1 = centreLogo.files;
        if (files1.length > 0) {
            for (var i = 0; i < files1.length; i++) {
                fdata.append("CentreLogo", files1[i]);
            }
        }
        var bisLogo = $("#BISLogoupload").get(0);
        var files2 = bisLogo.files;
        if (files2.length > 0) {
            for (var i = 0; i < files2.length; i++) {
                fdata.append("BISLogo", files2[i]);
            }
        }
        if (HallmarkCentreValidation()) {
            if (files1.length > 0) {
                if (files2.length > 0) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ParamsHallmarkCentreArray':" + JSON.stringify($scope.HallmarkCentreModel) + " }",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed" && data != "Data Exists") {
                                var sucessdata = data;
                                var profidenity;
                                if (sucessdata.length > 0) {
                                    profidenity = sucessdata.split('@')[1];
                                }

                                $scope.HallmarkCentreModel.CentreID = profidenity;
                                fdata.append('HallmarkDetails', JSON.stringify($scope.HallmarkCentreModel));

                                $("#hdnCentreId").val(profidenity);
                                var options = {};
                                options.url = "Handlers/HallmarkCentreImageHandler.ashx";
                                options.type = "POST";
                                options.data = fdata;
                                options.contentType = false;
                                options.processData = false;
                                options.success = function (result) {
                                    // alert(result);
                                };
                                options.error = function (err) {
                                    // alert(err.responseText)
                                };

                                $.ajax(options);
                                alert("Hallmark Centre Created Successfully", {
                                    "title": "",
                                    "button": "Ok"
                                }, function () {
                                    window.location = $("#Index").val();
                                });

                            }
                            else if (data == "Data Exists") {
                                alert("Hallmark Centre Already Exists", {
                                    "title": "",
                                    "button": "Ok"
                                }, function () {
                                });
                            }
                        }, error: function (data) {
                        }
                    });
                }
                else {
                    alert("Upload BIS Logo", {
                        "title": "",
                        "button": "Ok"
                    }, function () {
                    });
                }
            }
            else {
                alert("Upload Centre Logo", {
                    "title": "",
                    "button": "Ok"
                }, function () {
                });
            }
        }

    };
    function HallmarkCentreValidation() {
        var flag = true;
        var AccountName = $scope.HallmarkCentreModel.AccountName;
        var CentreName = $scope.HallmarkCentreModel.CentreName;
        var address1 = $scope.HallmarkCentreModel.Address1;
        var address2 = $scope.HallmarkCentreModel.Address2;
        var address3 = $scope.HallmarkCentreModel.Address3;
        var pINCode = $scope.HallmarkCentreModel.PINCode;
        var phoneNumber = $scope.HallmarkCentreModel.PhoneNumber;
        var cellNumber = $scope.HallmarkCentreModel.CellNumber;
        var email = $scope.HallmarkCentreModel.Email;
        var bISLicenseNumber = $scope.HallmarkCentreModel.BISLicenseNumber;
        var GSTIN = $scope.HallmarkCentreModel.GSTIN;

        if (AccountName == '' || AccountName == null) {
            alert('Please Select Account.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (CentreName == '') {
            alert('Centre Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address1 == '') {
            alert('Address1 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address2 == '') {
            alert('Address2 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address3 == '') {
            alert('Address3 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (pINCode == '') {
            alert('PIN Code Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (phoneNumber == '') {
            alert('Phone Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (cellNumber == '') {
            alert('Cell Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (email == '') {
            alert('Email Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (bISLicenseNumber == '') {
            alert('BIS License Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (GSTIN == '') {
            alert('GSTIN Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        return flag;
    }
    function ViewEditHallmarkByID() {
        var url = $("#ViewHallmarkCentre").val();
        var hdnFlagVal = angular.element("#hdnCentreId").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'CentreID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {

                            $scope.HallmarkCentreModel.AccountName = data[i]["AccountID"];
                            $scope.HallmarkCentreModel.CentreID = data[i]["CentreID"];
                            $scope.HallmarkCentreModel.CentreName = data[i]["CentreName"];
                            $scope.HallmarkCentreModel.Address1 = data[i]["Address1"];
                            $scope.HallmarkCentreModel.Address2 = data[i]["Address2"];
                            $scope.HallmarkCentreModel.Address3 = data[i]["Address3"];
                            $scope.HallmarkCentreModel.PINCode = data[i]["PINCode"];
                            $scope.HallmarkCentreModel.PhoneNumber = data[i]["PhoneNumber"];
                            $scope.HallmarkCentreModel.CellNumber = data[i]["CellNumber"];
                            $scope.HallmarkCentreModel.Email = data[i]["Email"];
                            $scope.HallmarkCentreModel.BISLicenseNumber = data[i]["BISLicenseNumber"];
                            $scope.HallmarkCentreModel.GSTIN = data[i]["GSTIN"];
                            $scope.HallmarkCentreModel.Status = data[i]["Status"];
                            if (data[i]["CentreLogo"] != '') {
                                $("#CentreLogoImg").attr('src', data[i]["CentreLogo"]);
                            }
                            else {
                                $("#CentreLogoImg").attr('src', '../Images/default-logo.png');
                            }
                            if (data[i]["BISLogo"] != '') {
                                $("#BISLogoImg").attr('src', data[i]["BISLogo"]);
                            }
                            else {
                                $("#BISLogoImg").attr('src', '../Images/default-logo.png');
                            }
                            $('input[type="text"],select').attr("disabled", true);
                            $('input[type="checkbox"],select').attr("disabled", true);
                            $scope.UpdateHallmarkButton = false;
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.ViewHallmarkNext = function () {
        $scope.UpdateHallmarkButton = true;
        $('input[type="text"],select').attr("disabled", false);
        $('input[type="checkbox"],select').attr("disabled", false);
        $("#ButtonEdit").hide();
        $('.fileUploadDiv').css({ "background-color": "#fff" })
    };
    $scope.CancelHallmarkUpdate = function () {
        window.location.href = window.location.href;
    }
    $scope.UpdateHallmarkCentre = function () {
        var url = $("#UpdateHallmarkCentre").val();
        var fdata = new FormData();
        var centreLogo = $("#CentreLogoupload").get(0);
        var files1 = centreLogo.files;
        if (files1.length > 0) {
            for (var i = 0; i < files1.length; i++) {
                fdata.append("CentreLogo", files1[i]);
            }
        }
        var bisLogo = $("#BISLogoupload").get(0);
        var files2 = bisLogo.files;
        if (files2.length > 0) {
            for (var i = 0; i < files2.length; i++) {
                fdata.append("BISLogo", files2[i]);
            }
        }

        if (HallmarkCentreValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsHallmarkCentreArray':" + JSON.stringify($scope.HallmarkCentreModel) + " }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed" && data != "Data Exists") {
                        var sucessdata = data;
                        var profidenity;
                        if (sucessdata.length > 0) {
                            profidenity = sucessdata.split('@')[1];
                        }

                        $scope.HallmarkCentreModel.CentreID = profidenity;
                        fdata.append('HallmarkDetails', JSON.stringify($scope.HallmarkCentreModel));

                        //if (files1.length > 0 && files2.length > 0) {
                        $("#hdnCentreId").val(profidenity);
                        var options = {};
                        options.url = "Handlers/HallmarkCentreImageHandler.ashx";
                        options.type = "POST";
                        options.data = fdata;
                        options.contentType = false;
                        options.processData = false;
                        options.success = function (result) {
                            // alert(result);
                        };
                        options.error = function (err) {
                            // alert(err.responseText)
                        };

                        $.ajax(options);
                        // }
                        alert("Hallmark Centre Updated Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            $('input[type="text"],select,input[type="file"]').attr("disabled", true);
                            $scope.UpdateHallmarkButton = false;
                            $("#ButtonEdit").show();
                            $scope.$apply();
                            window.location = $("#Index").val();
                        });
                    }
                    else if (data == "Data Exists") {
                        alert("Hallmark Centre Already Exists", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    };
    $scope.DeleteHallmarkCentre = function (hallmarkcentre) {
        var url = $("#DeleteHallmarkCentre").val();

        if (url != undefined) {
            $scope.HallmarkCentreModel.CentreID = hallmarkcentre.CentreID;
            confirm("Do you want to Delete Hallmark Centre ?", function (done) {
                if (done) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;",
                        url: url,
                        data: "{'HallmarkCentreDetails':" + JSON.stringify($scope.HallmarkCentreModel) + "}",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert('Hallmark Centre Deleted Successfully', {
                                    "title": "",
                                    "button": "OK"
                                }, function () {
                                    window.location.href = window.location.href;
                                });
                            }
                        }, error: function (data) {
                            alert('Error:' + data, {
                                "title": "",
                                "button": "OK"
                            }, function () {
                            });
                        }
                    });
                }
                else {
                    $("#loadingDiv").hide();
                    return false;
                }
            }, {
                "title": "",
                "cancel": {
                    "text": "No",
                    "default": false
                },
                "done": {
                    "text": "Yes"
                }
            })
        }
    };
    //Hallmark Centre End

    //EmployeeType Start
    GetEmployeeTypeList();
    ViewEditEmployeeTypeByID();

    function GetEmployeeTypeList() {

        var url = $("#GetEmployeeTypeList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.EmployeeTypeListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.EmployeeTypeListModel.length; items++) {
                            $scope.EmployeeTypeListModel[items].UpdatedDate = GetLastModifiedDate($scope.EmployeeTypeListModel[items].UpdatedDate);
                            if ($scope.EmployeeTypeListModel[items].Status == true) {
                                $scope.EmployeeTypeListModel[items].Status = 'Active';
                            }
                            else {
                                $scope.EmployeeTypeListModel[items].Status = 'Inactive';
                            }
                        }
                        $scope.sortpropertyName = 'EmployeeTypeName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.SaveEmployeeType = function () {
        var url = $("#SaveEmployeeType").val();
        if ($scope.EmployeeTypeModel.EmployeeTypeID == "") {
            var EmployeeTypeID = 0;
        }
        else {
            var EmployeeTypeID = $scope.EmployeeTypeModel.EmployeeTypeID;
        }
        $scope.EmployeeModel.EmployeeTypeID = EmployeeTypeID;
        if (EmployeeTypeValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsEmployeeTypeArray':" + JSON.stringify($scope.EmployeeTypeModel) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("EmployeeType Created Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#EmployeeTypeList").val();
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    }
    function EmployeeTypeValidation() {
        var flag = true;
        var EmployeeType = $scope.EmployeeTypeModel.EmployeeTypeName;

        if (EmployeeType == '') {
            alert('EmployeeType Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        return flag;
    }
    function ViewEditEmployeeTypeByID() {

        var url = $("#ViewEmployeeType").val();
        var hdnFlagVal = angular.element("#hdnEmployeeTypeId").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'EmployeeTypeID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.EmployeeTypeModel.EmployeeTypeID = data[i]["EmployeeTypeID"];
                            $scope.EmployeeTypeModel.EmployeeTypeName = data[i]["EmployeeTypeName"];
                            $scope.EmployeeTypeModel.Status = data[i]["Status"];
                            $('input[type="text"]').attr("disabled", true);
                            $('input[type="checkbox"]').attr("disabled", true);
                            $scope.UpdateEmployeeTypeButton = false;
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.ViewEmployeeTypeNext = function () {
        $scope.UpdateEmployeeTypeButton = true;
        $('input[type="checkbox"]').attr("disabled", false);
        $("#ButtonEdit").hide();
    };
    $scope.CancelEmployeeTypeUpdate = function () {
        window.location.href = window.location.href;
    }
    $scope.UpdateEmployeeType = function () {

        var url = $("#UpdateEmployeeType").val();
        if ($scope.EmployeeTypeModel.EmployeeTypeID == "") {
            var EmployeeTypeID = 0;
        }
        else {
            var EmployeeTypeID = $scope.EmployeeTypeModel.EmployeeTypeID;
        }
        $scope.EmployeeModel.EmployeeTypeID = EmployeeTypeID;
        $.ajax({
            type: "POST",
            url: url,
            data: "{'ParamsEmployeeTypeArray':" + JSON.stringify($scope.EmployeeTypeModel) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != "Failed") {
                    alert("EmployeeType Updated Successfully", {
                        "title": "",
                        "button": "Ok"
                    }, function () {
                        window.location = $("#Index").val();
                    });
                }
            }, error: function (data) {
            }

        });
    }
    $scope.DeleteEmployeeType = function (employeeType) {
        var url = $("#DeleteEmployeeType").val();

        if (url != undefined) {
            $scope.EmployeeTypeModel.EmployeeTypeID = employeeType.EmployeeTypeID;
            confirm("Do you want to Delete  EmployeeType ?", function (done) {
                if (done) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;",
                        url: url,
                        data: "{'EmployeeTypeDetails':" + JSON.stringify($scope.EmployeeTypeModel) + "}",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert('Employee Type  Deleted Successfully', {
                                    "title": "",
                                    "button": "OK"
                                }, function () {
                                    window.location.href = window.location.href;
                                });
                            }
                        }, error: function (data) {
                            alert('Error:' + data, {
                                "title": "",
                                "button": "OK"
                            }, function () {
                            });
                        }
                    });
                }

            }, {
                "title": "",
                "cancel": {
                    "text": "No",
                    "default": false
                },
                "done": {
                    "text": "Yes"
                }
            })
        }
    };

    //EmployeeType End

    //Employee Start
    GetEmployeeList();
    EditEmployeeByID();
    function GetEmployeeList() {
        var url = $("#GetEmployeeList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.EmployeeListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.EmployeeListModel.length; items++) {
                            $scope.EmployeeListModel[items].UpdatedDate = GetLastModifiedDate($scope.EmployeeListModel[items].UpdatedDate);
                            if ($scope.EmployeeListModel[items].Status == true) {
                                $scope.EmployeeListModel[items].Status = 'Active';
                            }
                            else {
                                $scope.EmployeeListModel[items].Status = 'Inactive';
                            }
                        }
                        $scope.sortpropertyName = 'EmployeeName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.SubmitEmployee = function () {
        var url = $("#SaveEmployee").val();

        if ($scope.EmployeeModel.EmployeeID == "") {
            var EmployeeID = 0;
        }
        else {
            var EmployeeID = $scope.EmployeeModel.EmployeeID;
        }
        $scope.EmployeeModel.EmployeeID = EmployeeID;

        if (EmployeeValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsEmployeeArray':" + JSON.stringify($scope.EmployeeModel) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("Employee Created Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    };
    function EmployeeValidation() {
        var flag = true;
        var EmployeeType = $scope.EmployeeModel.EmployeeTypeID;
        var HallmarkCenter = $scope.EmployeeModel.HallmarkCenterID;
        var EmployeeName = $scope.EmployeeModel.EmployeeName;
        var address1 = $scope.EmployeeModel.Address1;
        var address2 = $scope.EmployeeModel.Address2;
        var address3 = $scope.EmployeeModel.Address3;
        var pINCode = $scope.EmployeeModel.PINCode;
        var phoneNumber = $scope.EmployeeModel.PhoneNumber;
        var cellNumber = $scope.EmployeeModel.CellNumber;
        var email = $scope.EmployeeModel.Email;
        var password = $scope.EmployeeModel.Password;

        if (EmployeeName == '') {
            alert('Employee Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (EmployeeType == '' || EmployeeType == null) {
            alert('Please Select EmployeeType.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (HallmarkCenter == '' || HallmarkCenter == null) {
            alert('Please Select Hallmark Center.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address1 == '') {
            alert('Address1 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address2 == '') {
            alert('Address2 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address3 == '') {
            alert('Address3 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (pINCode == '') {
            alert('PIN Code Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (phoneNumber == '') {
            alert('Phone Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (cellNumber == '') {
            alert('Cell Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (email == '') {
            alert('Email Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (password == '') {
            alert('Password Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        return flag;
    }
    function EditEmployeeByID() {
        var url = $("#ViewEmployee").val();
        var hdnFlagVal = angular.element("#hdnemployeeid").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'EmployeeID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.EmployeeModel.EmployeeID = data[i]["EmployeeID"];
                            $scope.EmployeeModel.EmployeeName = data[i]["EmployeeName"];
                            $scope.EmployeeModel.EmployeeTypeID = data[i]["EmployeeTypeID"];
                            $scope.EmployeeModel.HallmarkCenterID = data[i]["HallmarkCenterID"];
                            $scope.EmployeeModel.Address1 = data[i]["Address1"];
                            $scope.EmployeeModel.Address2 = data[i]["Address2"];
                            $scope.EmployeeModel.Address3 = data[i]["Address3"];
                            $scope.EmployeeModel.PINCode = data[i]["PINCode"];
                            $scope.EmployeeModel.PhoneNumber = data[i]["PhoneNumber"];
                            $scope.EmployeeModel.CellNumber = data[i]["CellNumber"];
                            $scope.EmployeeModel.Email = data[i]["Email"];
                            $scope.EmployeeModel.Password = data[i]["Password"];
                            $scope.EmployeeModel.Status = data[i]["Status"];
                            $('input[type="text"],select').attr("disabled", true);
                            $('input[type="checkbox"],select').attr("disabled", true);
                            $scope.UpdateEmployeeButton = false;
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.ViewEmployeeNext = function () {
        $scope.UpdateEmployeeButton = true;
        $('input[type="text"]').attr("disabled", false);
        $('input[type="checkbox"],select').attr("disabled", false);
        $("#EmpName").attr("disabled", true);
        $("#ButtonEdit").hide();
    };
    $scope.CancelEmployeeUpdate = function () {
        window.location.href = window.location.href;
    };
    $scope.UpdateEmployee = function () {
        var url = $("#UpdateEmployee").val();
        if ($scope.EmployeeModel.EmployeeID == "") {
            var EmployeeID = 0;
        }
        else {
            var EmployeeID = $scope.EmployeeModel.EmployeeID;
        }
        $scope.EmployeeModel.EmployeeID = EmployeeID;
        if (EmployeeValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsEmployeeArray':" + JSON.stringify($scope.EmployeeModel) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("Employee Updated Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        })
                    }
                }, error: function (data) {
                }
            });
        }
    };
    $scope.DeleteEmployee = function (employee) {
        var url = $("#DeleteEmployee").val();

        if (url != undefined) {
            $scope.EmployeeModel.EmployeeID = employee.EmployeeID;
            confirm("Do you want to Delete Employee ?", function (done) {
                if (done) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;",
                        url: url,
                        data: "{'EmployeeDetails':" + JSON.stringify($scope.EmployeeModel) + "}",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert('Employee Deleted Successfully', {
                                    "title": "",
                                    "button": "OK"
                                }, function () {
                                    window.location.href = window.location.href;
                                });
                            }
                        }, error: function (data) {
                            alert('Error:' + data, {
                                "title": "",
                                "button": "OK"
                            }, function () {
                            });
                        }
                    });
                }
                else {
                    $("#loadingDiv").hide();
                    return false;
                }
            }, {
                "title": "",
                "cancel": {
                    "text": "No",
                    "default": false
                },
                "done": {
                    "text": "Yes"
                }
            })
        }
    };
    //Employee End

    //Customer Start
    GetCustomerList();
    EditCustomerByID();
    function GetCustomerList() {
        var url = $("#GetCustomerList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.CustomerListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.CustomerListModel.length; items++) {
                            $scope.CustomerListModel[items].UpdatedDate = GetLastModifiedDate($scope.CustomerListModel[items].UpdatedDate);
                            if ($scope.CustomerListModel[items].Status == true) {
                                $scope.CustomerListModel[items].Status = 'Active';
                            }
                            else {
                                $scope.CustomerListModel[items].Status = 'Inactive';
                            }
                        }
                        $scope.sortpropertyName = 'CustomerName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.SubmitCustomer = function () {
        var url = $("#SaveCustomer").val();
        var fdata = new FormData();
        var centreLogo = $("#Logoupload").get(0);
        var files1 = centreLogo.files;
        if (files1.length > 0) {
            for (var i = 0; i < files1.length; i++) {
                // fdata.append(files1[i].name, files1[i]);
                fdata.append("CustomerLogo", files1[i]);
            }
        }
        var agreementLogo = $("#Agreementupload").get(0);
        var files2 = agreementLogo.files;
        if (files2.length > 0) {
            for (var i = 0; i < files2.length; i++) {
                fdata.append("CustomerAgreement", files2[i]);
            }
        }
        var BASPermissionLogo = $("#BASPermissionupload").get(0);
        var files3 = BASPermissionLogo.files;
        if (files3.length > 0) {
            for (var i = 0; i < files3.length; i++) {
                fdata.append("BASPermission", files3[i]);
            }
        }

        if (CustomerValidation()) {
            if (files1.length > 0) {
                if (files2.length > 0) {
                    if (files3.length > 0) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'ParamsCustomerArray':" + JSON.stringify($scope.CustomerModel) + " }",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed" && data != "Data Exists") {
                                    var sucessdata = data;
                                    var profidenity;
                                    if (sucessdata.length > 0) {
                                        profidenity = sucessdata.split('@')[1];
                                    }

                                    $scope.CustomerModel.CustomerID = profidenity;
                                    fdata.append('CustomerDetails', JSON.stringify($scope.CustomerModel));

                                    $("#hdnCentreId").val(profidenity);
                                    var options = {};
                                    options.url = "Handlers/CustomerLogoHandler.ashx";
                                    options.type = "POST";
                                    options.data = fdata;
                                    options.contentType = false;
                                    options.processData = false;
                                    options.success = function (result) {
                                        // alert(result);
                                    };
                                    options.error = function (err) {
                                        // alert(err.responseText)
                                    };

                                    $.ajax(options);
                                    alert("Customer Created Successfully", {
                                        "title": "",
                                        "button": "Ok"
                                    }, function () {
                                        window.location = $("#Index").val();
                                    });

                                }
                                else if (data == "Data Exists") {
                                    alert("Customer Already Exists", {
                                        "title": "",
                                        "button": "Ok"
                                    }, function () {
                                    });
                                }
                            }, error: function (data) {
                            }
                        });
                    }
                    alert("Upload BIS Certificate", {
                        "title": "",
                        "button": "Ok"
                    }, function () {
                    });
                }
                else {
                    alert("Upload Customer Agreement", {
                        "title": "",
                        "button": "Ok"
                    }, function () {
                    });
                }
            } else {
                alert("Upload Customer Logo", {
                    "title": "",
                    "button": "Ok"
                }, function () {
                });
            }
        }

    };
    function CustomerValidation() {
        var flag = true;
        var CustomerName = $scope.CustomerModel.CustomerName;
        var DBAName = $scope.CustomerModel.DBAName;
        var CustomerCode = $scope.CustomerModel.CustomerCode;
        var address1 = $scope.CustomerModel.Address1;
        var address2 = $scope.CustomerModel.Address2;
        var address3 = $scope.CustomerModel.Address3;
        var pinCode = $scope.CustomerModel.PINCode;
        var phoneNumber = $scope.CustomerModel.PhoneNumber;
        var email = $scope.CustomerModel.Email;
        var GSTIN = $scope.CustomerModel.GSTIN;
        var expiryDate = $.trim($('#ExpiryDate').val());
        var certificateNumber = $scope.CustomerModel.CertificateNumber;

        if (CustomerName == '') {
            alert('Customer Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (DBAName == '') {
            alert('DBA Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (CustomerCode == '') {
            alert('Customer Code Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address1 == '') {
            alert('Address1 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address2 == '') {
            alert('Address2 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address3 == '') {
            alert('Address3 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (pinCode == '') {
            alert('PIN Code Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (phoneNumber == '') {
            alert('Phone Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (email == '') {
            alert('Email Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (GSTIN == '') {
            alert('GSTIN Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (expiryDate == '') {
            alert('Expiry Date Should Not Be Empty', {
                "title": "",
                "button": "Ok"
            }, function () {

            });
            flag = false;
            return flag;
        }
        if (certificateNumber == '') {
            alert('Certificate Number Should Not Be Empty', {
                "title": "",
                "button": "Ok"
            }, function () {

            });
            flag = false;
            return flag;
        }
        return flag;
    }
    function EditCustomerByID() {
        var url = $("#ViewCustomer").val();
        var hdnFlagVal = angular.element("#hdnCustomerid").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'CustomerID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.CustomerModel.HallmarkCenterID = data[i]["HallmarkCenterID"];
                            $scope.CustomerModel.CustomerID = data[i]["CustomerID"];
                            $scope.CustomerModel.CustomerName = data[i]["CustomerName"];
                            $scope.CustomerModel.CustomerCode = data[i]["CustomerCode"];
                            $scope.CustomerModel.DBAName = data[i]["DBAName"];
                            $scope.CustomerModel.Address1 = data[i]["Address1"];
                            $scope.CustomerModel.Address2 = data[i]["Address2"];
                            $scope.CustomerModel.Address3 = data[i]["Address3"];
                            $scope.CustomerModel.PINCode = data[i]["PINCode"];
                            $scope.CustomerModel.PhoneNumber = data[i]["PhoneNumber"];
                            $scope.CustomerModel.Email = data[i]["Email"];
                            $scope.CustomerModel.GSTIN = data[i]["GSTIN"];
                            $scope.CustomerModel.Status = data[i]["Status"];
                            if (data[i]["Logo"] != '') {
                                $("#LogoImg").attr('src', data[i]["Logo"]);
                            }
                            else {
                                $("#LogoImg").attr('src', '../Images/default-logo.png');
                            }
                            $scope.CustomerModel.ExpiryDate = GetStartDate(data[i]["ExpiryDate"]);
                            //$('#ExpiryDate').text(data[i]["ExpiryDate"]);
                            $scope.CustomerModel.CertificateNumber = data[i]["CertificateNumber"];

                            if (data[i]["CustomerAgreement"] != '') {
                                $("#AgreementImg").attr('src', data[i]["CustomerAgreement"]);
                            }
                            else {
                                $("#AgreementImg").attr('src', '../Images/default-logo.png');
                            }
                            if (data[i]["BASPermission"] != '') {
                                $("#BASPermissionImg").attr('src', data[i]["BASPermission"]);
                            }
                            else {
                                $("#BASPermissionImg").attr('src', '../Images/default-logo.png');
                            }

                            $('#spnCustomerAgreement').text(data[i]["AgreementName"]);
                            $('#spnBASPermission').text(data[i]["BASPermissionName"]);

                            $('input[type="text"],select').attr("disabled", true);
                            $scope.UpdateCustomerButton = false;
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.ViewNext = function () {
        $scope.UpdateCustomerButton = true;
        $('input[type="text"],select').attr("disabled", false);
        $("#ButtonEdit").hide();
        $('.fileUploadDiv').css({ "background-color": "#fff" })
        $('#ExpiryDate').css({ "background-color": "#fff" })
    };
    $scope.CancelCustomerUpdate = function () {
        window.location.href = window.location.href;
    }
    $scope.UpdateCustomer = function () {
        var url = $("#UpdateCustomer").val();
        var fdata = new FormData();
        var customerLogo = $("#Logoupload").get(0);
        var files1 = customerLogo.files;
        if (files1.length > 0) {
            for (var i = 0; i < files1.length; i++) {
                //fdata.append(files1[i].name, files1[i]);
                fdata.append("CustomerLogo", files1[i]);
            }
        }
        var agreementLogo = $("#Agreementupload").get(0);
        var files2 = agreementLogo.files;
        if (files2.length > 0) {
            for (var i = 0; i < files2.length; i++) {
                fdata.append("CustomerAgreement", files2[i]);
            }
        }
        var BASPermissionLogo = $("#BASPermissionupload").get(0);
        var files3 = BASPermissionLogo.files;
        if (files3.length > 0) {
            for (var i = 0; i < files3.length; i++) {
                fdata.append("BASPermission", files3[i]);
            }
        }
        if (CustomerValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsCustomerArray':" + JSON.stringify($scope.CustomerModel) + " }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed" && data != "Data Exists") {
                        var sucessdata = data;
                        var profidenity;
                        if (sucessdata.length > 0) {
                            profidenity = sucessdata.split('@')[1];
                        }

                        $scope.CustomerModel.CustomerID = profidenity;
                        fdata.append('CustomerDetails', JSON.stringify($scope.CustomerModel));

                        //if (files1.length > 0 && files2.length > 0 && files3.length > 0) {
                        $("#hdnCentreId").val(profidenity);
                        var options = {};
                        options.url = "Handlers/CustomerLogoHandler.ashx";
                        options.type = "POST";
                        options.data = fdata;
                        options.contentType = false;
                        options.processData = false;
                        options.success = function (result) {
                            // alert(result);
                        };
                        options.error = function (err) {
                            // alert(err.responseText)
                        };

                        $.ajax(options);
                        //}

                        alert("Customer Updated Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            $('input[type="text"],select,input[type="file"]').attr("disabled", true);
                            $scope.UpdateCustomerButton = false;
                            $("#ButtonEdit").show();
                            $scope.$apply();
                            window.location = $("#Index").val();
                        });
                    }
                    else if (data == "Data Exists") {
                        alert("Customer Already Exists", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    };
    $scope.DeleteCustomer = function (customer) {
        var url = $("#DeleteCustomer").val();

        if (url != undefined) {
            $scope.CustomerModel.CustomerID = customer.CustomerID;
            confirm("Do you want to Delete Customer ?", function (done) {
                if (done) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;",
                        url: url,
                        data: "{'CustomerDetails':" + JSON.stringify($scope.CustomerModel) + "}",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert('Customer Deleted Successfully', {
                                    "title": "",
                                    "button": "OK"
                                }, function () {
                                    window.location.href = window.location.href;
                                });
                            }
                        }, error: function (data) {
                            alert('Error:' + data, {
                                "title": "",
                                "button": "OK"
                            }, function () {
                            });
                        }
                    });
                }
                else {
                    return false;
                }
            }, {
                "title": "",
                "cancel": {
                    "text": "No",
                    "default": false
                },
                "done": {
                    "text": "Yes"
                }
            })
        }
    };
    //Customer End

    //CustomerPOC Start
    GetCustomerPOCList();
    EditCustomerPOCByID();
    function GetCustomerPOCList() {
        var url = $("#GetCustomerPOCList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.CustomerPOCListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.CustomerPOCListModel.length; items++) {
                            $scope.CustomerPOCListModel[items].UpdatedDate = GetLastModifiedDate($scope.CustomerPOCListModel[items].UpdatedDate);
                            if ($scope.CustomerPOCListModel[items].Status == true) {
                                $scope.CustomerPOCListModel[items].Status = 'Active';
                            }
                            else {
                                $scope.CustomerPOCListModel[items].Status = 'Inactive';
                            }
                        }
                        $scope.sortpropertyName = 'CustomerPOCName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.SubmitCustomerPOC = function () {
        var url = $("#SaveCustomerPOC").val();
        var fdata = new FormData();
        var IdProof = $("#IDProofupload").get(0);
        var files1 = IdProof.files;
        var fileType;

        if (CustomerPOCValidation()) {
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fileType = files1[i].name.split('.')[files1[i].name.split('.').length - 1];
                    fdata.append(files1[i].name, files1[i]);
                }

                if (fileType == "png" || fileType == "jpg" || fileType == "jpeg" || fileType == "pdf") {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ParamsCustomerPOCArray':" + JSON.stringify($scope.CustomerPOCModel) + " }",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                var sucessdata = data;
                                var profidenity;
                                if (sucessdata.length > 0) {
                                    profidenity = sucessdata.split('@')[1];
                                }

                                $scope.CustomerPOCModel.CustomerPOCID = profidenity;
                                fdata.append('CustomerPOCDetails', JSON.stringify($scope.CustomerPOCModel));

                                //$("#hdnCentreId").val(profidenity);
                                var options = {};
                                options.url = "Handlers/CustomerPOCIDProofHandler.ashx";
                                options.type = "POST";
                                options.data = fdata;
                                options.contentType = false;
                                options.processData = false;
                                options.success = function (result) {
                                    // alert(result);
                                };
                                options.error = function (err) {
                                    // alert(err.responseText)
                                };

                                $.ajax(options);

                                alert("CustomerPOC Created Successfully", {
                                    "title": "",
                                    "button": "Ok"
                                }, function () {
                                    window.location = $("#Index").val();
                                });
                            }
                        }, error: function (data) {
                        }
                    });

                }
                else {
                    alert("Please Choose JPG or PNG Image", {
                        "title": "",
                        "button": "Ok"
                    });
                }
            } else {
                alert("Please Upload ID Proof.", {
                    "title": "",
                    "button": "Ok"
                });
            }
        }
    };
    function CustomerPOCValidation() {
        var flag = true;
        var CustomerPOCName = $scope.CustomerPOCModel.CustomerPOCName;
        var Customer = $scope.CustomerPOCModel.CustomerID;
        var address1 = $scope.CustomerPOCModel.Address1;
        var address2 = $scope.CustomerPOCModel.Address2;
        var address3 = $scope.CustomerPOCModel.Address3;
        var pINCode = $scope.CustomerPOCModel.PINCode;
        var phoneNumber = $scope.CustomerPOCModel.PhoneNumber;
        var email = $scope.CustomerPOCModel.Email;

        if (CustomerPOCName == '') {
            alert('CustomerPOC Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (Customer == '' || Customer == null) {
            alert('Please Select Customer.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address1 == '') {
            alert('Address1 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address2 == '') {
            alert('Address2 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (address3 == '') {
            alert('Address3 Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (pINCode == '') {
            alert('PIN Code Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (phoneNumber == '') {
            alert('Phone Number Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (email == '') {
            alert('Email Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        return flag;
    }
    function EditCustomerPOCByID() {
        var url = $("#ViewCustomerPOC").val();
        var hdnFlagVal = angular.element("#hdncustomerpocId").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'CustomerPOCID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {

                        for (var i = 0; i < data.length; i++) {
                            $scope.CustomerPOCModel.CustomerPOCID = data[i]["CustomerPOCID"];
                            $scope.CustomerPOCModel.CustomerPOCName = data[i]["CustomerPOCName"];
                            $scope.CustomerPOCModel.CustomerID = data[i]["CustomerID"];
                            $scope.CustomerPOCModel.Address1 = data[i]["Address1"];
                            $scope.CustomerPOCModel.Address2 = data[i]["Address2"];
                            $scope.CustomerPOCModel.Address3 = data[i]["Address3"];
                            $scope.CustomerPOCModel.PINCode = data[i]["PINCode"];
                            $scope.CustomerPOCModel.PhoneNumber = data[i]["PhoneNumber"];
                            $scope.CustomerPOCModel.Email = data[i]["Email"];
                            $scope.CustomerPOCModel.Status = data[i]["Status"];
                            if (data[i]["IDProof"] != '') {
                                $("#IDProofImg").attr('src', data[i]["IDProof"]);
                            }
                            else {
                                $("#IDProofImg").attr('src', '../Images/default-logo.png');
                            }
                            $('input[type="text"],select').attr("disabled", true);
                            $scope.UpdateCustomerButton = false;
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    $scope.ViewPOCNext = function () {
        $scope.UpdateCustomerPOCButton = true;
        $('input[type="text"],select').attr("disabled", false);
        $("#ButtonEdit").hide();
    };
    $scope.CancelCustomerPOCUpdate = function () {
        window.location.href = window.location.href;
    }
    $scope.UpdateCustomerPOC = function () {
        var url = $("#UpdateCustomerPOC").val();
        if ($scope.CustomerPOCModel.CustomerPOCID == "") {
            var CustomerPOCID = 0;
        }
        else {
            var CustomerPOCID = $scope.CustomerPOCModel.CustomerPOCID;
        }
        $scope.CustomerPOCModel.CustomerPOCID = CustomerPOCID;
        var fdata = new FormData();
        var IdProof = $("#IDProofupload").get(0);
        var files1 = IdProof.files;
        var fileType;


        if (CustomerPOCValidation()) {
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fileType = files1[i].name.split('.')[files1[i].name.split('.').length - 1];
                    fdata.append(files1[i].name, files1[i]);
                }
            }

            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsCustomerPOCArray':" + JSON.stringify($scope.CustomerPOCModel) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {

                        var sucessdata = data;
                        var profidenity;
                        if (sucessdata.length > 0) {
                            profidenity = sucessdata.split('@')[1];
                        }

                        $scope.CustomerPOCModel.CustomerPOCID = profidenity;
                        fdata.append('CustomerPOCDetails', JSON.stringify($scope.CustomerPOCModel));
                        if (files1.length > 0) {
                            var options = {};
                            options.url = "Handlers/CustomerPOCIDProofHandler.ashx";
                            options.type = "POST";
                            options.data = fdata;
                            options.contentType = false;
                            options.processData = false;
                            options.success = function (result) {
                                // alert(result);
                            };
                            options.error = function (err) {
                                // alert(err.responseText)
                            };

                            $.ajax(options);
                        }

                        alert("CustomerPOC Updated Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        })
                    }
                }, error: function (data) {
                }
            });
        }
    };
    $scope.DeleteCustomerPOC = function (customerpoc) {
        var url = $("#DeleteCustomerPOC").val();

        if (url != undefined) {
            $scope.CustomerPOCModel.CustomerPOCID = customerpoc.CustomerPOCID;
            confirm("Do you want to Delete CustomerPOC ?", function (done) {
                if (done) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;",
                        url: url,
                        data: "{'CustomerPOCDetails':" + JSON.stringify($scope.CustomerPOCModel) + "}",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert('CustomerPOC Deleted Successfully', {
                                    "title": "",
                                    "button": "OK"
                                }, function () {
                                    window.location.href = window.location.href;
                                });
                            }
                        }, error: function (data) {
                            alert('Error:' + data, {
                                "title": "",
                                "button": "OK"
                            }, function () {
                            });
                        }
                    });
                }
                else {
                    return false;
                }
            }, {
                "title": "",
                "cancel": {
                    "text": "No",
                    "default": false
                },
                "done": {
                    "text": "Yes"
                }
            })
        }
    };

    //CustomerPOC End

    function GetLastModifiedDate(jsonDate) {

        if (jsonDate != undefined && jsonDate != '/Date(-62135596800000)/') {
            var value = new Date(parseInt(jsonDate.substr(6)));
            var date = (parseInt(value.getDate())).toString().length < 2 ? '0' + (parseInt(value.getDate())).toString() : (parseInt(value.getDate())).toString();
            var month = (parseInt(value.getMonth()) + 1).toString().length < 2 ? '0' + (parseInt(value.getMonth()) + 1).toString() : (parseInt(value.getMonth()) + 1).toString();
            var hours = (parseInt(value.getHours())).toString().length < 2 ? '0' + (parseInt(value.getHours())).toString() : (parseInt(value.getHours())).toString();

            var minutes = (parseInt(value.getMinutes())).toString().length < 2 ? '0' + (parseInt(value.getMinutes())).toString() : (parseInt(value.getMinutes())).toString();

            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            hours = hours < 10 ? '0' + hours : hours;
            var fulldate = month + "/" + date + "/" + value.getFullYear() + " " + hours + ":" + minutes + " " + ampm;
            return new Date(fulldate);
        }
        else {
            return "";
        }
    };
    $scope.sortBy = function (propertyName) {
        $scope.sortorder = ($scope.sortpropertyName === propertyName) ? !$scope.sortorder : false;
        $scope.sortpropertyName = propertyName;
    };
    function GetStartDate(jsonDate) {
        if (jsonDate != undefined) {
            var value = jsonDate.split('/');
            var date = value[0] + "/" + value[1] + "/" + value[2].substring(0, 4)
            return date;
        }
        else {
            return "";
        }
    };
}]);

BISHallmark.controller('ServiceRequestController', ['$scope', function ($scope) {
    ModelDefinations();
    function ModelDefinations() {
        $scope.ServiceRequestListModel = {};
        $scope.ServiceRequestModel = {
            'ServiceRequestID': '',
            'Token': '',
            'CustomerID': '',
            'CustomerPOCID': '',
            'Status': '',
            'CustomerName': '',
            'CustomerPOCName': '',
            'UpdatedDate': '',
            'UpdatedBy': '',
            'CarrierID': '',
            'CarrierName': ''
        };

        $scope.CustomerListModel = {};
        $scope.CustomerPOCListModel = {};
        $scope.CarrierListModel = {};


        $scope.CarrierPickupDetailsModel = {
            'BINID': '',
            'ServiceRequestID': '',
            'CustomerBinID': '',
            'PickupDateTime': '',
            'Notes': '',
            'ItemDescription': '',
            'NoOfItemsRecievedInEachCategory': '',
            'TotalNoOfItems': '',
            'TotalWeightOfItems': '',
            'CustomerSignature': '',
            'CarrierSignature': '',
            'Status': '',
            'UpdatedBy': '',
            'CustomerName': '',
            'CarrierName': '',
            'CustomerPOC': ''
        };
        $scope.ListofBinItemsModel = [{
            'ItemType': '',
            'NoOfItems': '',
            'Weight': ''
        }];
        $scope.ItemsListModel = [];

        $scope.BinItemListModel = [];

        $scope.ListofXRFAssayerModel = [{
            'XRFAssayerID': '',
            'ItemTypeID': '',
            'NoOfItemsAssigned': '',
            'WeightofItemsAssigned': ''
        }];
        $scope.showXRFAssayerMinusButton = false;

        $scope.ManualAssayerListModel = [];
        $scope.XRFAssayerListModel = [];

        $scope.ManualAssayerModel = {
            'ManualAssayerID': '',
            'ItemTypeID': '',
            'NoOfItemsManualAssayer': '',
            'WeightManualAssayer': ''
        };

    }

    GetServiceRequestList();
    function GetServiceRequestList() {
        var url = $("#GetServiceRequestList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.ServiceRequestListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.ServiceRequestListModel.length; items++) {
                            $scope.ServiceRequestListModel[items].UpdatedDate = GetLastModifiedDate($scope.ServiceRequestListModel[items].UpdatedDate);
                        }
                        $scope.sortpropertyName = 'CustomerName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    GetCustomerList();
    function GetCustomerList() {
        var url = $("#GetCustomerList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.CustomerListModel = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    $scope.GetCustomerPOCByCustomer = function (customerID) {
        GetCustomerPOCListByCustomer(customerID);
    };
    function GetCustomerPOCListByCustomer(customerID) {
        var url = $("#GetCustomerPOCListBYCustomerID").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'CustomerID':" + customerID + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.CustomerPOCListModel = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    GetCarrierList();
    function GetCarrierList() {
        var url = $("#GetCarrierList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.CarrierListModel = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    $scope.SaveServiceRequest = function () {
        var url = $("#SaveRequest").val();
        if ($scope.ServiceRequestModel.ServiceRequestID == "") {
            var requestID = 0;
        }
        else {
            var requestID = $scope.ServiceRequestModel.ServiceRequestID;
        }
        $scope.ServiceRequestModel.ServiceRequestID = requestID;

        if (RequestValidation()) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsRequestArray':" + JSON.stringify($scope.ServiceRequestModel) + " }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("Service Request Created Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        });
                    }
                    else {
                        alert(data, {
                            "title": "",
                            "button": "Ok"
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    };

    function RequestValidation() {
        var flag = true;
        var Customer = $scope.ServiceRequestModel.CustomerID;
        var customerPOC = $scope.ServiceRequestModel.CustomerPOCID;
        var carrier = $scope.ServiceRequestModel.CarrierID;

        if (Customer == '' || Customer == null) {
            alert('Please Select Customer.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (customerPOC == '' || customerPOC == null) {
            alert('Please Select Customer POC.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }
        if (carrier == '' || carrier == null) {
            alert('Please Select Carrier.', {
                "title": "",
                "button": "Ok"
            }, function () {
            });
            flag = false;
            return flag;
        }

        return flag;
    }

    VerifyServiceRequest();
    GetStackingDependencyDetails();

    function VerifyServiceRequest() {
        var url = $("#ViewServiceRequestDetails").val();
        var hdnFlagVal = angular.element("#hdnRequestid").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ServiceRequestID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.CarrierPickupDetailsModel.BINID = data[i]["BINID"];
                            $scope.CarrierPickupDetailsModel.CustomerName = data[i]["CustomerName"];
                            $scope.CarrierPickupDetailsModel.CarrierName = data[i]["CarrierName"];
                            $scope.CarrierPickupDetailsModel.CustomerPOC = data[i]["CustomerPOC"];
                            $scope.CarrierPickupDetailsModel.ItemDescription = data[i]["ItemDescription"];
                            $scope.CarrierPickupDetailsModel.TotalNoOfItems = data[i]["TotalNoOfItems"];
                            $scope.CarrierPickupDetailsModel.TotalWeightOfItems = data[i]["TotalWeightOfItems"];
                            if (data[i]["CustomerSignature"] != '') {
                                $("#CustomerSignatureImg").attr('src', data[i]["CustomerSignature"]);
                            }
                            else {
                                $("#CustomerSignatureImg").attr('src', '../Images/default-logo.png');
                            }
                            if (data[i]["CarrierSignature"] != '') {
                                $("#CarrierSignatureImg").attr('src', data[i]["CarrierSignature"]);
                            }
                            else {
                                $("#CarrierSignatureImg").attr('src', '../Images/default-logo.png');
                            }
                            $('input[type="text"]').attr("disabled", true);
                            $('input[type="checkbox"],input[type="file"').attr("disabled", true);
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }
    function GetStackingDependencyDetails() {
        var hdnFlagVal = angular.element("#hdnRequestid").val();
        var url = $('#ViewBinItemsDetails').val();

        if (url != undefined) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: url,
                data: "{'ServiceRequestID':" + hdnFlagVal + "}",
                dataType: "json",
                success: function (data) {
                    if (data != "") {
                        $scope.ListofBinItemsModel = data;
                        $scope.$apply();
                        //for (var i = 0; i < $scope.ListofDependentStackingModel.length; i++) {
                        //    $scope.ListofDependentStackingModel[i].OperatorValue = numberWithCommas($scope.ListofDependentStackingModel[i].OperatorValue.toString());
                        //}

                        //if ($scope.ListofDependentStackingModel.length > 1) {
                        //    $scope.showDependecyAssumptionMinusButton = true;
                        //}
                        //else {
                        //    $scope.showDependecyAssumptionMinusButton = false
                        //}
                        //$scope.$apply();
                        //for (var i = 0; i < $scope.ListofDependentStackingModel.length; i++) {
                        //    $scope.ListofDependentStackingModel[i].ToYear = $scope.ListofDependentStackingModel[i].ToYear.toString();
                        //    $scope.$apply();
                        //}
                    }
                    else {
                        $scope.ListofBinItemsModel = {};
                        $scope.$apply();
                    }

                }, error: function (data) {
                    alert('Error:' + data, {
                        "title": "",
                        "button": "Ok"
                    });
                }
            });
        }
    }

    GetListofItems();
    function GetListofItems() {
        var url = $("#GetItemsList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.ItemsListModel = data;
                        //  $scope.ListofBinItemsModel[0].ItemsList = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    $scope.ApprovePickupDetails = function () {
        var url = $("#ApproveServiceRequest").val();
        var hdnFlagVal = angular.element("#hdnRequestid").val();
        $scope.CarrierPickupDetailsModel.CustomerBinID = "Cust-001";
        $scope.CarrierPickupDetailsModel.ServiceRequestID = hdnFlagVal;

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ParamsBinDetailsArray':" + JSON.stringify($scope.CarrierPickupDetailsModel) + " }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != "Failed") {
                        alert("Bin Created Successfully", {
                            "title": "",
                            "button": "Ok"
                        }, function () {
                            window.location = $("#Index").val();
                        });
                    }
                    else {
                        alert(data, {
                            "title": "",
                            "button": "Ok"
                        });
                    }
                }, error: function (data) {
                }
            });
        }
    };
    $scope.Cancel = function () {
        window.location = $("#Index").val();
    };

    GetBinItemTypes();
    function GetBinItemTypes() {
        var url = $("#GetBinItemTypes").val();
        var hdnFlagVal = angular.element("#hdnRequestid").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ServiceRequestID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.BinItemListModel = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    $scope.AssigntoNewXRFAssayer = function () {
        $scope.ListofXRFAssayerModel.push({
            'XRFAssayerID': '',
            'ItemTypeID': '',
            'NoOfItemsAssigned': '',
            'WeightofItemsAssigned': ''
        });
        $scope.showXRFAssayerMinusButton = true;
    };
    $scope.RemoveXRFAssayer = function (List, i) {
        $scope.ListofXRFAssayerModel.splice(i, 1);
        $scope.$apply();
    };

    GetManualAssayerList();
    GetXRFAssayerList();
    function GetManualAssayerList() {
        var url = $("#GetManualAssayer").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.ManualAssayerListModel = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }
    function GetXRFAssayerList() {
        var url = $("#GetXRFAssayer").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.XRFAssayerListModel = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    function GetLastModifiedDate(jsonDate) {

        if (jsonDate != undefined && jsonDate != '/Date(-62135596800000)/') {
            var value = new Date(parseInt(jsonDate.substr(6)));
            var date = (parseInt(value.getDate())).toString().length < 2 ? '0' + (parseInt(value.getDate())).toString() : (parseInt(value.getDate())).toString();
            var month = (parseInt(value.getMonth()) + 1).toString().length < 2 ? '0' + (parseInt(value.getMonth()) + 1).toString() : (parseInt(value.getMonth()) + 1).toString();
            var hours = (parseInt(value.getHours())).toString().length < 2 ? '0' + (parseInt(value.getHours())).toString() : (parseInt(value.getHours())).toString();

            var minutes = (parseInt(value.getMinutes())).toString().length < 2 ? '0' + (parseInt(value.getMinutes())).toString() : (parseInt(value.getMinutes())).toString();

            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            hours = hours < 10 ? '0' + hours : hours;
            var fulldate = month + "/" + date + "/" + value.getFullYear() + " " + hours + ":" + minutes + " " + ampm;
            return new Date(fulldate);
        }
        else {
            return "";
        }
    };
}]);

BISHallmark.controller('CarrierController', ['$scope', function ($scope) {
    ModelDefinations();
    function ModelDefinations() {
        $scope.CarrierServiceRequestsListModel = {};
        $scope.CarrierPickupDetailsModel = {
            'BINID': '',
            'ServiceRequestID': '',
            'CustomerBinID': '',
            'PickupDateTime': '',
            'Notes': '',
            'ItemDescription': '',
            'NoOfItemsRecievedInEachCategory': '',
            'TotalNoOfItems': '',
            'TotalWeightOfItems': '',
            'CustomerSignature': '',
            'CarrierSignature': '',
            'Status': '',
            'UpdatedBy': '',
            'CustomerName': '',
            'CarrierName': '',
            'CustomerPOC': ''
        };

        $scope.ListofBinItemsModel = [{
            'ItemType': '',
            'NoOfItems': '',
            'Weight': ''
            //,'ItemsList': []
        }];

        $scope.ItemsListModel = [];
    }

    GetCarrierServiceRequestList();
    function GetCarrierServiceRequestList() {
        var url = $("#GetCarrierServiceRequestList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.CarrierServiceRequestsListModel = data;
                        $scope.$apply();
                        for (var items = 0; items < $scope.CarrierServiceRequestsListModel.length; items++) {
                            $scope.CarrierServiceRequestsListModel[items].UpdatedDate = GetLastModifiedDate($scope.CarrierServiceRequestsListModel[items].UpdatedDate);
                        }
                        $scope.sortpropertyName = 'CustomerName';
                        $scope.sortorder = false;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    EditServiceRequestDetails();
    function EditServiceRequestDetails() {
        var url = $("#ViewServiceRequestDetails").val();
        var hdnFlagVal = angular.element("#hdnRequestid").val();

        if (hdnFlagVal != undefined && url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "{'ServiceRequestID':" + hdnFlagVal + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            $scope.CarrierPickupDetailsModel.CustomerName = data[i]["CustomerName"];
                            $scope.CarrierPickupDetailsModel.CarrierName = data[i]["CarrierName"];
                            $scope.CarrierPickupDetailsModel.ServiceRequestID = hdnFlagVal;
                            $scope.CarrierPickupDetailsModel.CustomerPOC = data[i]["CustomerPOC"];
                            //$scope.CarrierPickupDetailsModel.ItemDescription = data[i]["ItemDescription"];
                            //$scope.CarrierPickupDetailsModel.NoOfItemsRecievedInEachCategory = data[i]["NoOfItemsRecievedInEachCategory"];
                            //$scope.CarrierPickupDetailsModel.TotalNoOfItems = data[i]["TotalNoOfItems"];
                            //$scope.CarrierPickupDetailsModel.TotalWeightOfItems = data[i]["TotalWeightOfItems"];
                            //if (data[i]["CentreLogo"] != '') {
                            //    $("#CentreLogoImg").attr('src', data[i]["CentreLogo"]);
                            //}
                            //else {
                            //    $("#CentreLogoImg").attr('src', '../Images/default-logo.png');
                            //}
                            //if (data[i]["BISLogo"] != '') {
                            //    $("#BISLogoImg").attr('src', data[i]["BISLogo"]);
                            //}
                            //else {
                            //    $("#BISLogoImg").attr('src', '../Images/default-logo.png');
                            //}
                            //  $('input[type="text"]').attr("disabled", true);
                            $('.noedit').attr("disabled", true);
                            $scope.$apply();
                        }
                    }
                }, error: function (data) {
                }
            });
        }
    }

    $scope.SavePickupDetails = function () {
        var url = $("#SavePickupDetails").val();
        var fdata = new FormData();
        var customerSignature = $("#CustSignatureupload").get(0);
        var files1 = customerSignature.files;
        if (files1.length > 0) {
            for (var i = 0; i < files1.length; i++) {
                fdata.append("CustomerSignature", files1[i]);
            }
        }
        var carrierSignature = $("#CarrierSignatureupload").get(0);
        var files2 = carrierSignature.files;
        if (files2.length > 0) {
            for (var i = 0; i < files2.length; i++) {
                fdata.append("CarrierSignature", files2[i]);
            }
        }

        var ItemsArray = new Array();
        var itemListFlag = true;
        for (var i = 0; i < $scope.ListofBinItemsModel.length; i++) {
            if ($scope.ListofBinItemsModel[i].ItemType != null && $scope.ListofBinItemsModel[i].ItemType != "" && $scope.ListofBinItemsModel[i].ItemType != undefined &&
                $scope.ListofBinItemsModel[i].NoOfItems != null && $scope.ListofBinItemsModel[i].NoOfItems != "" &&
                $scope.ListofBinItemsModel[i].Weight != null && $scope.ListofBinItemsModel[i].Weight != "") {
                ItemsArray[i] = $scope.ListofBinItemsModel[i];
                itemListFlag = true;
            }
            else {
                itemListFlag = false;
            }
        }

        if (ValidData()) {
            if (itemListFlag) {
                if (files1.length > 0) {
                    if (files2.length > 0) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'ParamsPickupArray':" + JSON.stringify($scope.CarrierPickupDetailsModel) + " }",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed") {
                                    var sucessdata = data;
                                    var profidenity;
                                    if (sucessdata.length > 0) {
                                        profidenity = sucessdata.split('@')[1];
                                    }

                                    $scope.CarrierPickupDetailsModel.BINID = profidenity;
                                    fdata.append('PickupDetails', JSON.stringify($scope.CarrierPickupDetailsModel));

                                    var options = {};
                                    options.url = "Handlers/PickupDetailsHandler.ashx";
                                    options.type = "POST";
                                    options.data = fdata;
                                    options.contentType = false;
                                    options.processData = false;
                                    options.success = function (result) {
                                        // alert(result);
                                    };
                                    options.error = function (err) {
                                        // alert(err.responseText)
                                    };

                                    $.ajax(options);

                                    if (ItemsArray != null && ItemsArray != "") {
                                        SaveBinItems(ItemsArray, $scope.CarrierPickupDetailsModel.BINID);
                                    }

                                    alert("Order Recieved Successfully", {
                                        "title": "",
                                        "button": "Ok"
                                    }, function () {
                                        window.location = $("#Index").val();
                                    });
                                }
                            }, error: function (data) {
                            }
                        });
                    }
                    else {
                        alert("Carrier Signature Should not be Empty.", {
                            "title": "",
                            "button": "Ok"
                        });
                    }
                }
                else {
                    alert("Customer Signature Should not be Empty.", {
                        "title": "",
                        "button": "Ok"
                    });
                }
            }
            else {
                alert('Items List Should not be Empty.', {
                    "title": "",
                    "button": "Ok"
                });
                return false;
            }
        }
    };

    function ValidData() {
        var flag = true;
        var customerName = $scope.CarrierPickupDetailsModel.CustomerName;
        var carrierName = $scope.CarrierPickupDetailsModel.CarrierName;
        var description = $scope.CarrierPickupDetailsModel.ItemDescription;
        var totalItems = $scope.CarrierPickupDetailsModel.TotalNoOfItems;
        var totalWeight = $scope.CarrierPickupDetailsModel.TotalWeightOfItems;

        if (customerName == '') {
            alert('Customer Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            });
            flag = false;
            return flag;
        }
        if (carrierName == '') {
            alert('Carrier Name Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            });
            flag = false;
            return flag;
        }
        if (description == '') {
            alert('Item Description Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            });
            flag = false;
            return flag;
        }
        if (totalItems == '') {
            alert('Total Number of Items Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            });
            flag = false;
            return flag;
        }
        if (totalWeight == '') {
            alert('Total Weight of Items Should Not Be Empty.', {
                "title": "",
                "button": "Ok"
            });
            flag = false;
            return flag;
        }
        return flag;
    }

    function findIndexInData(data, property, value) {
        for (var i = 0, l = data.length ; i < l ; i++) {
            if (data[i][property] === value) {
                return i;
            }
        }
        return -1;
    }

    $scope.AddNewItem = function ($index, ItemList) {
        $scope.ListofBinItemsModel.push({
            "ItemType": "",
            "NoOfItems": "",
            "Weight": ""
        });
        $scope.$apply();
        //var binItem = {
        //    "ItemType": "",
        //    "NoOfItems": "",
        //    "Weight": "",
        //    "ItemsList": []
        //}
        ////$scope.ListofBinItemsModel.push();
        //var list = $scope.ListofBinItemsModel[$index].ItemsList;
        //var value=$scope.ListofBinItemsModel[$index].ItemType;

        //var index = findIndexInData($scope.ItemsListModel, 'ItemTypeID', value);
        //list.splice(index, 1);
        //binItem.ItemsList = list;
        //$scope.ListofBinItemsModel.push(binItem);


    };
    $scope.RemoveItem = function (ItemList, i) {
        $scope.ListofBinItemsModel.splice(i, 1);
        $scope.ItemsTotal();
        $scope.ItemWeight();
    };
    $scope.ItemsTotal = function () {
        var totalItems = 0;
        for (var i = 0; i < $scope.ListofBinItemsModel.length; i++) {
            totalItems = parseFloat(totalItems) + parseFloat($scope.ListofBinItemsModel[i].NoOfItems);
        }
        $scope.CarrierPickupDetailsModel.TotalNoOfItems = totalItems;
    }
    $scope.ItemWeight = function () {
        var totalWeight = 0;
        for (var i = 0; i < $scope.ListofBinItemsModel.length; i++) {
            totalWeight = parseFloat(totalWeight) + parseFloat($scope.ListofBinItemsModel[i].Weight);
        }
        $scope.CarrierPickupDetailsModel.TotalWeightOfItems = totalWeight;
    };

    GetListofItems();
    function GetListofItems() {
        var url = $("#GetItemsList").val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                url: url,
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        $scope.ItemsListModel = data;
                        //  $scope.ListofBinItemsModel[0].ItemsList = data;
                        $scope.$apply();
                    }
                }, error: function (data) {
                }
            });
        }
    }

    function SaveBinItems(ItemsArray, BINID) {
        var url = $('#SaveBinItems').val();
        if (url != undefined) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: url,
                data: "{'itemsArray':" + JSON.stringify(ItemsArray) + ",'BINID':" + BINID + "}",
                dataType: "json",
                success: function (data) {
                }, error: function (data) {
                }
            });
        }
    }

    function GetLastModifiedDate(jsonDate) {

        if (jsonDate != undefined && jsonDate != '/Date(-62135596800000)/') {
            var value = new Date(parseInt(jsonDate.substr(6)));
            var date = (parseInt(value.getDate())).toString().length < 2 ? '0' + (parseInt(value.getDate())).toString() : (parseInt(value.getDate())).toString();
            var month = (parseInt(value.getMonth()) + 1).toString().length < 2 ? '0' + (parseInt(value.getMonth()) + 1).toString() : (parseInt(value.getMonth()) + 1).toString();
            var hours = (parseInt(value.getHours())).toString().length < 2 ? '0' + (parseInt(value.getHours())).toString() : (parseInt(value.getHours())).toString();

            var minutes = (parseInt(value.getMinutes())).toString().length < 2 ? '0' + (parseInt(value.getMinutes())).toString() : (parseInt(value.getMinutes())).toString();

            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            hours = hours < 10 ? '0' + hours : hours;
            var fulldate = month + "/" + date + "/" + value.getFullYear() + " " + hours + ":" + minutes + " " + ampm;
            return new Date(fulldate);
        }
        else {
            return "";
        }
    };
}]);