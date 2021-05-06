(function () {
    'use strict';

    angular.module('app')
        .controller('PassesCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', PassesCtrl]);

    function PassesCtrl($scope, $state, $stateParams, $mdDialog) {

        ModelDefinations();
        function ModelDefinations() {
            $scope.PassListModel = [];
            $scope.PassTypeModel = {
                'PassPriceID': '',
                'PassTypeID': '',
                'PassCode': '',
                'PassName': '',
                'StationAccess': '',
                'Duration': '',
                'StartDate': '',
                'EndDate': '',
                //'NFCApplicable': '',
                // 'NFCCardPrice': '',
                'VehicleTypeID': '',
                'Price': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': ''
            };

            $scope.PassTypeListModel = [];
            $scope.VehicleTypeListModel = [];

            $scope.PassSaleLimitListModel = [];
            $scope.PassSaleLimitModel = {
                'PassSaleLimitID': '',
                'PassTypeID': '',
                'VehicleTypeID': '',
                'LimitPercentage': '',
                'IsActive': '',
                'UpdatedOn': '',
                'UpdatedBy': ''
            };
        }

        //$scope.StationAccessModel = [
        //    { 'name': 'Single Station' },
        //    { 'name': 'Multi Stations' },
        //    { 'name': 'All Stations' }];
        $scope.DurationModel = [
            { 'name': '1 Day(s)' },
            { 'name': '5 Days' },
            { 'name': '7 Days' },
            { 'name': '10 Days' },
            { 'name': '28 Days' },
            { 'name': '30 Days' },
            { 'name': '31 Days' }];


        GetPassList();
        
        function GetPassList() {
            var url = $("#GetPassTypeList").val();
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
                                $scope.PassListModel = data;
                                $scope.$apply();//16122020
                                for (var items = 0; items < $scope.PassListModel.length; items++) {
                                    if ($scope.PassListModel[items].IsActive == true) {
                                        $scope.PassListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.PassListModel[items].IsActive = 'Inactive';
                                    }
                                    if ($scope.PassListModel[items].NFCApplicable == true) {
                                        $scope.PassListModel[items].NFCApplicable = 'Yes';
                                    }
                                    else {
                                        $scope.PassListModel[items].NFCApplicable = 'No';
                                    }
                                }
                                $scope.sortpropertyName = 'PassName';
                                $scope.sortorder = false;
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }

        GetVehicleTypeList();
        GetActivePassTypeList();
        function GetVehicleTypeList() {
            var url = $("#GetActiveVehicleTypes").val();
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
                                $scope.VehicleTypeListModel = data;
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        function GetActivePassTypeList() {
            var url = $("#GetActivePassTypes").val();
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
                                $scope.PassTypeListModel = data;
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }

        $scope.SubmitPassType = function () {

            var url = $("#SavePassType").val();
            var PassPriceID;
            if ($scope.PassTypeModel.PassPriceID == "") {
                PassPriceID = 0;
            }
            else {
                PassPriceID = $scope.PassTypeModel.PassPriceID;
            }
            $scope.PassTypeModel.PassPriceID = PassPriceID;





            $('#loader-container').show();
            if (CheckInSession()) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: "{'PassTypeData':" + JSON.stringify($scope.PassTypeModel) + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data != "Failed") {
                            alert("Pass Created Successfully");
                            $state.go("passes");
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
        };
        ViewPassTypeByID();
        function ViewPassTypeByID() {
            var url = $("#ViewPassType").val();
            var hdnFlagVal = $stateParams.passtypeid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'PassPriceID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.PassTypeModel.PassPriceID = data[i]["PassPriceID"];
                                    $scope.PassTypeModel.PassTypeID = data[i]["PassTypeID"];
                                    $scope.PassTypeModel.PassCode = data[i]["PassCode"];
                                    $scope.PassTypeModel.PassName = data[i]["PassName"];

                                    var passtypename;
                                    for (var k = 0; k < $scope.PassTypeListModel.length; k++) {
                                        if ($scope.PassTypeListModel[k].PassTypeID == $scope.PassTypeModel.PassTypeID) {
                                            passtypename = $scope.PassTypeListModel[k].PassTypeName;
                                        }
                                    }
                                    if (passtypename == 'Event Pass') {
                                        $scope.StationAccessModel = [
                                            { 'name': 'Single Station' }];
                                    }
                                    else {
                                        $scope.StationAccessModel = [
                                            { 'name': 'Single Station' },
                                            { 'name': 'Multi Stations' },
                                            { 'name': 'All Stations' }];
                                    }
                                    $scope.PassTypeModel.StationAccess = data[i]["StationAccess"];
                                    $scope.PassTypeModel.Duration = data[i]["Duration"];

                                    $scope.PassTypeModel.StartDate = new Date(parseInt(data[i]["StartDate"].substr(6)));
                                    $scope.PassTypeModel.EndDate = new Date(parseInt(data[i]["EndDate"].substr(6)));


                                    //$scope.PassTypeModel.StartDate = new Date(data[i]["StartDate"]);
                                    //$scope.PassTypeModel.EndDate = new Date(data[i]["EndDate"]);



                                    // $scope.PassTypeModel.NFCApplicable = data[i]["NFCApplicable"];
                                    // $scope.PassTypeModel.NFCCardPrice = data[i]["NFCCardPrice"];
                                    $scope.PassTypeModel.VehicleTypeID = data[i]["VehicleTypeID"];
                                    $scope.PassTypeModel.Price = data[i]["Price"];
                                    $scope.PassTypeModel.PassDescription = data[i]["PassDescription"];
                                    $scope.PassTypeModel.IsActive = data[i]["IsActive"];
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
        $scope.UpdatePassType = function () {
            var url = $("#UpdatePassType").val();
            $scope.PassTypeModel.PassPriceID = $stateParams.passtypeid;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    //var startDate;
                    //var endDate;
                    //if (typeof $scope.PassTypeModel.StartDate == "string") {
                    //    startDate = $scope.PassTypeModel.StartDate;
                    //}
                    //else {
                    //    startDate = $scope.PassTypeModel.StartDate.toLocaleDateString();
                    //}
                    //if (typeof $scope.PassTypeModel.EndDate == "string") {
                    //    endDate = $scope.PassTypeModel.EndDate;
                    //}
                    //else {
                    //    endDate = $scope.PassTypeModel.EndDate.toLocaleDateString();
                    //}
                    //$scope.PassTypeModel.StartDate = startDate;
                    //$scope.PassTypeModel.EndDate = endDate;

                    


                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'PassTypeData':" + JSON.stringify($scope.PassTypeModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert("Pass Updated Successfully");
                                $state.go("passes");
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
        };

        $scope.ChangeEndDate = function () {
            validEndDate();
        };

        function validEndDate() {
            var sDate = $scope.PassTypeModel.StartDate;
            var eDate = $scope.PassTypeModel.EndDate;
            if (eDate != "" && eDate != null && eDate != undefined) {
                if (eDate < sDate) {
                    alert('select valid End Date');
                    $scope.PassTypeModel.EndDate = "";
                }
            }
        }

        $scope.GetStationAccessByPassType = function (passtype) {
            $scope.PassTypeModel.StationAccess = '';
            var passtypename;
            for (var i = 0; i < $scope.PassTypeListModel.length; i++) {
                if ($scope.PassTypeListModel[i].PassTypeID == passtype) {
                    passtypename = $scope.PassTypeListModel[i].PassTypeName;
                }
            }
            if (passtypename == 'Event Pass') {
                $scope.StationAccessModel = [
                    { 'name': 'Single Station' }];                   
            }
            else {
                $scope.StationAccessModel = [
                    { 'name': 'Single Station' },
                    { 'name': 'Multi Stations' },
                    { 'name': 'All Stations' }];
            }
        };

        //PassSale Limit Code Start
        GetPassSaleLimitList();
        function GetPassSaleLimitList() {
            var url = $("#GetPassSaleLimitList").val();
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
                                $scope.PassSaleLimitListModel = data;
                                for (var items = 0; items < $scope.PassSaleLimitListModel.length; items++) {
                                    if ($scope.PassSaleLimitListModel[items].IsActive == true) {
                                        $scope.PassSaleLimitListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.PassSaleLimitListModel[items].IsActive = 'Inactive';
                                    }
                                }

                                $scope.sortpropertyName = 'PassTypeName';
                                $scope.sortorder = false;
                                $scope.$apply();//16122020
                            }
                            $('#loader-container').hide();
                        }, error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        }
        $scope.RestrictPassSaleLimit = function () {
            var url = $("#RestrictPassSaleLimit").val();
            var PassSaleLimitID;
            if ($scope.PassSaleLimitModel.PassSaleLimitID == "") {
                PassSaleLimitID = 0;
            }
            else {
                PassSaleLimitID = $scope.PassSaleLimitModel.PassSaleLimitID;
            }
            $scope.PassSaleLimitModel.PassSaleLimitID = PassSaleLimitID;
            $('#loader-container').show();
            if (CheckInSession()) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: "{'PassSaleLimitData':" + JSON.stringify($scope.PassSaleLimitModel) + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data == "Success") {
                            alert("Pass Sale Limit Saved Successfully");
                            GetPassSaleLimitList();
                            $scope.PassSaleLimitModel = {
                                'PassSaleLimitID': '',
                                'PassTypeID': '',
                                'VehicleTypeID': '',
                                'LimitPercentage': '',
                                'IsActive': '',
                                'UpdatedOn': '',
                                'UpdatedBy': ''
                            };
                            //$state.go("passes");
                        }
                        else if (data == "Data Exists") {
                            alert("Pass Sale Limit already exist.");
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
        };
        $scope.EditPassSaleLimit = function (passsaleid) {
            var url = $("#EditPassSaleLimit").val();
            var hdnFlagVal = passsaleid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'PassSaleLimitID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.PassSaleLimitModel.PassSaleLimitID = data[i]["PassSaleLimitID"];
                                    $scope.PassSaleLimitModel.PassTypeID = data[i]["PassTypeID"];
                                    $scope.PassSaleLimitModel.VehicleTypeID = data[i]["VehicleTypeID"];
                                    $scope.PassSaleLimitModel.LimitPercentage = data[i]["LimitPercentage"];
                                    $scope.PassSaleLimitModel.IsActive = data[i]["IsActive"];
                                    $scope.$apply();//16122020
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
        };
        $scope.DeletePassSaleLimit = function (passsaleid) {
            var url = $("#DeletePassSaleLimit").val();
            var hdnFlagVal = passsaleid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'PassSaleLimitID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                GetPassSaleLimitList();
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
        };
        $scope.ClearPassSaleLimit = function () {
            $scope.PassSaleLimitModel = {
                'PassSaleLimitID': '',
                'PassTypeID': '',
                'VehicleTypeID': '',
                'LimitPercentage': '',
                'IsActive': '',
                'UpdatedOn': '',
                'UpdatedBy': ''
            };
        };
        //PassSale Limit Code End

        // table code

        $scope.selected = [];
        $scope.limitOptions = [10, 20, 30];

        $scope.options = {
            rowSelection: true,
            multiSelect: true,
            autoSelect: true,
            decapitate: false,
            largeEditDialog: false,
            boundaryLinks: false,
            limitSelect: true,
            pageSelect: true
        };

        $scope.query = {
            order: 'name',
            limit: 10,
            page: 1
        };

        $scope.editComment = function (event, dessert) {
            event.stopPropagation(); // in case autoselect is enabled            

            var promise;

            if ($scope.options.largeEditDialog) {
                promise = $mdEditDialog.large(editDialog);
            } else {
                promise = $mdEditDialog.small(editDialog);
            }

            promise.then(function (ctrl) {
                var input = ctrl.getInput();

                input.$viewChangeListeners.push(function () {
                    input.$setValidity('test', input.$modelValue !== 'test');
                });
            });
        };

        $scope.toggleLimitOptions = function () {
            $scope.limitOptions = $scope.limitOptions ? undefined : [10, 20, 30];
        };

        $scope.getTypes = function () {
            return ['Candy', 'Ice cream', 'Other', 'Pastry'];
        };

        $scope.loadStuff = function () {
            $scope.promise = $timeout(function () {
                // loading
            }, 2000);
        }

        $scope.logItem = function (item) {
            console.log(item.name, 'was selected');
        };

        $scope.logOrder = function (order) {
            console.log('order: ', order);
        };

        $scope.logPagination = function (page, limit) {
            console.log('page: ', page);
            console.log('limit: ', limit);
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

    angular.module('app').directive('validNumber', function () {
        return {
            require: '?ngModel',
            link: function (scope, element, attrs, ngModelCtrl) {
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
        };
    });
})(); 
