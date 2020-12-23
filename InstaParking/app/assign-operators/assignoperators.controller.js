(function () {
    'use strict';

    angular.module('app')
        .controller('AssignoperatorsCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', AssignoperatorsCtrl]);

    function AssignoperatorsCtrl($scope, $state, $stateParams, $mdDialog) {

        ModelDefinations();
        function ModelDefinations() {
            $scope.LocationListModel = [];
            $scope.LotsListModel = [];

            $scope.SearchModel = {
                'LocationID': '',
                'LocationParkingLotID': ''
            };

            $scope.AssignOperatorsListModel = [];
            $scope.OperatorsListModel = [];


            $scope.visible = [];
            $scope.DropdownOperatorListModel = {
                'UserID': '',
                'UserName': ''
            };
            $scope.AddDropdownOperatorListModel = {
                'UserID': '',
                'UserName': ''
            };

            $scope.Addbuttonddlvisible = [];
        }

        GetLocationsofSupervisor();
        GetOperatorsLoginStatusList();
        GetOperatorsBySupervisorID();

        function GetOperatorsLoginStatusList() {

            var url = $("#GetOperatorsLoginStatusList").val();
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

                                $scope.AssignOperatorsListModel = data;
                                for (var i = 0; i < data.length; i++) {
                                    $scope.visible[i] = false;
                                }

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
        function GetOperatorsBySupervisorID() {
            var url = $("#GetOperatorsBySupervisorID").val();
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
                                $scope.OperatorsListModel = data;
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

        function GetLocationsofSupervisor() {
            var url = $("#GetLocationsofSupervisor").val();
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
                                $scope.LocationListModel = data;
                                $scope.LocationListModel.splice(0, 0, { 'LocationID': 'All', 'LocationName': 'All' });
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
        function GetLotsByLocation(locationID) {
            var url = $("#GetLotsByLocationofSupervisor").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LocationID':" + locationID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.LotsListModel = data;
                                $scope.LotsListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
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
        $scope.GetLots = function (locationID) {
            $scope.LotsListModel = [];
            if (locationID != 'All') {
                $scope.SearchModel.LocationParkingLotID = '';
                GetLotsByLocation(locationID);
                //GetOperatorsLoginStatusByLocationandLot($scope.SearchModel);
            }
            else {
                $scope.LotsListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
            }
            GetOperatorsLoginStatusByLocationandLot($scope.SearchModel);
        };
        $scope.GetOperatorLoginStatusbyLot = function (searchModel) {
            GetOperatorsLoginStatusByLocationandLot(searchModel);
        };
        function GetOperatorsLoginStatusByLocationandLot(searchModel) {

            if (searchModel.LocationID == 'All') {
                searchModel.LocationID = 0;
                searchModel.LocationParkingLotID = 0;
            }
            if (searchModel.LocationParkingLotID == 'All') {
                searchModel.LocationParkingLotID = 0;
            }

            if ((searchModel.LocationID != 0 && searchModel.LocationID != "" && searchModel.LocationID != undefined)
                || (searchModel.LocationParkingLotID != 0 && searchModel.LocationParkingLotID != "" && searchModel.LocationParkingLotID != undefined)) {
                var url = $("#GetOperatorsLoginStatusByLocationandLot").val();
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'SearchData':" + JSON.stringify(searchModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data.length > 0) {
                                    $scope.AssignOperatorsListModel = data;
                                    for (var i = 0; i < data.length; i++) {
                                        $scope.visible[i] = false;
                                    }
                                    $scope.OperatorsListModel = [];
                                    $scope.$apply();
                                }
                                else {
                                    $scope.AssignOperatorsListModel = [];
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
            else {
                GetOperatorsLoginStatusList();
                searchModel.LocationID = 'All';
                searchModel.LocationParkingLotID = 'All';
            }
        }

        $scope.ShowUserDropdown = function (index) {
            $scope.OperatorsListModel = [];
            GetOperatorsBySupervisorID();

            for (var i = 0; i < $scope.AssignOperatorsListModel.length; i++) {
                $scope.Addbuttonddlvisible[i] = false;
            }

            for (var j = 0; j < $scope.AssignOperatorsListModel.length; j++) {
                $scope.visible[j] = false;
                for (var k = 0; k < $scope.AssignOperatorsListModel[j].userslist.length; k++) {
                    var indx = j + k + $scope.AssignOperatorsListModel[j].LocationID + $scope.AssignOperatorsListModel[j].LocationParkingLotID +
                        $scope.AssignOperatorsListModel[j].UserID + $scope.AssignOperatorsListModel[j].LocationName.length;
                    $scope.visible[indx] = false;
                }
            }
            $scope.visible[index] = true;
        };
        $scope.HideUserDropdown = function (index) {
            $scope.visible[index] = false;
            $scope.Addbuttonddlvisible[index] = false;
        };
        $scope.AssignUsertoLocationLot = function (selectedUser, index, model, userid) {

            if (selectedUser != undefined) {
                var splitString = selectedUser.toString().split(",");
                var found;
                if (splitString.length <= 3) {
                    for (var i = 0; i < splitString.length; i++) {
                        var stringPart = splitString[i];
                        if (stringPart != userid) {
                            found = false;
                            continue;
                        } else {
                            found = true;
                            break;
                        }
                    }
                }
                else {
                    alert("You can able to assign max 3 operators.");
                    return false;
                }
            }



            if (selectedUser != undefined) {
                if (!found) {
                    var strSelectedUser = selectedUser.toString();

                    $('#loader-container').show();
                    var url_UserStation = $("#AssignOperatortoLot").val();
                    if (CheckInSession()) {
                        if (selectedUser != "" && selectedUser != undefined && selectedUser != 0
                            && model.LocationID != "" && model.LocationID != undefined && model.LocationID != 0
                            && model.LocationParkingLotID != "" && model.LocationParkingLotID != undefined && model.LocationParkingLotID != 0) {
                            $.ajax({
                                type: "POST",
                                url: url_UserStation,
                                data: "{'operatorID':'" + strSelectedUser + "','LotDetails':" + JSON.stringify(model) + ",'absentUserID':'" + userid + "'}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data == "Success") {
                                        alert('Opearator Assigned to Lot Successfully');
                                        $scope.visible[index] = false;
                                        $scope.OperatorsListModel = [];
                                        GetOperatorsLoginStatusByLocationandLot($scope.SearchModel);
                                        $scope.$apply();
                                    }
                                    $('#loader-container').hide();
                                },
                                error: function (data) {
                                    $('#loader-container').hide();
                                }
                            });
                        }
                    }
                    else {
                        window.location.href = $("#LogOut").val();
                    }
                } else {
                    alert("Please select another operator.");
                }
            }
            else {
                alert("Please select Operator.");
            }
        };
        
        $scope.ShowUserAddDropdown = function (index) {
            $scope.OperatorsListModel = [];
            GetOperatorsBySupervisorID();

            for (var j = 0; j < $scope.AssignOperatorsListModel.length; j++) {
                $scope.visible[j] = false;
                for (var k = 0; k < $scope.AssignOperatorsListModel[j].userslist.length; k++) {
                    var indx = j + k + $scope.AssignOperatorsListModel[j].LocationID + $scope.AssignOperatorsListModel[j].LocationParkingLotID +
                        $scope.AssignOperatorsListModel[j].UserID + $scope.AssignOperatorsListModel[j].LocationName.length;
                    $scope.visible[indx] = false;
                }

            }
            for (var i = 0; i < $scope.AssignOperatorsListModel.length; i++) {
                $scope.Addbuttonddlvisible[i] = false;
            }
            $scope.Addbuttonddlvisible[index] = true;
            //$scope.$apply();
        };
        $scope.HideUserAddDropdown = function (index) {
            $scope.Addbuttonddlvisible[index] = false;
            $scope.visible[index] = false;
        };
        $scope.ADDUsertoLocationLot = function (selectedUser, index, model) {

            if (selectedUser != undefined) {
                var splitString = selectedUser.toString().split(",");
                //03122020
                var found;
                var foundChild;
                if (splitString.length <= 3) {
                    for (var i = 0; i < splitString.length; i++) {
                        var stringPart = splitString[i];
                        if (stringPart != model.UserID) {
                            found = false;
                            continue;
                        } else {
                            found = true;
                            break;
                        }
                    }
                    for (var k = 0; k < model.userslist.length; k++) {
                        var stringPart1 = splitString[k];
                        if (stringPart1 != model.userslist[k].UserID) {
                            foundChild = false;
                            continue;
                        } else {
                            foundChild = true;
                            break;
                        }
                    }
                }
                else {
                    alert("You can able to add max 3 operators.");
                    return false;
                }
                //03122020
            }


            //var found = false;
            //if (splitString.length >= 3) {                
            //    alert("You can able to Add max 3 operators.");
            //    return false;
            //}
            //alert(found);
            //alert(foundChild);

            if (selectedUser != undefined) {
                if (!found && !foundChild) {
                    var strSelectedUser = selectedUser.toString();

                    $('#loader-container').show();
                    var url_UserStation = $("#AddOperatortoLot").val();
                    if (CheckInSession()) {
                        if (selectedUser != "" && selectedUser != undefined && selectedUser != 0
                            && model.LocationID != "" && model.LocationID != undefined && model.LocationID != 0
                            && model.LocationParkingLotID != "" && model.LocationParkingLotID != undefined && model.LocationParkingLotID != 0) {
                            $.ajax({
                                type: "POST",
                                url: url_UserStation,
                                data: "{'operatorID':'" + strSelectedUser + "','LotDetails':" + JSON.stringify(model) + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data == "Success") {
                                        alert('Opearator Added to Lot Successfully');
                                        $scope.Addbuttonddlvisible[index] = false;
                                        $scope.OperatorsListModel = [];
                                        GetOperatorsLoginStatusByLocationandLot($scope.SearchModel);
                                        $scope.$apply();
                                    }
                                    $('#loader-container').hide();
                                },
                                error: function (data) {
                                    $('#loader-container').hide();
                                }
                            });
                        }
                    }
                    else {
                        window.location.href = $("#LogOut").val();
                    }
                } else {
                    alert("Please select another operator.");
                }
            }
            else {
                alert("Please select Operator.");
            }
        };

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
        $scope.assignopes = {
            "count": 9,
            "data": [
                {
                    "aslocation": "KPHB",
                    "aslot": "KPH A",
                    "asoperator": "Ramu"
                }, {
                    "aslocation": "KPHB",
                    "aslot": "KPH B",
                    "asoperator": "Samu"
                }, {
                    "aslocation": "KPHB",
                    "aslot": "KPH C",
                    "asoperator": "Raghu"
                }, {
                    "aslocation": "Miyapur",
                    "aslot": "MIY A",
                    "asoperator": "Kranthi"
                }, {
                    "aslocation": "Miyapur",
                    "aslot": "MIY B",
                    "asoperator": "Santhi"
                }, {
                    "aslocation": "Nijampet",
                    "aslot": "NJT A",
                    "asoperator": "Somu"
                }, {
                    "aslocation": "Nijampet",
                    "aslot": "NJT B",
                    "asoperator": "Ramu"
                }
            ]
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
            $scope.limitOptions = $scope.limitOptions ? undefined : [5, 10, 15];
        };
        $scope.getTypes = function () {
            return ['Candy', 'Ice cream', 'Other', 'Pastry'];
        };
        $scope.loadStuff = function () {
            $scope.promise = $timeout(function () {
                // loading
            }, 2000);
        };
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

    angular.module('app').filter('unique', function () {

        return function (items, filterOn) {

            if (filterOn === false) {
                return items;
            }

            if ((filterOn || angular.isUndefined(filterOn)) && angular.isArray(items)) {
                var hashCheck = {}, newItems = [];

                var extractValueToCompare = function (item) {
                    if (angular.isObject(item) && angular.isString(filterOn)) {
                        return item[filterOn];
                    } else {
                        return item;
                    }
                };

                angular.forEach(items, function (item) {
                    var valueToCheck, isDuplicate = false;

                    for (var i = 0; i < newItems.length; i++) {
                        if (angular.equals(extractValueToCompare(newItems[i]), extractValueToCompare(item))) {
                            isDuplicate = true;
                            break;
                        }
                    }
                    if (!isDuplicate) {
                        newItems.push(item);
                    }

                });
                items = newItems;
            }
            return items;
        };
    });
})(); 
