(function () {
    'use strict';

    angular.module('app')
        .controller('VehiclesCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', VehiclesCtrl])

    function VehiclesCtrl($scope, $state, $stateParams, $mdDialog) {
        ModelDefinations();
        function ModelDefinations() {
            $scope.VehicleTypesListModel = [];
            $scope.VehicleTypeModel = {
                'VehicleTypeID': '',
                'VehicleTypeCode': '',
                'VehicleTypeName': '',
                'WheelCount': '',
                'AxleCount':'',
                'VehicleTypeDesc': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedOn': ''
            };
        }
        
        GetVehicleTypeList();
        ViewVehicleTypeByID();
        function GetVehicleTypeList() {
            var url = $("#GetVehicleTypeList").val();
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
                                $scope.VehicleTypesListModel = data;
                                $scope.$apply();
                                for (var items = 0; items < $scope.VehicleTypesListModel.length; items++) {
                                    if ($scope.VehicleTypesListModel[items].IsActive == true) {
                                        $scope.VehicleTypesListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.VehicleTypesListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.sortpropertyName = 'VehicleTypeName';
                                $scope.sortorder = false;
                                $scope.$apply();
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

        function UrlExists(url) {
            var http = new XMLHttpRequest();
            http.open('HEAD', url, false);
            http.send();
            return http.status != 404;
        }
        $scope.SubmitVehicleType = function () {
            var url = $("#SaveVehicleType").val();
            var VehicleTypeID;
            if ($scope.VehicleTypeModel.VehicleTypeID == "") {
                VehicleTypeID = 0;
            }
            else {
                VehicleTypeID = $scope.VehicleTypeModel.VehicleTypeID;
            }
            $scope.VehicleTypeModel.VehicleTypeID = VehicleTypeID;

            var fdata = new FormData();
            var VehicleIconLogo = $("#VehicleIconupload").get(0);
            var files1 = VehicleIconLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("VehicleTypeLogo", files1[i]);
                }
            }


            $('#loader-container').show();
            if (CheckInSession()) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: "{'VehicleTypeData':" + JSON.stringify($scope.VehicleTypeModel) + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data != "Failed" && data != "Data Exists") {
                            var sucessdata = data;
                            var profidenity;
                            if (sucessdata.length > 0) {
                                profidenity = sucessdata.split('@')[1];
                            }

                            $scope.VehicleTypeModel.VehicleTypeID = profidenity;

                            fdata.append('VehicleTypeDetails', JSON.stringify($scope.VehicleTypeModel));

                            var options = {};
                            options.url = "Handlers/VehicleTypeIconUploadHandler.ashx";
                            options.type = "POST";
                            options.data = fdata;
                            options.contentType = false;
                            options.processData = false;
                            options.success = function (result) {
                            };
                            options.error = function (err) {
                            };

                            $.ajax(options);

                            alert("Vehicle Type Created Successfully");
                            $state.go("vehicles");
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
        function ViewVehicleTypeByID() {
            var url = $("#ViewVehicleType").val();
            var hdnFlagVal = $stateParams.vehicletypeid;
           
            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'VehicleTypeID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.VehicleTypeModel.VehicleTypeID = data[i]["VehicleTypeID"];
                                    $scope.VehicleTypeModel.VehicleTypeCode = data[i]["VehicleTypeCode"];
                                    $scope.VehicleTypeModel.VehicleTypeName = data[i]["VehicleTypeName"];
                                    $scope.VehicleTypeModel.WheelCount = data[i]["WheelCount"];
                                    $scope.VehicleTypeModel.AxleCount = data[i]["AxleCount"];
                                    $scope.VehicleTypeModel.VehicleTypeDesc = data[i]["VehicleTypeDesc"];
                                    $scope.VehicleTypeModel.IsActive = data[i]["IsActive"];

                                    if (data[i]["VehicleTypeIcon"] != '') {
                                        $("#VehicleImg").attr('src', "VehicleTypeIcons/" + data[i]["VehicleTypeIcon"]);
                                        if (!UrlExists('VehicleTypeIcons/' + data[i]["VehicleTypeIcon"])) {
                                            $("#VehicleImg").attr('src', 'assets/images/feature-icons/icon-place.jpg');
                                        } 
                                    }
                                    else {
                                        $("#VehicleImg").attr('src', '../assets/images/feature-icons/icon-place.jpg');
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
        $scope.UpdateVehicleType = function () {
            var url = $("#UpdateVehicleType").val();
            $scope.VehicleTypeModel.VehicleTypeID = $stateParams.vehicletypeid;


            var fdata = new FormData();
            var VehicleIconLogo = $("#VehicleIconupload").get(0);
            var files1 = VehicleIconLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("VehicleTypeLogo", files1[i]);
                }
            }

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'VehicleTypeData':" + JSON.stringify($scope.VehicleTypeModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed" && data != "Data Exists") {
                                var sucessdata = data;
                                var profidenity;
                                if (sucessdata.length > 0) {
                                    profidenity = sucessdata.split('@')[1];
                                }

                                $scope.VehicleTypeModel.VehicleTypeID = profidenity;

                                fdata.append('VehicleTypeDetails', JSON.stringify($scope.VehicleTypeModel));

                                var options = {};
                                options.url = "Handlers/VehicleTypeIconUploadHandler.ashx";
                                options.type = "POST";
                                options.data = fdata;
                                options.contentType = false;
                                options.processData = false;
                                options.success = function (result) {
                                };
                                options.error = function (err) {
                                };

                                $.ajax(options);


                                alert("Vehicle Type Updated Successfully");
                                $state.go("vehicles");
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
    // table code

    $scope.selected = [];
        $scope.limitOptions = [5, 10, 15];
        
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
            limit: 5,
            page: 1
        };
        
        $scope.vehicletypes = {
            "count": 9,
            "data": [
                {
                    "vtcode": "VT-123",
                    "vtname": "Two Wheeler",
                    "vtdes": "Xyz Content here",
                    "vtstatus": "Active",
                    "vtcreatedby": "Admin",
                    "vtcreatedon": "01/20/2020 3.44 PM",
                    "vtupdatedby": "Admin",
                    "vtupdatedon": "01/20/2020 5.00 PM"
                }, {
                    "vtcode": "VT-223",
                    "vtname": "Four Wheeler",
                    "vtdes": "Xyz Content here",
                    "vtstatus": "Active",
                    "vtcreatedby": "Admin",
                    "vtcreatedon": "01/20/2020 3.44 PM",
                    "vtupdatedby": "Admin",
                    "vtupdatedon": "01/20/2020 5.00 PM"
                }, {
                    "vtcode": "VT-323",
                    "vtname": "Heavy Wheeler",
                    "vtdes": "Xyz Content here",
                    "vtstatus": "Active",
                    "vtcreatedby": "Admin",
                    "vtcreatedon": "01/20/2020 3.44 PM",
                    "vtupdatedby": "Admin",
                    "vtupdatedon": "01/20/2020 5.00 PM"
                }
            ]
        };
        
        $scope.editComment = function (event, dessert) {
            event.stopPropagation(); // in case autoselect is enabled            
            
            var promise;
            
            if($scope.options.largeEditDialog) {
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
            $scope.limitOptions = $scope.limitOptions ? undefined : [5, 10, 15];
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

        //select fields 

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
