(function () {
    'use strict';

    angular.module('app')
        .controller('FeaturesCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', FeaturesCtrl]);

    function FeaturesCtrl($scope, $state, $stateParams, $mdDialog) {

        ModelDefinations();
        function ModelDefinations() {
            $scope.ServiceTypeListModel = [];
            $scope.ServiceTypeModel = {
                'ServiceTypeID': '',
                'ServiceTypeCode': '',
                'ServiceTypeName':'',
                'ServiceTypeDesc': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': '',
                'ServiceTypeImage': '',
                'IconName': '',
                'IconType':''
            };
        }

        GetAllServiceTypesList();
        function GetAllServiceTypesList() {
            var url = $("#GetFeaturesList").val();
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
                                $scope.ServiceTypeListModel = data;
                                $scope.$apply();
                                for (var items = 0; items < $scope.ServiceTypeListModel.length; items++) {
                                    if ($scope.ServiceTypeListModel[items].IsActive == true) {
                                        $scope.ServiceTypeListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.ServiceTypeListModel[items].IsActive = 'Inactive';
                                    }
                                    if ($scope.ServiceTypeListModel[items].IconName != "") {

                                        $scope.ServiceTypeListModel[items].IconName = "ServiceTypeImages/" + $scope.ServiceTypeListModel[items].IconName;

                                        if (!UrlExists($scope.ServiceTypeListModel[items].IconName)) {                                            
                                            $scope.ServiceTypeListModel[items].IconName = 'assets/images/feature-icons/icon-place.jpg';
                                        }                                         
                                    }
                                }
                                $scope.sortpropertyName = 'ServiceTypeName';
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

        $scope.SubmitServiceType = function () {
            var url = $("#SaveServiceType").val();
            var serviceTypeID;
            if ($scope.ServiceTypeModel.ServiceTypeID == "") {
                serviceTypeID = 0;
            }
            else {
                serviceTypeID = $scope.ServiceTypeModel.ServiceTypeID;
            }
            $scope.ServiceTypeModel.ServiceTypeID = serviceTypeID;

            var fdata = new FormData();
            var IconLogo = $("#Iconupload").get(0);
            var files1 = IconLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("ServiceTypeLogo", files1[i]);
                }
            }

            $('#loader-container').show();
            if (CheckInSession()) {
               // if (files1.length > 0) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'serviceTypeData':" + JSON.stringify($scope.ServiceTypeModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed" && data != "Data Exists") {
                                var sucessdata = data;
                                var profidenity;
                                if (sucessdata.length > 0) {
                                    profidenity = sucessdata.split('@')[1];
                                }

                                $scope.ServiceTypeModel.ServiceTypeID = profidenity;

                                fdata.append('ServiceTypeDetails', JSON.stringify($scope.ServiceTypeModel));

                                var options = {};
                                options.url = "Handlers/ServiceTypeIconUploadHandler.ashx";
                                options.type = "POST";
                                options.data = fdata;
                                options.contentType = false;
                                options.processData = false;
                                options.success = function (result) {
                                };
                                options.error = function (err) {
                                };

                                $.ajax(options);

                                alert("Lot Feature Saved Successfully");
                                GetAllServiceTypesList();
                                $scope.ServiceTypeModel = {
                                    'ServiceTypeID': '',
                                    'ServiceTypeCode': '',
                                    'ServiceTypeName': '',
                                    'ServiceTypeDesc': '',
                                    'IsActive': '',
                                    'IsDeleted': '',
                                    'UpdatedBy': '',
                                    'ServiceTypeImage': '',
                                    'IconName': '',
                                    'IconType': ''
                                };
                                $("#IconImg").attr('src', 'assets/images/feature-icons/icon-place.jpg');
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                //}
                //else {
                //    alert("Upload Icon");
                //    $('#loader-container').hide();
                //}
            }
            else {
                window.location.href = $("#LogOut").val();
            }
        };
        $scope.EditServiceType = function (serviceTypeID) {
            var url = $("#ViewServiceType").val();
            var hdnFlagVal = serviceTypeID;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ServiceTypeID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.ServiceTypeModel.ServiceTypeID = data[i]["ServiceTypeID"];
                                    $scope.ServiceTypeModel.ServiceTypeName = data[i]["ServiceTypeName"];
                                    $scope.ServiceTypeModel.ServiceTypeDesc = data[i]["ServiceTypeDesc"];
                                    $scope.ServiceTypeModel.IsActive = data[i]["IsActive"];
                                    
                                    if (data[i]["IconName"] != '') {
                                        $("#IconImg").attr('src', "ServiceTypeImages/" + data[i]["IconName"]);
                                    }
                                    else {                                    
                                        $("#IconImg").attr('src', '../assets/images/feature-icons/icon-place.jpg');
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
        };

        $scope.ClearServiceType = function () {
            $scope.ServiceTypeModel = {
                'ServiceTypeID': '',
                'ServiceTypeCode': '',
                'ServiceTypeName': '',
                'ServiceTypeDesc': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': '',
                'ServiceTypeImage': '',
                'IconName': '',
                'IconType': ''
            };
            $("#IconImg").attr('src', '../assets/images/feature-icons/icon-place.jpg');
        };

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
        
        $scope.featurecres = {
            "count": 9,
            "data": [
                {
                    "fname": "Pre Booking",
                    "fdis": "User can do Pre Booking",
                    "fstatus": "Inactive"
                }, {
                    "fname": "Covered Parking",
                    "fdis": "Feature Description here",
                    "fstatus": "Active"
                }, {
                    "fname": "Physically Handicapped  Parking",
                    "fdis": "Feature Description here",
                    "fstatus": "Active"
                },{
                    "fname": "EV Charging",
                    "fdis": "User can charge his electricle bike",
                    "fstatus": "Active"
                }, {
                    "fname": "Car Wash",
                    "fdis": "Feature Description here",
                    "fstatus": "Active"
                }, {
                    "fname": "Bike Wash",
                    "fdis": "Feature Description here",
                    "fstatus": "Active"
                }, {
                    "fname": "Mechanic",
                    "fdis": "Vehicle repairs",
                    "fstatus": "Active"
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

        //select fields   
      
        $scope.featuresddl = {};
        $scope.featuresddl.featuresname = '';
        $scope.featuresddl.features = [
        {'abbrev':'Kiran'},
        {'abbrev':'Srinivas'},
        {'abbrev':'Umakanth'}]

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
     
})(); 
