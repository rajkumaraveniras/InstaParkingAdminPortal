(function () {
    'use strict';

    angular.module('app')
        .controller('ParkingCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', ParkingCtrl]);

    function ParkingCtrl($scope, $state, $stateParams, $mdDialog) {

        ModelDefinations();
        function ModelDefinations() {
            $scope.ZonesListModel = [];
            $scope.ZonesModel = {
                'ZoneID': '',
                'CityID': '',
                'City': '',
                'ZoneCode': '',
                'ZoneName': '',
                'ZoneDesc': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': ''
            };
            $scope.CityListModel = [];
            $scope.LocationsListModel = [];
            $scope.LocationsModel = {
                'LocationID': '',
                'LocationCode': '',
                'LocationName': '',
                'LocationDesc': '',
                'Address': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': '',
                'Lattitude': '',
                'Longitude': ''
            };
            $scope.LotsListModel = [];
            $scope.LotsModel = {
                'LocationParkingLotID': '',
                'LocationID': '',
                'LocationName': '',
                'ParkingTypeID': '',
                'ParkingTypeName': '',
                'VehicleTypeID': '',
                'VehicleTypeName': '',
                'ParkingBayID': '',
                'ParkingBayName': '',
                'ParentLocationParkingLotID': '',
                'LocationParkingLotCode': '',
                'LocationParkingLotName': '',
                'Lattitude': '',
                'Longitude': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': '',
                'Address': '',
                'PhoneNumber': ''
                , 'LotVehicleAvailabilityID': ''
                , 'LotVehicleAvailabilityName': ''
                , 'IsHoliday': ''
            };
            $scope.ActiveLocationsListModel = [];
            $scope.ActiveParkingTypesListModel = [];
            $scope.ActiveVehicleTypesListModel = [];
            $scope.ActiveParkingBayListModel = [];


            $scope.LotTimingListModel = [];
            $scope.LotTimingsModel = {
                'ParkingLotTimingID': '',
                'LotID': '',
                'DayOfWeek': '',
                'LotOpenTime': '',
                'LotCloseTime': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': ''
                , 'LotOpenTimeHour': ''
                , 'LotOpenTimeMinute': ''
                , 'LotOpenTimeMeridiem': ''
                , 'LotCloseTimeHour': ''
                , 'LotCloseTimeMinute': ''
                , 'LotCloseTimeMeridiem': ''
            };
            $scope.LotDayofWeekModel = [
                { 'name': 'Select' },
                { 'name': 'Monday' },
                { 'name': 'Tuesday' },
                { 'name': 'Wednesday' },
                { 'name': 'Thursday' },
                { 'name': 'Friday' },
                { 'name': 'Saturday' },
                { 'name': 'Sunday' }];

            $scope.ApplicationTypeListModel = [];
            $scope.LotPriceListModel = [];
            $scope.LotPriceModel = {
                'PriceID': '',
                'LocationParkingLotID': '',
                'ApplicationTypeID': '',
                'VehicleTypeID': '',
                'Price': '',
                'Duration': '',
                'IsActive': '',
                'UpdatedBy': ''
            };

            $scope.ActiveServiceTypeListModel = [];

            $scope.LotBaysListModel = [];
            $scope.LotBaysModel = {
                'ParkingBayID': '',
                'LocationParkingLotID': '',
                'VehicleTypeID': '',
                'ParkingBayCode': '',
                'ParkingBayName': '',
                'NumberOfBays': '',
                'ParkingBayRange': '',
                'IsActive': '',
                'IsDeleted': '',
                'UpdatedBy': ''
            };

            $scope.ChargesModel = {
                'ChargesID': '',
                'ClampFee': '',
                'ClampFeeLimit': '',
                'ClampFeefor4W': '',
                'ClampFeeLimitfor4W':'',
                'PriceLimitForTwoWheller': '',
                'PriceLimitForFourWheller': '',
                'IsActive': '',
                'UpdatedOn': '',
                'UpdatedBy': ''
            };



            $scope.LotHoursModel = [
                { 'hour': '01' },
                { 'hour': '02' },
                { 'hour': '03' },
                { 'hour': '04' },
                { 'hour': '05' },
                { 'hour': '06' },
                { 'hour': '07' },
                { 'hour': '08' },
                { 'hour': '09' },
                { 'hour': '10' },
                { 'hour': '11' },
                { 'hour': '12' }
            ];
            $scope.LotMinutesModel = [
                { 'minute': '00' },
                { 'minute': '05' },
                { 'minute': '10' },
                { 'minute': '15' },
                { 'minute': '20' },
                { 'minute': '25' },
                { 'minute': '30' },
                { 'minute': '35' },
                { 'minute': '40' },
                { 'minute': '45' },
                { 'minute': '50' },
                { 'minute': '55' }
            ];
            $scope.LotTimeMeridiemModel = [
                { 'meridiem': 'AM' },
                { 'meridiem': 'PM' }
            ];
            $scope.LotTimingsModel.DayOfWeek = "Select";
            $scope.LotTimingsModel.LotOpenTimeHour = "01";
            $scope.LotTimingsModel.LotOpenTimeMinute = "00";
            $scope.LotTimingsModel.LotOpenTimeMeridiem = "AM";
            $scope.LotTimingsModel.LotCloseTimeHour = "01";
            $scope.LotTimingsModel.LotCloseTimeMinute = "00";
            $scope.LotTimingsModel.LotCloseTimeMeridiem = "AM";

            $scope.TwoWheelerCapacity = 0;
            $scope.FourWheelerCapacity = 0;

            $scope.LotVehicleAvailabilityModel = [];

            //$scope.lotvehAvailabilityName = '';
            //$scope.parkingVehicleTypeName = '';
            $scope.activeCheckboxDisabled = false;
        }

        //Location Code Start
        GetLocationsList();
        EditLocationByID();
        function GetLocationsList() {
            var url = $("#GetLocationsList").val();
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
                                $scope.LocationsListModel = data;
                               // $scope.$apply();
                                for (var items = 0; items < $scope.LocationsListModel.length; items++) {
                                    if ($scope.LocationsListModel[items].IsActive == true) {
                                        $scope.LocationsListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.LocationsListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.sortpropertyName = 'LocationName';
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
        $scope.SubmitStation = function () {
            var url = $("#SaveLocation").val();
            var LocationID;
            if ($scope.LocationsModel.LocationID == "") {
                LocationID = 0;
            }
            else {
                LocationID = $scope.LocationsModel.LocationID;
            }
            $scope.LocationsModel.LocationID = LocationID;
            $('#loader-container').show();
            if (CheckInSession()) {

                if (!Number.isInteger($scope.LocationsModel.Longitude) && !Number.isInteger($scope.LocationsModel.Lattitude)) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LocationData':" + JSON.stringify($scope.LocationsModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                alert("Location Created Successfully");
                                $state.go("parking/stations");
                            }
                            $('#loader-container').hide();
                        },
                        error: function (data) {
                            $('#loader-container').hide();
                        }
                    });
                }
                else {
                    alert("Latitude/Longitude must be a decimal value.");
                    $('#loader-container').hide();
                    return false;
                }
            }
            else {
                window.location.href = $("#LogOut").val();
            }
        };
        function EditLocationByID() {
            var url = $("#ViewLocation").val();
            var hdnFlagVal = $stateParams.locationid;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LocationID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.LocationsModel.LocationID = data[i]["LocationID"];
                                    $scope.LocationsModel.LocationCode = data[i]["LocationCode"];
                                    $scope.LocationsModel.LocationName = data[i]["LocationName"];
                                    $scope.LocationsModel.Address = data[i]["Address"];
                                    $scope.LocationsModel.Lattitude = data[i]["Lattitude"];
                                    $scope.LocationsModel.Longitude = data[i]["Longitude"];
                                    $scope.LocationsModel.IsActive = data[i]["IsActive"];
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
        $scope.UpdateStation = function () {
            var url = $("#UpdateLocation").val();
            $scope.LocationsModel.LocationID = $stateParams.locationid;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {

                    if (!Number.isInteger($scope.LocationsModel.Longitude) && !Number.isInteger($scope.LocationsModel.Lattitude)) {

                        if ($scope.LocationsModel.IsActive == false) {
                            if (confirm('All Lots under this location will be deactivated?')) {

                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: "{'LocationData':" + JSON.stringify($scope.LocationsModel) + "}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data) {
                                        if (data != "Failed") {
                                            alert("Location Updated Successfully");
                                            $state.go("parking/stations");
                                        }
                                        $('#loader-container').hide();
                                    },
                                    error: function (data) {
                                        $('#loader-container').hide();
                                    }
                                });

                            }
                            else {
                                $('#loader-container').hide();
                                return false;
                            }
                        }
                        else {

                            $.ajax({
                                type: "POST",
                                url: url,
                                data: "{'LocationData':" + JSON.stringify($scope.LocationsModel) + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data != "Failed") {
                                        alert("Location Updated Successfully");
                                        $state.go("parking/stations");
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
                        alert("Latitude/Longitude must be a decimal value.");
                        $('#loader-container').hide();
                        return false;
                    }
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //Location Code End

        //Lots Code Start
        GetLotsList();
        EditLotByID();
        function GetLotsList() {
            var url = $("#GetLotsList").val();
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
                                $scope.LotsListModel = data;
                                //$scope.$apply();
                                for (var items = 0; items < $scope.LotsListModel.length; items++) {
                                    if ($scope.LotsListModel[items].IsActive == true) {
                                        $scope.LotsListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.LotsListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.sortpropertyName = 'LocationParkingLotName';
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

        GetActiveLocationsList();
        GetActiveParkingTypesList();
        GetActiveVehicleTypesList();
        GetActiveParkingBayList();
        GetActiveVehicleAvailabilityList();

        function GetActiveLocationsList() {
            var url = $("#GetActiveLocationList").val();
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
                                $scope.ActiveLocationsListModel = data;
                                $scope.$apply();
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
        function GetActiveParkingTypesList() {
            var url = $("#GetActiveParkingTypesList").val();
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
                                $scope.ActiveParkingTypesListModel = data;
                                $scope.$apply();
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
        function GetActiveVehicleTypesList() {
            var url = $("#GetActiveVehicleTypesList").val();
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
                                $scope.ActiveVehicleTypesListModel = data;
                                $scope.$apply();
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
        function GetActiveParkingBayList() {
            var url = $("#GetActiveParkingBayList").val();
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
                                $scope.ActiveParkingBayListModel = data;
                                $scope.$apply();
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
        function GetActiveVehicleAvailabilityList() {
            var url = $("#GetActiveLotVehicleAvailabilityList").val();
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
                                $scope.LotVehicleAvailabilityModel = data;
                                $scope.$apply();
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

        $scope.SubmitLot = function () {
            var url = $("#SaveLot").val();
            var LocationParkingLotID;
            if ($scope.LotsModel.LocationParkingLotID == "") {
                LocationParkingLotID = 0;
            }
            else {
                LocationParkingLotID = $scope.LotsModel.LocationParkingLotID;
            }
            $scope.LotsModel.LocationParkingLotID = LocationParkingLotID;


            var size = 0;
            var size2 = 0;
            var size3 = 0;

            var fdata = new FormData();
            var LotLogo = $("#Lotupload").get(0);
            var files1 = LotLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("LotLogo", files1[i]);
                }
                size = $('#Lotupload')[0].files[0].size;
            }

            //alert(size);
            //alert(files1.size);

            var LotLogo2 = $("#Lotupload2").get(0);
            var files2 = LotLogo2.files;
            if (files2.length > 0) {
                for (var j = 0; j < files2.length; j++) {
                    fdata.append("LotLogo2", files2[j]);
                }
                size2 = $('#Lotupload2')[0].files[0].size;
            }

            // alert(size2);

            var LotLogo3 = $("#Lotupload3").get(0);
            var files3 = LotLogo3.files;
            if (files3.length > 0) {
                for (var k = 0; k < files3.length; k++) {
                    fdata.append("LotLogo3", files3[k]);
                }
                size3 = $('#Lotupload3')[0].files[0].size;
            }

            // alert(size3);

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {


                    if (!Number.isInteger($scope.LotsModel.Longitude) && !Number.isInteger($scope.LotsModel.Lattitude)) {

                        if (files1.length == 0 && files2.length == 0 && files3.length == 0) {
                            alert("Upload Atleast one Lot Image");
                            $('#loader-container').hide();

                        } else {
                            if (size <= 20480 && size2 <= 20480 && size3 <= 20480) {
                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: "{'LotsData':" + JSON.stringify($scope.LotsModel) + "}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data) {
                                        if (data != "Failed" && data != "Data Exists") {
                                            var sucessdata = data;
                                            var profidenity;
                                            if (sucessdata.length > 0) {
                                                profidenity = sucessdata.split('@')[1];
                                            }
                                            $scope.LotsModel.LocationParkingLotID = profidenity;
                                            fdata.append('LotDetails', JSON.stringify($scope.LotsModel));

                                            var options = {};
                                            options.url = "Handlers/LotImageUploadHandler.ashx";
                                            options.type = "POST";
                                            options.data = fdata;
                                            options.contentType = false;
                                            options.processData = false;
                                            options.success = function (result) {
                                            };
                                            options.error = function (err) {
                                            };

                                            $.ajax(options);

                                            alert("Lot Created Successfully");
                                            GetSavedLotDetailsByID($scope.LotsModel.LocationParkingLotID);
                                            GetLotTimingsList($scope.LotsModel.LocationParkingLotID);//New
                                            GetLotPriceList($scope.LotsModel.LocationParkingLotID);//New
                                            // LotCapacityByVehicleType($scope.LotsModel.LocationParkingLotID);//New
                                            //$("input[id=hdnLotID]").val($scope.LotsModel.LocationParkingLotID);
                                            $("#lotButton").attr("disabled", true);
                                            $('input[type="file"]').attr("disabled", true);
                                            //$(".type").attr("disabled", true);
                                            //$('input[type="text"],input[type="checkbox"],input[type="file"]').attr("disabled", true);
                                            //  $state.go("parking/lots");
                                        }
                                        $('#loader-container').hide();
                                    },
                                    error: function (data) {
                                        $('#loader-container').hide();
                                    }
                                });
                            }
                            else {
                                alert('Lot Image Size should be less than or equal to 20KB');
                                $('#loader-container').hide();
                            }
                        }

                    }
                    else {
                        alert("Latitude/Longitude must be a decimal value.");
                        $('#loader-container').hide();
                        return false;
                    }

                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };

        function UrlExists(url) {
            var http = new XMLHttpRequest();
            http.open('HEAD', url, false);
            http.send();
            return http.status != 404;
        }

        function GetSavedLotDetailsByID(lotId) {
            var url = $("#GetSavedLotDetails").val();
            var hdnFlagVal = lotId;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LotID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.LotsModel.LocationParkingLotID = data[i]["LocationParkingLotID"];
                                    $scope.LotsModel.LocationID = data[i]["LocationID"];
                                    $scope.LotsModel.ParkingTypeID = data[i]["ParkingTypeID"];
                                    $scope.LotsModel.VehicleTypeID = data[i]["VehicleTypeID"];
                                    $scope.LotsModel.ParkingBayID = data[i]["ParkingBayID"];
                                    $scope.LotsModel.LocationParkingLotCode = data[i]["LocationParkingLotCode"];
                                    $scope.LotsModel.LocationParkingLotName = data[i]["LocationParkingLotName"];
                                    $scope.LotsModel.Lattitude = data[i]["Lattitude"];
                                    $scope.LotsModel.Longitude = data[i]["Longitude"];
                                    $scope.LotsModel.IsActive = data[i]["IsActive"];
                                    $scope.LotsModel.LotVehicleAvailabilityID = data[i]["LotVehicleAvailabilityID"];
                                    $scope.LotsModel.LotVehicleAvailabilityName = data[i]["LotVehicleAvailabilityName"];
                                                                       

                                    $scope.LotsModel.IsHoliday = data[i]["IsHoliday"];

                                    if ($scope.LotsModel.IsHoliday == true) {
                                        $scope.activeCheckboxDisabled = true;
                                    }
                                    else {
                                        $scope.activeCheckboxDisabled = false;
                                    }


                                    $scope.LotsModel.Address = data[i]["Address"];
                                    $scope.LotsModel.PhoneNumber = data[i]["PhoneNumber"];
                                    if (data[i]["LotImageName"] != '' && data[i]["LotImageName"] != null) {
                                        $("#LotImg").attr('src', 'LotImages/' + data[i]["LotImageName"]);
                                        $("#LotImg").val(data[i]["LotImageName"]);//New
                                        if (!UrlExists('LotImages/' + data[i]["LotImageName"])) {
                                            $("#LotImg").attr('src', 'assets/images/lot.jpg');
                                            $("#LotImg").val(data[i]["LotImageName"]);//New
                                        }
                                    }
                                    else {
                                        $("#LotImg").attr('src', 'assets/images/lot.jpg');
                                        $("#LotImg").val(data[i]["LotImageName"]);//New
                                    }

                                    if (data[i]["LotImageName2"] != '' && data[i]["LotImageName2"] != null) {
                                        $("#LotImg2").attr('src', 'LotImages/' + data[i]["LotImageName2"]);
                                        $("#LotImg2").val(data[i]["LotImageName2"]);//New
                                        if (!UrlExists('LotImages/' + data[i]["LotImageName2"])) {
                                            $("#LotImg2").attr('src', 'assets/images/lot.jpg');
                                            $("#LotImg2").val(data[i]["LotImageName2"]);//New
                                        }
                                    }
                                    else {
                                        $("#LotImg2").attr('src', 'assets/images/lot.jpg');
                                        $("#LotImg2").val(data[i]["LotImageName2"]);//New
                                    }

                                    if (data[i]["LotImageName3"] != '' && data[i]["LotImageName3"] != null) {
                                        $("#LotImg3").attr('src', 'LotImages/' + data[i]["LotImageName3"]);
                                        $("#LotImg3").val(data[i]["LotImageName3"]);//New
                                        if (!UrlExists('LotImages/' + data[i]["LotImageName3"])) {
                                            $("#LotImg3").attr('src', 'assets/images/lot.jpg');
                                            $("#LotImg3").val(data[i]["LotImageName3"]);//New
                                        }
                                    }
                                    else {
                                        $("#LotImageName3").attr('src', 'assets/images/lot.jpg');
                                        $("#LotImg3").val(data[i]["LotImageName3"]);//New
                                    }
                                    $scope.$apply();
                                    // $scope.GetLotVehicleAvailabilityText($scope.LotsModel.LotVehicleAvailabilityID);
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
        function EditLotByID() {
            // var url = $("#ViewLot").val();
            var hdnFlagVal = $stateParams.lotid;

            if (hdnFlagVal != "" && hdnFlagVal != undefined) {
                $("input[id=hdnUpdateLotID]").val(hdnFlagVal);
                $('#loader-container').show();
                //NEW
                GetSavedLotDetailsByID(hdnFlagVal);
                GetLotTimingsList(hdnFlagVal);
                GetLotPriceList(hdnFlagVal);
                GetSelectedLotServicesList(hdnFlagVal);
                GetLotBaysList(hdnFlagVal);
                // LotCapacityByVehicleType(hdnFlagVal);//New
                //NEW
                $('#loader-container').hide();
            }

            //if (hdnFlagVal != undefined && url != undefined) {
            //    $('#loader-container').show();
            //    if (CheckInSession()) {

            //        $.ajax({
            //            type: "POST",
            //            url: url,
            //            data: "{'LotID':" + hdnFlagVal + "}",
            //            contentType: "application/json; charset=utf-8",
            //            dataType: "json",
            //            success: function (data) {
            //                if (data.length > 0) {
            //                    for (var i = 0; i < data.length; i++) {
            //                        $scope.LotsModel.LocationParkingLotID = data[i]["LocationParkingLotID"];
            //                        $scope.LotsModel.LocationID = data[i]["LocationID"];
            //                        $scope.LotsModel.ParkingTypeID = data[i]["ParkingTypeID"];
            //                        $scope.LotsModel.LocationParkingLotCode = data[i]["LocationParkingLotCode"];
            //                        $scope.LotsModel.LocationParkingLotName = data[i]["LocationParkingLotName"];
            //                        $scope.LotsModel.Lattitude = data[i]["Lattitude"];
            //                        $scope.LotsModel.Longitude = data[i]["Longitude"];
            //                        $scope.LotsModel.PhoneNumber = data[i]["PhoneNumber"];
            //                        $scope.LotsModel.IsActive = data[i]["IsActive"];
            //                        $scope.$apply();
            //                    }
            //                }
            //                $('#loader-container').hide();
            //            },
            //            error: function (data) {
            //                $('#loader-container').hide();
            //            }
            //        });
            //    }
            //    else {
            //        window.location.href = $("#LogOut").val();
            //    }
            //}
        }
        $scope.UpdateLot = function () {
            var url = $("#UpdateLot").val();
            $scope.LotsModel.LocationParkingLotID = $stateParams.lotid;

            for (var i = 0; i < $scope.LotVehicleAvailabilityModel.length; i++) {
                if ($scope.LotVehicleAvailabilityModel[i].LotVehicleAvailabilityID === parseInt($scope.LotsModel.LotVehicleAvailabilityID)) {
                    $scope.LotsModel.LotVehicleAvailabilityName = $scope.LotVehicleAvailabilityModel[i].LotVehicleAvailabilityName;
                }
            }


            var filesizeflag = true;

            var fdata = new FormData();
            var LotLogo = $("#Lotupload").get(0);
            var files1 = LotLogo.files;
            if (files1.length > 0) {
                for (var i = 0; i < files1.length; i++) {
                    fdata.append("LotLogo", files1[i]);
                }
                var size = $('#Lotupload')[0].files[0].size;
                if (size > 20480) {
                    filesizeflag = false;
                }
                //alert(size);
            }


            var LotLogo2 = $("#Lotupload2").get(0);
            var files2 = LotLogo2.files;
            if (files2.length > 0) {
                for (var j = 0; j < files2.length; j++) {
                    fdata.append("LotLogo2", files2[j]);
                }
                var size2 = $('#Lotupload2')[0].files[0].size;
                if (size2 > 20480) {
                    filesizeflag = false;
                }
                //alert(size2);
            }


            var LotLogo3 = $("#Lotupload3").get(0);
            var files3 = LotLogo3.files;
            if (files3.length > 0) {
                for (var k = 0; k < files3.length; k++) {
                    fdata.append("LotLogo3", files3[k]);
                }
                var size3 = $('#Lotupload3')[0].files[0].size;
                if (size3 > 20480) {
                    filesizeflag = false;
                }
            }


            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {


                    if (!Number.isInteger($scope.LotsModel.Longitude) && !Number.isInteger($scope.LotsModel.Lattitude)) {

                        if ((files1.length == 0 && files2.length == 0 && files3.length == 0) &&
                            ($("#LotImg").val() == '' && $("#LotImg2").val() == '' && $("#LotImg3").val() == '')) {
                            alert("Upload Atleast one Lot Image");
                            $('#loader-container').hide();

                        }
                        else {
                            if (filesizeflag) {
                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: "{'LotsData':" + JSON.stringify($scope.LotsModel) + "}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data) {
                                        if (data != "Failed" && data != "Data Exists") {
                                            var sucessdata = data;
                                            var profidenity;
                                            if (sucessdata.length > 0) {
                                                profidenity = sucessdata.split('@')[1];
                                            }
                                            $scope.LotsModel.LocationParkingLotID = profidenity;
                                            fdata.append('LotDetails', JSON.stringify($scope.LotsModel));

                                            var options = {};
                                            options.url = "Handlers/LotImageUploadHandler.ashx";
                                            options.type = "POST";
                                            options.data = fdata;
                                            options.contentType = false;
                                            options.processData = false;
                                            options.success = function (result) {
                                            };
                                            options.error = function (err) {
                                            };

                                            $.ajax(options);

                                            alert("Lot Updated Successfully");
                                            GetSavedLotDetailsByID($scope.LotsModel.LocationParkingLotID);
                                            GetLotBaysList($scope.LotsModel.LocationParkingLotID);
                                            // GetLotTimingsList($scope.LotsModel.LocationParkingLotID);//New
                                            // GetLotPriceList($scope.LotsModel.LocationParkingLotID);//New
                                            // LotCapacityByVehicleType($scope.LotsModel.LocationParkingLotID);
                                        }
                                        $('#loader-container').hide();
                                    },
                                    error: function (data) {
                                        $('#loader-container').hide();
                                    }
                                });
                            }
                            else {
                                alert('Lot Image Size should be less than or equal to 20KB');
                                $('#loader-container').hide();
                            }
                        }

                    } else {
                        alert("Latitude/Longitude must be a decimal value.");
                        $('#loader-container').hide();
                        return false;
                    }


                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };

        //Lot Timings Start        
        function DisplayCurrentTime(date) {
            var hours = date.getHours() > 12 ? date.getHours() - 12 : date.getHours();
            var am_pm = date.getHours() >= 12 ? "PM" : "AM";
            hours = hours < 10 ? "0" + hours : hours;
            var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
            var time = hours + ":" + minutes + " " + am_pm;
            return time;
        };
        $scope.SubmitLotTiming = function () {

            var todayDate = new Date().toLocaleString().split(',')[0];

            var Lotopentime = $scope.LotTimingsModel.LotOpenTimeHour + ":" +
                $scope.LotTimingsModel.LotOpenTimeMinute + " " +
                $scope.LotTimingsModel.LotOpenTimeMeridiem;

            var lotOpendateTime = new Date(todayDate + ' ' + Lotopentime);
            //alert(new Date(lotOpendateTime));

            var Lotclosetime = $scope.LotTimingsModel.LotCloseTimeHour + ":" +
                $scope.LotTimingsModel.LotCloseTimeMinute + " " +
                $scope.LotTimingsModel.LotCloseTimeMeridiem;

            var lotClosedateTime = new Date(todayDate + ' ' + Lotclosetime);
            // alert(new Date(lotClosedateTime));


            var url = $("#SaveLotTimings").val();
            $scope.LotTimingsModel.LotID = $scope.LotsModel.LocationParkingLotID;
            //$scope.LotTimingsModel.LotID = 20;
            var dateflag = false;
            //if (new Date($scope.LotTimingsModel.LotOpenTime) >= new Date($scope.LotTimingsModel.LotCloseTime)) {
            if (new Date(lotOpendateTime) >= new Date(lotClosedateTime)) {
                dateflag = false;
            }
            else {
                dateflag = true;
            }

            // alert(dateflag);

            if (LotTimeValidation()) {

                if (dateflag) {

                    //if (!isNaN(Date.parse($scope.LotTimingsModel.LotOpenTime))) {
                    //    $scope.LotTimingsModel.LotOpenTime = DisplayCurrentTime(new Date($scope.LotTimingsModel.LotOpenTime));
                    //}
                    //if (!isNaN(Date.parse($scope.LotTimingsModel.LotCloseTime))) {
                    //    $scope.LotTimingsModel.LotCloseTime = DisplayCurrentTime(new Date($scope.LotTimingsModel.LotCloseTime));
                    //}
                    $scope.LotTimingsModel.LotOpenTime = Lotopentime;
                    $scope.LotTimingsModel.LotCloseTime = Lotclosetime;

                    var ParkingLotTimingID;
                    if ($scope.LotTimingsModel.ParkingLotTimingID == "") {
                        ParkingLotTimingID = 0;
                    }
                    else {
                        ParkingLotTimingID = $scope.LotTimingsModel.ParkingLotTimingID;
                    }
                    $scope.LotTimingsModel.ParkingLotTimingID = ParkingLotTimingID;

                    if ($scope.LotTimingsModel.LotID != "" && $scope.LotTimingsModel.LotID != undefined) {
                        if (url != undefined) {
                            $('#loader-container').show();
                            if (CheckInSession()) {
                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: "{'timingsData':" + JSON.stringify($scope.LotTimingsModel) + "}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data) {
                                        if (data == "Success") {
                                            GetLotTimingsList($scope.LotTimingsModel.LotID);
                                            $scope.LotTimingsModel = {
                                                'ParkingLotTimingID': '',
                                                'LotID': '',
                                                'DayOfWeek': '',
                                                'LotOpenTime': '',
                                                'LotCloseTime': '',
                                                'IsActive': '',
                                                'IsDeleted': '',
                                                'UpdatedBy': ''
                                                , 'LotOpenTimeHour': ''
                                                , 'LotOpenTimeMinute': ''
                                                , 'LotOpenTimeMeridiem': ''
                                                , 'LotCloseTimeHour': ''
                                                , 'LotCloseTimeMinute': ''
                                                , 'LotCloseTimeMeridiem': ''
                                            };
                                            $scope.LotTimingsModel.DayOfWeek = "Select";
                                            $scope.LotTimingsModel.LotOpenTimeHour = "01";
                                            $scope.LotTimingsModel.LotOpenTimeMinute = "00";
                                            $scope.LotTimingsModel.LotOpenTimeMeridiem = "AM";
                                            $scope.LotTimingsModel.LotCloseTimeHour = "01";
                                            $scope.LotTimingsModel.LotCloseTimeMinute = "00";
                                            $scope.LotTimingsModel.LotCloseTimeMeridiem = "AM";
                                            $scope.$apply();
                                        }
                                        else if (data == "Data Exists") {
                                            alert("Day already exist.");
                                        }
                                        //else {
                                        //    alert(data);
                                        //}

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
                    else {
                        alert('you need to create Lot');
                    }
                }
                else {
                    alert('Lot Close Time must be greater than Lot Open Time.');
                }
            }

        };
        function GetLotTimingsList(lotid) {
            var url = $("#GetLotTimings").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LotID':'" + lotid + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.LotTimingListModel = data;
                                for (var items = 0; items < $scope.LotTimingListModel.length; items++) {
                                    if ($scope.LotTimingListModel[items].IsActive == true) {
                                        $scope.LotTimingListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.LotTimingListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.$apply();
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
        $scope.EditLotTime = function (lottimingID) {
            var url = $("#GetLotTimingByID").val();
            var hdnFlagVal = lottimingID;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ParkingLotTimingID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.LotTimingsModel.ParkingLotTimingID = data[i]["ParkingLotTimingID"];
                                    $scope.LotTimingsModel.LotID = data[i]["LotID"];
                                    $scope.LotTimingsModel.DayOfWeek = data[i]["DayOfWeek"];

                                    //$scope.LotTimingsModel.LotOpenTime = data[i]["LotOpenTime"];
                                    //$scope.LotTimingsModel.LotCloseTime = data[i]["LotCloseTime"];                                    

                                    //var Sdat = "Thu Jan 01 1970" + " " + $scope.LotTimingsModel.LotOpenTime;
                                    //$scope.LotTimingsModel.LotOpenTime = new Date(Sdat);

                                    //var Edat = "Thu Jan 01 1970" + " " + $scope.LotTimingsModel.LotCloseTime;
                                    //$scope.LotTimingsModel.LotCloseTime = new Date(Edat);

                                    $scope.LotTimingsModel.LotOpenTimeHour = data[i]["LotOpenTime"].split(':')[0];
                                    $scope.LotTimingsModel.LotOpenTimeMinute = data[i]["LotOpenTime"].split(':')[1].split(' ')[0];
                                    $scope.LotTimingsModel.LotOpenTimeMeridiem = data[i]["LotOpenTime"].split(':')[1].split(' ')[1];
                                    $scope.LotTimingsModel.LotCloseTimeHour = data[i]["LotCloseTime"].split(':')[0];
                                    $scope.LotTimingsModel.LotCloseTimeMinute = data[i]["LotCloseTime"].split(':')[1].split(' ')[0];
                                    $scope.LotTimingsModel.LotCloseTimeMeridiem = data[i]["LotCloseTime"].split(':')[1].split(' ')[1];

                                    $scope.LotTimingsModel.IsActive = data[i]["IsActive"];
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
        $scope.DeleteLotTime = function (lotTimeID) {
            var url = $("#DeleteLotTime").val();
            var hdnFlagVal = lotTimeID;
            var hdnupdatLotID = $("#hdnUpdateLotID").val();
            if (hdnupdatLotID != "" && hdnupdatLotID != undefined) {
                $scope.LotTimingsModel.LotID = hdnupdatLotID;
            }
            else {
                $scope.LotTimingsModel.LotID = $scope.LotsModel.LocationParkingLotID;
            }


            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ParkingLotTimingID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                GetLotTimingsList($scope.LotTimingsModel.LotID);
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
        function LotTimeValidation() {
            var flag = true;
            var dayofweek = $scope.LotTimingsModel.DayOfWeek;
            if (dayofweek == "Select") {
                alert('Please select Day of Week');
                flag = false;
                return flag;
            }
            return flag;
        }
        //Lot Timings End

        //Lot Price Start
        GetApplicationTypeList();
        function GetApplicationTypeList() {
            var url = $("#GetApplicationTypesList").val();
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
                                $scope.ApplicationTypeListModel = data;
                                $scope.$apply();
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
        $scope.SubmitLotPrice = function () {
            var url = $("#SaveLotPrice").val();
            $scope.LotPriceModel.LocationParkingLotID = $scope.LotsModel.LocationParkingLotID;
            //$scope.LotPriceModel.LocationParkingLotID = 20;

            var PriceID;
            if ($scope.LotPriceModel.PriceID == "") {
                PriceID = 0;
            }
            else {
                PriceID = $scope.LotPriceModel.PriceID;
            }
            $scope.LotPriceModel.PriceID = PriceID;

            if ($scope.LotPriceModel.LocationParkingLotID != "" && $scope.LotPriceModel.LocationParkingLotID != undefined) {
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'lotPriceData':" + JSON.stringify($scope.LotPriceModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data == "Success") {
                                    GetLotPriceList($scope.LotPriceModel.LocationParkingLotID);
                                    $scope.LotPriceModel = {
                                        'PriceID': '',
                                        'LocationParkingLotID': '',
                                        'ApplicationTypeID': '',
                                        'VehicleTypeID': '',
                                        'Price': '',
                                        'Duration': '',
                                        'IsActive': '',
                                        'UpdatedBy': ''
                                    };
                                    $scope.$apply();
                                }
                                else if (data == "Data Exists") {
                                    alert("Price already exist.");
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
            else {
                alert('you need to create Lot');
            }
        };
        function GetLotPriceList(lotid) {
            var url = $("#GetLotPrices").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LotID':'" + lotid + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.LotPriceListModel = data;
                                for (var items = 0; items < $scope.LotPriceListModel.length; items++) {
                                    if ($scope.LotPriceListModel[items].IsActive == true) {
                                        $scope.LotPriceListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.LotPriceListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.$apply();
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
        $scope.EditLotPrice = function (priceID) {
            var url = $("#GetLotPriceByID").val();
            var hdnFlagVal = priceID;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'PriceID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.LotPriceModel.PriceID = data[i]["PriceID"];
                                    $scope.LotPriceModel.LocationParkingLotID = data[i]["LocationParkingLotID"];
                                    $scope.LotPriceModel.VehicleTypeID = data[i]["VehicleTypeID"];
                                    $scope.LotPriceModel.ApplicationTypeID = data[i]["ApplicationTypeID"];
                                    $scope.LotPriceModel.Price = data[i]["Price"];
                                    $scope.LotPriceModel.Duration = data[i]["Duration"];
                                    $scope.LotPriceModel.IsActive = data[i]["IsActive"];
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
        $scope.DeleteLotPrice = function (priceID) {
            var url = $("#DeleteLotPrice").val();
            var hdnFlagVal = priceID;

            var hdnupdatLotID = $("#hdnUpdateLotID").val();
            if (hdnupdatLotID != "" && hdnupdatLotID != undefined) {
                $scope.LotPriceModel.LocationParkingLotID = hdnupdatLotID;
            } else {
                $scope.LotPriceModel.LocationParkingLotID = $scope.LotsModel.LocationParkingLotID;
            }


            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'PriceID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                GetLotPriceList($scope.LotPriceModel.LocationParkingLotID);
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
        //Lot Price End

        //Other code Start
        GetActiveServiceTypesList();
        function GetActiveServiceTypesList() {
            var url = $("#GetActiveServiceTypesList").val();
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
                                $scope.ActiveServiceTypeListModel = data;
                                $scope.$apply();
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
        function GetActiveServiceTypesListforEdit() {
            var url = $("#GetActiveServiceTypesListEdit").val();
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
                                $scope.ActiveServiceTypeListModel = data;
                                $scope.$apply();
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
        $scope.SubmitLotServices = function () {

            $scope.ActiveServiceTypeListModel = $scope.ActiveServiceTypeListModel;
            $scope.serviceTypeArray = [];
            for (var i = 0; i < $scope.ActiveServiceTypeListModel.length; i++) {
                $scope.serviceTypeArray.push($scope.ActiveServiceTypeListModel[i]);
            }

            var lotID = $scope.LotsModel.LocationParkingLotID;
            // var lotID = 20;//static.should change to above line

            //var x = $("#hdnLotID").val();
            var url = $("#SaveLotServices").val();

            if (lotID != "" && lotID != undefined) {
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'servicesArray':" + JSON.stringify($scope.serviceTypeArray) + ",'LotID':'" + lotID + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed") {
                                    GetSelectedLotServicesList(lotID);
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
            else {
                alert('you need to create Lot');
                //$('#loader-container').hide();
            }

        };
        function GetSelectedLotServicesList(lotid) {
            var url = $("#GetSelectedLotServiceTypesList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LotID':'" + lotid + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.ActiveServiceTypeListModel = data;
                                $scope.$apply();
                            }
                            else {
                                GetActiveServiceTypesListforEdit();
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
        //Other code End

        //LotParking Bay Start
        $scope.SubmitLotBays = function () {
            var url = $("#SaveLotBays").val();
            $scope.LotBaysModel.LocationParkingLotID = $scope.LotsModel.LocationParkingLotID;
            //$scope.LotBaysModel.LocationParkingLotID = 20;

            var ParkingBayID;
            if ($scope.LotBaysModel.ParkingBayID == "") {
                ParkingBayID = 0;
            }
            else {
                ParkingBayID = $scope.LotBaysModel.ParkingBayID;
            }
            $scope.LotBaysModel.ParkingBayID = ParkingBayID;


            var lotVehicleAvailability;
            var parkingBayVehicleType;

            if ($scope.LotsModel.LotVehicleAvailabilityName == "" || $scope.LotsModel.LotVehicleAvailabilityName == undefined) {
                for (var i = 0; i < $scope.LotVehicleAvailabilityModel.length; i++) {
                    if ($scope.LotVehicleAvailabilityModel[i].LotVehicleAvailabilityID === $scope.LotsModel.LotVehicleAvailabilityID) {
                        lotVehicleAvailability = $scope.LotVehicleAvailabilityModel[i].LotVehicleAvailabilityName;
                    }
                }
            }
            else {
                lotVehicleAvailability = $scope.LotsModel.LotVehicleAvailabilityName;
            }
            for (var j = 0; j < $scope.ActiveVehicleTypesListModel.length; j++) {
                if ($scope.ActiveVehicleTypesListModel[j].VehicleTypeID === parseInt($scope.LotBaysModel.VehicleTypeID)) {
                    parkingBayVehicleType = $scope.ActiveVehicleTypesListModel[j].VehicleTypeName;
                }
            }


            var flag = false;
            if ((lotVehicleAvailability === 'TWO WHEELER') && (parkingBayVehicleType === '2 Wheeler')) {
                flag = true;
            }
            else if ((lotVehicleAvailability === 'FOUR WHEELER') && (parkingBayVehicleType === '4 Wheeler')) {
                flag = true;
            }
            else if (lotVehicleAvailability === 'BOTH') {
                flag = true;
            }

            if ($scope.LotBaysModel.LocationParkingLotID != "" && $scope.LotBaysModel.LocationParkingLotID != undefined) {

                if (flag) {

                    if (url != undefined) {
                        $('#loader-container').show();
                        if (CheckInSession()) {
                            $.ajax({
                                type: "POST",
                                url: url,
                                data: "{'lotBaysData':" + JSON.stringify($scope.LotBaysModel) + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data != "Failed") {
                                        GetLotBaysList($scope.LotBaysModel.LocationParkingLotID);
                                        // LotCapacityByVehicleType($scope.LotBaysModel.LocationParkingLotID);
                                        $scope.LotBaysModel = {
                                            'ParkingBayID': '',
                                            'LocationParkingLotID': '',
                                            'VehicleTypeID': '',
                                            'ParkingBayCode': '',
                                            'ParkingBayName': '',
                                            'NumberOfBays': '',
                                            'ParkingBayRange': '',
                                            'IsActive': '',
                                            'IsDeleted': '',
                                            'UpdatedBy': ''
                                        };
                                        $scope.$apply();
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
                else {
                    alert('Only ' + lotVehicleAvailability+' Parking Available in Lot');
                }
            }
            else {
                alert('you need to create Lot');
            }


        };
        function GetLotBaysList(lotid) {
            var url = $("#GetLotBays").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LotID':'" + lotid + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.LotBaysListModel = data;
                                for (var items = 0; items < $scope.LotBaysListModel.length; items++) {
                                    if ($scope.LotBaysListModel[items].IsActive == true) {
                                        $scope.LotBaysListModel[items].IsActive = 'Active';
                                    }
                                    else {
                                        $scope.LotBaysListModel[items].IsActive = 'Inactive';
                                    }
                                }
                                $scope.$apply();
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
        $scope.EditLotBay = function (lotBayID) {
            var url = $("#GetLotBayByID").val();
            var hdnFlagVal = lotBayID;

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ParkingBayID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                for (var i = 0; i < data.length; i++) {
                                    $scope.LotBaysModel.ParkingBayID = data[i]["ParkingBayID"];
                                    $scope.LotBaysModel.LocationParkingLotID = data[i]["LocationParkingLotID"];
                                    $scope.LotBaysModel.VehicleTypeID = data[i]["VehicleTypeID"];
                                    $scope.LotBaysModel.NumberOfBays = data[i]["NumberOfBays"];
                                    $scope.LotBaysModel.ParkingBayRange = data[i]["ParkingBayRange"];
                                    $scope.LotBaysModel.IsActive = data[i]["IsActive"];
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
        $scope.DeleteLotBay = function (lotBayID) {
            var url = $("#DeleteLotBay").val();
            var hdnFlagVal = lotBayID;

            var hdnupdatLotID = $("#hdnUpdateLotID").val();
            if (hdnupdatLotID != "" && hdnupdatLotID != undefined) {
                $scope.LotBaysModel.LocationParkingLotID = hdnupdatLotID;
            } else {
                $scope.LotBaysModel.LocationParkingLotID = $scope.LotsModel.LocationParkingLotID;
            }

            if (hdnFlagVal != undefined && url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'ParkingBayID':" + hdnFlagVal + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                GetLotBaysList($scope.LotBaysModel.LocationParkingLotID);
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

        function LotCapacityByVehicleType(lotid) {
            var url = $("#GetLotCapacityByVehicleType").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LotID':'" + lotid + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.TwoWheelerCapacity = data.split('-')[0];
                                $scope.FourWheelerCapacity = data.split('-')[1];
                                $scope.$apply();
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
        //LotParking Bay End

        $scope.ChangeActiveCheckboxByHoliday = function (isHoliday) {
            //alert(isHoliday);
            if (isHoliday) {
                $scope.activeCheckboxDisabled = true;
                $scope.LotsModel.IsActive = false;
            }
            else {
                $scope.activeCheckboxDisabled = false;
            }
        };
        //Lots Code End

        //Charges COde Start
        GetChargesData();
        function GetChargesData() {
            var url = $("#GetChargesData").val();
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
                                $scope.ChargesModel.ChargesID = data[0]["ChargesID"];
                                $scope.ChargesModel.ClampFee = data[0]["ClampFee"];
                                $scope.ChargesModel.ClampFeeLimit = data[0]["ClampFeeLimit"];
                                $scope.ChargesModel.ClampFeefor4W = data[0]["ClampFeefor4W"];
                                $scope.ChargesModel.ClampFeeLimitfor4W = data[0]["ClampFeeLimitfor4W"];
                                $scope.ChargesModel.PriceLimitForTwoWheller = data[0]["PriceLimitForTwoWheller"];
                                $scope.ChargesModel.PriceLimitForFourWheller = data[0]["PriceLimitForFourWheller"];
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
        $scope.EditCharges = function () {
            var url = $("#SaveCharges").val();
            var ChargesID;
            if ($scope.ChargesModel.ChargesID == "") {
                ChargesID = 0;
            }
            else {
                ChargesID = $scope.ChargesModel.ChargesID;
            }
            $scope.ChargesModel.ChargesID = ChargesID;

            var clampfee = $scope.ChargesModel.ClampFee;
            var limitclampfee = $scope.ChargesModel.ClampFeeLimit;

            var clampfeefor4w = $scope.ChargesModel.ClampFeefor4W;
            var limitclampfeefor4w = $scope.ChargesModel.ClampFeeLimitfor4W;


            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    if (parseFloat(clampfee) <= parseFloat(limitclampfee)) {
                        if (parseFloat(clampfeefor4w) <= parseFloat(limitclampfeefor4w)) {

                            $.ajax({
                                type: "POST",
                                url: url,
                                data: "{'chargesData':" + JSON.stringify($scope.ChargesModel) + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    if (data != "Failed") {
                                        alert('Charges Saved Successfully')
                                        GetChargesData();
                                        $scope.$apply();
                                    }
                                    $('#loader-container').hide();
                                },
                                error: function (data) {
                                    $('#loader-container').hide();
                                }
                            });

                        } else {
                            alert('Clamp Fee for 4W must be less than or equal to GO Clamp Fee Limit for 4W.');
                            $('#loader-container').hide();
                            return false;
                        }

                    }
                    else {
                        alert('Clamp Fee for 2W must be less than or equal to GO Clamp Fee Limit for 2W.');
                        $('#loader-container').hide();
                        return false;
                    }
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //Charges Code End


        //$scope.CheckDecimal = function (value) {
        //    if (Number.isInteger(value)) {
        //        alert("Latitude/Longitude must be a decimal value.");
        //        return false;
        //    }
        //};


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

        $scope.editComment = function (event, zonedata) {
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
        };

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

    angular.module('app').directive('allowDecimalNumbers', function () {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                elm.on('keydown', function (event) {
                    var $input = $(this);
                    var value = $input.val();
                    value = value.replace(/[^0-9\.]/g, '')
                    var findsDot = new RegExp(/\./g)
                    var containsDot = value.match(findsDot)
                    if (containsDot != null && ([46, 110, 190].indexOf(event.which) > -1)) {
                        event.preventDefault();
                        return false;
                    }
                    $input.val(value);
                    if (event.which == 64 || event.which == 16) {
                        // numbers
                        return false;
                    } if ([8, 9, 13, 27, 37, 38, 39, 40, 110, 17, 67, 86, 65].indexOf(event.which) > -1) {
                        // backspace, enter, escape, arrows
                        return true;
                    } else if (event.which >= 48 && event.which <= 57) {
                        // numbers
                        return true;
                    } else if (event.which >= 96 && event.which <= 105) {
                        // numpad number
                        return true;
                    } else if ([46, 110, 190].indexOf(event.which) > -1) {
                        // dot and numpad dot
                        return true;
                    } else {
                        event.preventDefault();
                        return false;
                    }
                });
            }
        }
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

                    if (!angular.isUndefined(decimalCheck[0])) {
                        decimalCheck[0] = decimalCheck[0].slice(0, 4);
                        clean = decimalCheck[0];
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
