(function () {
    'use strict';

    angular.module('app')
        .controller('CompanyinfoCtrl', ['$scope', CompanyinfoCtrl]);

    function CompanyinfoCtrl($scope) {
        ModelDefinations();
        function ModelDefinations() {
            $scope.AccountModel = {
                'AccountID': '',
                'AccountName': '',
                'Address1': '',
                'Address2': '',
                'ContactNumber': '',
                'AlternateNumber': '',
                'Email': '',
                'Website': '',
                'GSTNumber': '',
                'WhatsAppNumber': '',
                'SupportContactNumber': '',
                'SupportEmailID': '',
                'CompanyLogo': '',
                'IsActive': '',
                'CreatedBy': ''
            };
        }

        GetAccoutDetails();
        $scope.SubmitAccount = function () {
            var companylogosize = 0;
            var accountID;
            var filesizeflag = true;

            var fdata = new FormData();
            var CompanyLogo = $("#Companyupload").get(0);
            var files1 = CompanyLogo.files;
            if (files1.length > 0) {
                for (var j = 0; j < files1.length; j++) {
                    fdata.append("CompanyImg", files1[j]);
                }
                companylogosize = $('#Companyupload')[0].files[0].size;
                if (companylogosize > 20480) {
                    filesizeflag = false;
                }
            }

            var url = $("#SaveAccountDetails").val();
            if ($scope.AccountModel.AccountID == "") {
                accountID = 0;
            }
            else {
                accountID = $scope.AccountModel.AccountID;
            }
            $scope.AccountModel.AccountID = accountID;

            $('#loader-container').show();
            if (CheckInSession()) {
                //if (files1.length == 0 && $("#CompanyImg").val() == '') {
                //    alert("Upload Company Logo");
                //    $('#loader-container').hide();
                //}

                //if (filesizeflag) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'CompanyInfoData':" + JSON.stringify($scope.AccountModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed" && data != "Data Exists") {
                                var sucessdata = data;
                                var profidenity;
                                if (sucessdata.length > 0) {
                                    profidenity = sucessdata.split('@')[1];
                                }

                                if (files1.length > 0) {
                                    $scope.AccountModel.AccountID = profidenity;
                                    fdata.append('AccountDetails', JSON.stringify($scope.AccountModel));

                                    var options = {};
                                    options.url = "Handlers/CompanyLogoHandler.ashx";
                                    options.type = "POST";
                                    options.data = fdata;
                                    options.contentType = false;
                                    options.processData = false;
                                    options.success = function (result) {
                                    };
                                    options.error = function (err) {
                                    };
                                    $.ajax(options);
                                }
                                alert("Company Info Saved Successfully");
                                //  GetSavedUserDetailsByID($scope.EmployeesModel.UserID);                                      
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                //} else {
                //    alert("Upload Employee Photo and Image Size should be less than or equal to 20KB.");
                //    $('#loader-container').hide();
                //}

            }
            else {
                window.location.href = $("#LogOut").val();
            }
        };
        function UrlExists(url) {
            var http = new XMLHttpRequest();
            http.open('HEAD', url, false);
            http.send();
            return http.status != 404;
        }
        function GetAccoutDetails() {
            var url = $("#GetAccountDetails").val();
           
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.AccountModel.AccountID = data[i]["AccountID"];
                                    $scope.AccountModel.AccountName = data[i]["AccountName"];
                                    $scope.AccountModel.Address1 = data[i]["Address1"];
                                    $scope.AccountModel.Address2 = data[i]["Address2"];
                                    $scope.AccountModel.ContactNumber = data[i]["ContactNumber"];
                                    $scope.AccountModel.AlternateNumber = data[i]["AlternateNumber"];
                                    $scope.AccountModel.Email = data[i]["Email"];
                                    $scope.AccountModel.Website = data[i]["Website"];
                                    $scope.AccountModel.GSTNumber = data[i]["GSTNumber"];
                                    $scope.AccountModel.WhatsAppNumber = data[i]["WhatsAppNumber"];
                                    $scope.AccountModel.SupportContactNumber = data[i]["SupportContactNumber"];
                                    $scope.AccountModel.SupportEmailID = data[i]["SupportEmailID"];
                                    $scope.AccountModel.IsActive = data[i]["IsActive"];

                                    if (data[i]["CompanyLogo"]!= '') {
                                        $("#CompanyImg").attr('src', 'Images/' + data[i]["CompanyLogo"]);
                                        if (!UrlExists('Images/' + data[i]["CompanyLogo"])) {
                                            $("#CompanyImg").attr('src', 'assets/images/upload-logo.jpg');
                                        }
                                    }
                                    else {
                                        $("#CompanyImg").attr('src', 'assets/images/upload-logo.jpg');
                                    }                                   
                                    $scope.$apply();
                                }
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
    }
    function CheckInSession() {
        var url = $("#CheckSessionValue").val();
        var flagVal = false;
        // $("#loadingDiv").show();

        $.ajax({
            type: "POST",
            contentType: "application/json;",
            url: url,
            data: "{}",
            async: false,
            dataType: "text",
            success: function (data) {
                if (data == 'true') {
                    flagVal = true;
                }
                else {
                    flagVal = false;
                    // window.location.href = $("#LogOut").val();
                }
            }, error: function (xhr) {
                flagVal = false;
            }
        });

        // alert(flagVal)
        return flagVal;
    }
    angular.module('app').directive('allowNumbers', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, ngModelCtrl) {
                function fromUser(text) {
                    if (text) {
                        var transformedInput = text.replace(/[^0-9]/g, '');

                        if (transformedInput !== text) {
                            ngModelCtrl.$setViewValue(transformedInput);
                            ngModelCtrl.$render();
                        }
                        return transformedInput;
                    }
                    return undefined;
                }
                ngModelCtrl.$parsers.push(fromUser);
            }
        };
    });
})(); 
