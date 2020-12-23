(function () {
    'use strict';

    angular.module('app')
        .controller('RevenuereportsCtrl', ['$scope', '$state', '$stateParams', RevenuereportsCtrl]);

    function RevenuereportsCtrl($scope, $state, $stateParams) {
        ModelDefinations();
        function ModelDefinations() {

            $scope.datevalue = new Date().toDateString();

            $scope.ActiveLocationListModel = [];
            $scope.ActiveLotListModel = [];
            $scope.ActiveChannelsListModel = [];
            $scope.ActiveVehicleTypesListModel = [];

            $scope.ReportFilterModel = {
                'Company': 'HMRL',
                'LocationID': 'All',
                'LocationParkingLotID': 'All',
                'ApplicationTypeID': 'All',
                'VehicleTypeID': 'All',
                'FromDate': $scope.datevalue,
                'ToDate': $scope.datevalue,
                'Duration': 'Today',
                'SupervisorID': 'All',
                'Display': ''
                , 'Reason': 'All'
                , 'FromTime': '01'
                , 'FromMeridiem': 'AM'
                , 'ToTime': '11'
                , 'ToMeridiem': 'PM'
            };
            $scope.ReportByStationListModel = [];
            $scope.GrandTotal = 0;
            $scope.GrandTotalforVehicle = 0;
            $scope.ReportByVehicleListModel = [];

            $scope.SelectedStation = '';
            $scope.SelectedLot = '';
            $scope.SelectedVehicle = '';

            $scope.SelectedItemModel = {
                'SelectedStation': 'All Stations',
                'SelectedLot': 'All Lots',
                'SelectedVehicle': 'All Vehicles',
                'FromDate': '',
                'ToDate': '',
                'Duration': 'Today',
                'SupervisorID': 'All Supervisors',
                'SelectedChannel': ''
            };

            $scope.ActiveVehicleTypesListModelWithAll = [];
            $scope.isPrinting = false;
            $scope.TodayDate = new Date();
            $scope.ReportByPaymentTypeListModel = [];
            $scope.GrandTotalforPaymentType = 0;

            $scope.disabled = false;
            $scope.required = true;
            $scope.printFromDate = '';
            $scope.printToDate = '';
            $scope.DurationListModel = [
                { 'name': 'Select', 'value': '0' },
                { 'name': 'Today', 'value': 'Today' },
                { 'name': 'Yesterday', 'value': 'Yesterday' },
                { 'name': 'Day Before Yesterday', 'value': 'Day Before Yesterday' },
                { 'name': 'Week', 'value': 'Week' },
                { 'name': 'Last 15days', 'value': 'Last 15days' },
                { 'name': 'Current Month', 'value': 'Current Month' },
                { 'name': 'Previous Month', 'value': 'Previous Month' },
                { 'name': 'Quarterly', 'value': 'Quarterly' }];

            $scope.ReportByPassesListModel = [];
            $scope.GrandTotalforPasses = 0;
            $scope.ChannelListforPasses = [];
            $scope.ChannelListforPaymentType = [];

            $scope.GrandTotalforChannel = 0;
            $scope.ReportByChannelListModel = [];

            $scope.GrandTotalforViolation = 0;
            $scope.ReportByViolationListModel = [];
            $scope.ActiveReasonListModel = [];

            $scope.SupervisorListModel = [];
            $scope.SupervisorLocationListModel = [];
            $scope.SupervisorLotListModel = [];
            $scope.GrandTotalforSupervisor = 0;
            $scope.ReportBySupervisorListModel = [];

            // $scope.Lotdisabled = true;

            $scope.GrandTotalforTime = 0;
            $scope.ReportByTimeListModel = [];

            $scope.TomaxDate = initMaxDate();

            $scope.Stationdisabled = true;
            $scope.StationdisabledInPaymentType = true;

            //$("#station_PaymentType").prop('disabled', 'disabled');
            //$("#Lot_PaymentType").prop('disabled', 'disabled');

            $scope.ReportTimeFilterModel = [
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
            $scope.ReportTimeMeridiemModel = [
                { 'meridiem': 'AM' },
                { 'meridiem': 'PM' }
            ];
            $scope.timepickerdisabled = false;

            $scope.todayDateVal = new Date().toDateString();
            $scope.filterFromTime = '';

            $scope.printFromDate = new Date($scope.datevalue);
            $scope.printToDate = new Date($scope.datevalue);
        }

        function initMaxDate() {
            return new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate());
        }

        GetActiveReasonsList();
        GetActiveLocationsList();
        GetActiveVehicleTypesList();
        GetActiveVehicleTypesListWithoutAll();
        GetActiveChannelsList();
        GetChannelsListForPasses();
        GetChannelsListForPaymentType();
        function GetActiveLocationsList() {
            var url = $("#GetActiveLocationsList").val();
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
                                $scope.ActiveLocationListModel = data;
                                $scope.ActiveLocationListModel.splice(0, 0, { 'LocationID': 'All', 'LocationName': 'All' });
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
        function GetActiveLotssList(LocationID) {
            var url = $("#GetActiveLotssList").val();
            if (url != undefined && LocationID != 'All') {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'LocationID':" + LocationID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.ActiveLotListModel = data;
                                $scope.ActiveLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
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
        $scope.GetActiveLots = function (LocationID) {
            $scope.ActiveLotListModel = [];
            $scope.SelectedItemModel.SelectedLot = '';
            if (LocationID != 'All') {
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
                GetActiveLotssList(LocationID);
            }
            for (var i = 0; i < $scope.ActiveLocationListModel.length; i++) {
                if ($scope.ActiveLocationListModel[i].LocationID == LocationID) {
                    $scope.SelectedItemModel.SelectedStation = $scope.ActiveLocationListModel[i].LocationName;
                    if ($scope.SelectedItemModel.SelectedStation == 'All') {
                        $scope.SelectedItemModel.SelectedStation = 'All Stations';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        $scope.ActiveLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                        $scope.ReportFilterModel.LocationParkingLotID = 'All';
                    }
                    //$scope.SelectedStation = $scope.ActiveLocationListModel[i].LocationName;
                    //if ($scope.SelectedStation == 'All') {
                    //    $scope.SelectedStation = 'All Stations';
                    //    $scope.SelectedLot = 'All Lots';
                    //}
                }
            }
        };
        $scope.GetSelectedLot = function (lotID) {
            if (lotID != 'All') {
                if ($scope.ReportFilterModel.LocationID != 'All') {
                    for (var i = 0; i < $scope.ActiveLotListModel.length; i++) {
                        if ($scope.ActiveLotListModel[i].LocationParkingLotID == lotID) {
                            $scope.SelectedItemModel.SelectedLot = $scope.ActiveLotListModel[i].LocationParkingLotName;
                        }
                    }
                }
            }
            else {
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
            }
        };
        $scope.GetSelectedVehicle = function (vehicletypeid) {
            for (var i = 0; i < $scope.ActiveVehicleTypesListModel.length; i++) {
                if ($scope.ActiveVehicleTypesListModel[i].VehicleTypeID == vehicletypeid) {
                    $scope.SelectedItemModel.SelectedVehicle = $scope.ActiveVehicleTypesListModel[i].VehicleTypeName;
                }
            }
        };
        $scope.GetSelectedVehicleWithAll = function (vehicletypeid) {
            for (var i = 0; i < $scope.ActiveVehicleTypesListModelWithAll.length; i++) {
                if ($scope.ActiveVehicleTypesListModelWithAll[i].VehicleTypeID == vehicletypeid) {
                    $scope.SelectedItemModel.SelectedVehicle = $scope.ActiveVehicleTypesListModelWithAll[i].VehicleTypeName;
                    if ($scope.SelectedItemModel.SelectedVehicle == 'All') {
                        $scope.SelectedItemModel.SelectedVehicle = 'All Vehicles';
                    }
                }
            }
        };

        function GetActiveChannelsList() {
            var url = $("#GetActiveChannelsList").val();
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
                                $scope.ActiveChannelsListModel = data;
                                $scope.ActiveChannelsListModel.splice(0, 0, { 'ApplicationTypeID': 'All', 'ApplicationTypeName': 'All' });

                                //$scope.ActiveChannelsListModel.splice(4, 1);     

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
                                $scope.ActiveVehicleTypesListModelWithAll = data;
                                $scope.ActiveVehicleTypesListModelWithAll.splice(0, 0, { 'VehicleTypeID': 'All', 'VehicleTypeName': 'All' });
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
        function GetActiveVehicleTypesListWithoutAll() {
            var url = $("#GetActiveVehicleTypesListWithoutAll").val();
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
        function GetChannelsListForPasses() {
            var url = $("#GetChannelListforPasses").val();
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
                                $scope.ChannelListforPasses = data;
                                $scope.ChannelListforPasses.splice(0, 0, { 'ApplicationTypeID': 'All', 'ApplicationTypeName': 'All' });
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
        function GetChannelsListForPaymentType() {
            var url = $("#GetChannelListforPaymentType").val();
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
                                $scope.ChannelListforPaymentType = data;
                                $scope.ChannelListforPaymentType.splice(0, 0, { 'ApplicationTypeID': 'All', 'ApplicationTypeName': 'All' });
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

        $scope.ChangeToDate = function () {
            validToDate();
            $scope.filterFromTime = '';
            $scope.filterFromTime = $scope.ReportFilterModel.FromDate.toDateString();
            $scope.printFromDate = $scope.ReportFilterModel.FromDate;
            $scope.printToDate = $scope.ReportFilterModel.ToDate;
            var todayDate = new Date();
            if ($scope.ReportFilterModel.FromDate.toDateString() == todayDate.toDateString()) {
                //enable time pickers and also from duration dropdown today value
                $scope.timepickerdisabled = false;
                $scope.ReportFilterModel.FromTime = '01';
                $scope.ReportFilterModel.FromMeridiem = 'AM';
                $scope.ReportFilterModel.ToTime = '11';
                $scope.ReportFilterModel.ToMeridiem = 'PM';
            }
            else {
                $scope.timepickerdisabled = true;  
                $scope.ReportFilterModel.FromTime = '';
                $scope.ReportFilterModel.FromMeridiem = '';
                $scope.ReportFilterModel.ToTime = '';
                $scope.ReportFilterModel.ToMeridiem = '';
            }
        };
        function validToDate() {
            $scope.ReportFilterModel.ToDate = '';
            var fDate = $scope.ReportFilterModel.FromDate;
            var tDate = $scope.ReportFilterModel.ToDate;
            if (tDate != "" && tDate != null && tDate != undefined) {
                if (tDate < fDate) {
                    alert('select valid To Date');
                    $scope.ReportFilterModel.ToDate = "";
                }
            }
        }
        $scope.AssignDates = function () {
            $scope.ReportFilterModel.FromDate = '';
            $scope.ReportFilterModel.ToDate = '';

            $scope.printFromDate = '';
            $scope.printToDate = '';

            $scope.filterFromTime = '';

            if ($scope.ReportFilterModel.Duration == 'Today') {

                $scope.disabled = true;
                $scope.required = false;

                var d = new Date($scope.TodayDate);
                d.setDate(d.getDate());
                $scope.todaydatestring = d.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.todaydatestring;
                $scope.ReportFilterModel.ToDate = $scope.todaydatestring;

                $scope.printFromDate = new Date($scope.todaydatestring);
                $scope.printToDate = new Date($scope.todaydatestring);

                //$scope.filterFromTime = $scope.ReportFilterModel.FromDate.toDateString();
                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;

                $scope.ReportFilterModel.FromTime = '01';
                $scope.ReportFilterModel.FromMeridiem = 'AM';
                $scope.ReportFilterModel.ToTime = '11';
                $scope.ReportFilterModel.ToMeridiem = 'PM';
                $scope.timepickerdisabled = false;
            }
            else if ($scope.ReportFilterModel.Duration == 'Yesterday') {

                $scope.disabled = true;
                $scope.required = false;

                var d = new Date($scope.TodayDate);
                d.setDate(d.getDate() - 1);
                $scope.Yesterday = d.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.Yesterday;
                $scope.ReportFilterModel.ToDate = $scope.Yesterday;

                $scope.printFromDate = new Date($scope.Yesterday);
                $scope.printToDate = new Date($scope.Yesterday);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
            }
            else if ($scope.ReportFilterModel.Duration == 'Day Before Yesterday') {

                $scope.disabled = true;
                $scope.required = false;

                var d = new Date($scope.TodayDate);
                d.setDate(d.getDate() - 2);
                $scope.DayBeforeYesterday = d.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.DayBeforeYesterday;
                $scope.ReportFilterModel.ToDate = $scope.DayBeforeYesterday;

                $scope.printFromDate = new Date($scope.DayBeforeYesterday);
                $scope.printToDate = new Date($scope.DayBeforeYesterday);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
            }
            else if ($scope.ReportFilterModel.Duration == 'Week') {
                $scope.disabled = true;
                $scope.required = false;

                var weekEdate = new Date($scope.TodayDate);
                weekEdate.setDate(weekEdate.getDate() - 1);
                $scope.Yesterday = weekEdate.toDateString();

                var weekSdate = new Date($scope.TodayDate);
                weekSdate.setDate(weekSdate.getDate() - 7);
                $scope.WeekStartDate = weekSdate.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.WeekStartDate;
                $scope.ReportFilterModel.ToDate = $scope.Yesterday;

                $scope.printFromDate = new Date($scope.WeekStartDate);
                $scope.printToDate = new Date($scope.Yesterday);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
                //$scope.timepickerdisabled = true;
                //$scope.ReportFilterModel.FromTime = '';
                //$scope.ReportFilterModel.FromMeridiem = '';
                //$scope.ReportFilterModel.ToTime = '';
                //$scope.ReportFilterModel.ToMeridiem = '';
            }
            else if ($scope.ReportFilterModel.Duration == 'Last 15days') {
                $scope.disabled = true;
                $scope.required = false;

                var days15Edate = new Date($scope.TodayDate);
                days15Edate.setDate(days15Edate.getDate() - 1);
                $scope.Yesterday = days15Edate.toDateString();

                var days15Sdate = new Date($scope.TodayDate);
                days15Sdate.setDate(days15Sdate.getDate() - 15);
                $scope.days15StartDate = days15Sdate.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.days15StartDate;
                $scope.ReportFilterModel.ToDate = $scope.Yesterday;

                $scope.printFromDate = new Date($scope.days15StartDate);
                $scope.printToDate = new Date($scope.Yesterday);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
                //$scope.timepickerdisabled = true;
                //$scope.ReportFilterModel.FromTime = '';
                //$scope.ReportFilterModel.FromMeridiem = '';
                //$scope.ReportFilterModel.ToTime = '';
                //$scope.ReportFilterModel.ToMeridiem = '';
            }
            else if ($scope.ReportFilterModel.Duration == 'Current Month') {

                $scope.disabled = true;
                $scope.required = false;

                var date = new Date();
                $scope.firstDay = new Date(date.getFullYear(), date.getMonth(), 1);

                var d1 = new Date($scope.TodayDate);
                d1.setDate(d1.getDate() - 1);
                $scope.Yesterday = d1.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.firstDay.toDateString();
                $scope.ReportFilterModel.ToDate = $scope.Yesterday;

                $scope.printFromDate = new Date($scope.firstDay);
                $scope.printToDate = new Date($scope.Yesterday);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
                //$scope.timepickerdisabled = true;
                //$scope.ReportFilterModel.FromTime = '';
                //$scope.ReportFilterModel.FromMeridiem = '';
                //$scope.ReportFilterModel.ToTime = '';
                //$scope.ReportFilterModel.ToMeridiem = '';
            }
            else if ($scope.ReportFilterModel.Duration == 'Previous Month') {

                $scope.disabled = true;
                $scope.required = false;

                var date1 = new Date();
                $scope.PrevfirstDay = new Date(date1.getFullYear(), date1.getMonth() - 1, 1);
                $scope.PrevlastDay = new Date(date1.getFullYear(), date1.getMonth(), 0);

                $scope.ReportFilterModel.FromDate = $scope.PrevfirstDay.toDateString();
                $scope.ReportFilterModel.ToDate = $scope.PrevlastDay.toDateString();

                $scope.printFromDate = new Date($scope.PrevfirstDay);
                $scope.printToDate = new Date($scope.PrevlastDay);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
                //$scope.timepickerdisabled = true;
                //$scope.ReportFilterModel.FromTime = '';
                //$scope.ReportFilterModel.FromMeridiem = '';
                //$scope.ReportFilterModel.ToTime = '';
                //$scope.ReportFilterModel.ToMeridiem = '';
            }
            else if ($scope.ReportFilterModel.Duration == 'Quarterly') {

                $scope.disabled = true;
                $scope.required = false;

                var QuarterlyEdate = new Date($scope.TodayDate);
                // $scope.QuarterlyfirstDay = new Date(QuarterlyEdate.getFullYear(), QuarterlyEdate.getMonth() - 3, 1);

                QuarterlyEdate.setDate(QuarterlyEdate.getDate() - 90);
                $scope.QuarterlyEdate = QuarterlyEdate.toDateString();

                // $scope.QuarterlylastDay = new Date(QuarterlyEdate.getFullYear(), QuarterlyEdate.getMonth(), 0);
                var QDate = new Date($scope.TodayDate);
                QDate.setDate(QDate.getDate() - 1);
                $scope.Yesterday = QDate.toDateString();

                $scope.ReportFilterModel.FromDate = $scope.QuarterlyEdate;
                $scope.ReportFilterModel.ToDate = $scope.Yesterday;

                $scope.printFromDate = new Date($scope.QuarterlyEdate);
                $scope.printToDate = new Date($scope.Yesterday);

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
                //$scope.timepickerdisabled = true;
                //$scope.ReportFilterModel.FromTime = '';
                //$scope.ReportFilterModel.FromMeridiem = '';
                //$scope.ReportFilterModel.ToTime = '';
                //$scope.ReportFilterModel.ToMeridiem = '';
            }
            else {
                $scope.disabled = false;
                $scope.required = true;

                $scope.ReportFilterModel.FromDate = $scope.ReportFilterModel.FromDate;
                $scope.ReportFilterModel.ToDate = $scope.ReportFilterModel.ToDate;

                $scope.printFromDate = $scope.ReportFilterModel.FromDate;
                $scope.printToDate = $scope.ReportFilterModel.ToDate;

                $scope.filterFromTime = $scope.ReportFilterModel.FromDate;
            }


            var TDate = new Date();
            if ($scope.ReportFilterModel.FromDate == TDate.toDateString()) {
                $scope.timepickerdisabled = false;
            }
            else {
                $scope.timepickerdisabled = true;
                $scope.ReportFilterModel.FromTime = '';
                $scope.ReportFilterModel.FromMeridiem = '';
                $scope.ReportFilterModel.ToTime = '';
                $scope.ReportFilterModel.ToMeridiem = '';
            }

        };
        $scope.AssignToDate = function () {
            $scope.printFromDate = $scope.ReportFilterModel.FromDate;
            $scope.printToDate = $scope.ReportFilterModel.ToDate;
        };


        //By Station Start
        $scope.GetReportByStation = function () {
            var url = $("#GetReportByStation").val();
            $scope.GrandTotal = 0;
            $scope.ReportByStationListModel = [];

            var dateflag = false;
            if ($scope.ReportFilterModel.FromTime != "") {
                var todayDate = new Date().toLocaleString().split(',')[0];
                var ftime = $scope.ReportFilterModel.FromTime + ":00 " +
                    $scope.ReportFilterModel.FromMeridiem;
                var FromTime = new Date(todayDate + ' ' + ftime);
                var tTime = $scope.ReportFilterModel.ToTime + ":00 " +
                    $scope.ReportFilterModel.ToMeridiem;
                var ToTime = new Date(todayDate + ' ' + tTime);

                if (new Date(FromTime) >= new Date(ToTime)) {
                    dateflag = false;
                }
                else {
                    dateflag = true;
                }
            }
            else {
                dateflag = true;
            }

            if (dateflag) {
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'stationFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed") {
                                    $scope.ReportByStationListModel = data;

                                    for (var i = 0; i < $scope.ReportByStationListModel.length; i++) {
                                        $scope.ReportByStationListModel[i].Amount = parseFloat($scope.ReportByStationListModel[i].Amount).toFixed(2);
                                        $scope.GrandTotal += parseFloat($scope.ReportByStationListModel[i].Amount);
                                    }
                                    $scope.GrandTotal = parseFloat($scope.GrandTotal).toFixed(2);
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
                alert('To Time must be greater than From Time.');
            }
        };
        $scope.DownloadReportByStationPDF = function () {
            if ($scope.ReportByStationListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/StationReportPDFDownload?GrandTotal=' + $scope.GrandTotal + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByStationExcel = function () {
            if ($scope.ReportByStationListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/StationReportExcelDownload?GrandTotal=' + $scope.GrandTotal + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByStationPrint1 = function (divName) {
            if ($scope.ReportByStationListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Station End

        //By Vehicle Start
        $scope.GetReportByVehicle = function () {
            var url = $("#GetReportByVehicle").val();
            $scope.GrandTotalforVehicle = 0;

            $scope.ReportByVehicleListModel = [];

            var dateflag = false;
            if ($scope.ReportFilterModel.FromTime != "") {
                var todayDate = new Date().toLocaleString().split(',')[0];
                var ftime = $scope.ReportFilterModel.FromTime + ":00 " +
                    $scope.ReportFilterModel.FromMeridiem;
                var FromTime = new Date(todayDate + ' ' + ftime);
                var tTime = $scope.ReportFilterModel.ToTime + ":00 " +
                    $scope.ReportFilterModel.ToMeridiem;
                var ToTime = new Date(todayDate + ' ' + tTime);

                if (new Date(FromTime) >= new Date(ToTime)) {
                    dateflag = false;
                }
                else {
                    dateflag = true;
                }
            }
            else {
                dateflag = true;
            }

            if (dateflag) {
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'vehicleFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed") {
                                    $scope.ReportByVehicleListModel = data;

                                    for (var i = 0; i < $scope.ReportByVehicleListModel.length; i++) {
                                        $scope.ReportByVehicleListModel[i].Amount = parseFloat($scope.ReportByVehicleListModel[i].Amount).toFixed(2);
                                        $scope.GrandTotalforVehicle += parseFloat($scope.ReportByVehicleListModel[i].Amount);
                                    }
                                    $scope.GrandTotalforVehicle = parseFloat($scope.GrandTotalforVehicle).toFixed(2);
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
                alert('To Time must be greater than From Time.');
            }
        };
        $scope.DownloadReportByVehiclePDF = function () {
            if ($scope.ReportByVehicleListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/VehicleReportPDFDownload?GrandTotal=' + $scope.GrandTotalforVehicle + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                    //window.location = 'RevenueReports/VehicleReportPDFDownload?vehicle=' + JSON.stringify($scope.ReportByVehicleListModel) + '&GrandTotal=' + JSON.stringify($scope.GrandTotalforVehicle) + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByVehicleExcel = function () {
            if ($scope.ReportByVehicleListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/VehicleReportExcelDownload?GrandTotal=' + $scope.GrandTotalforVehicle + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                    //window.location = 'RevenueReports/VehicleReportExcelDownload?vehicle=' + JSON.stringify($scope.ReportByVehicleListModel) + '&GrandTotal=' + JSON.stringify($scope.GrandTotalforVehicle) + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByVehiclePrint = function (divName) {
            if ($scope.ReportByVehicleListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Vehicle End

        //By Payment Type Start
        $scope.GetReportByPaymentType = function () {
            var url = $("#GetReportByPaymentType").val();
            $scope.GrandTotalforPaymentType = 0;

            $scope.ReportByVehicleListModel = [];

            var dateflag = false;
            if ($scope.ReportFilterModel.FromTime != "") {
                var todayDate = new Date().toLocaleString().split(',')[0];
                var ftime = $scope.ReportFilterModel.FromTime + ":00 " +
                    $scope.ReportFilterModel.FromMeridiem;
                var FromTime = new Date(todayDate + ' ' + ftime);
                var tTime = $scope.ReportFilterModel.ToTime + ":00 " +
                    $scope.ReportFilterModel.ToMeridiem;
                var ToTime = new Date(todayDate + ' ' + tTime);

                if (new Date(FromTime) >= new Date(ToTime)) {
                    dateflag = false;
                }
                else {
                    dateflag = true;
                }
            }
            else {
                dateflag = true;
            }

            if (dateflag) {
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'paymentFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed") {
                                    $scope.ReportByPaymentTypeListModel = data;

                                    for (var i = 0; i < $scope.ReportByPaymentTypeListModel.length; i++) {
                                        $scope.ReportByPaymentTypeListModel[i].Amount = parseFloat($scope.ReportByPaymentTypeListModel[i].Amount).toFixed(2);
                                        $scope.GrandTotalforPaymentType += parseFloat($scope.ReportByPaymentTypeListModel[i].Amount);
                                    }
                                    $scope.GrandTotalforPaymentType = parseFloat($scope.GrandTotalforPaymentType).toFixed(2);
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
                alert('To Time must be greater than From Time.');
            }
        };
        $scope.DownloadReportByPaymentPDF = function () {
            if ($scope.ReportByPaymentTypeListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/PaymentTypeReportPDFDownload?GrandTotal=' + $scope.GrandTotalforPaymentType + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByPaymentExcel = function () {
            if ($scope.ReportByPaymentTypeListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/PaymentTypeReportExcelDownload?GrandTotal=' + $scope.GrandTotalforPaymentType + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByPaymentTypePrint = function (divName) {
            if ($scope.ReportByPaymentTypeListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.GetChannelNameforPaymentType = function (channel) {
            $scope.SelectedItemModel.SelectedStation = '';

            for (var i = 0; i < $scope.ChannelListforPaymentType.length; i++) {
                if ($scope.ChannelListforPaymentType[i].ApplicationTypeID == channel) {
                    $scope.SelectedItemModel.SelectedChannel = $scope.ChannelListforPaymentType[i].ApplicationTypeName;
                    if ($scope.SelectedItemModel.SelectedChannel == 'All') {
                        $scope.SelectedItemModel.SelectedChannel = 'All Channels';
                        $scope.ActiveLocationListModel = [];
                    }

                    if ($scope.SelectedItemModel.SelectedChannel == 'OPERATOR PAY') {
                        //$("#station_PaymentType").prop("disabled", false);
                        //$("#Lot_PaymentType").prop("disabled", false);
                        $scope.StationdisabledInPaymentType = false;
                        $scope.SelectedItemModel.SelectedStation = 'All Stations';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        GetActiveLocationsList();
                    }
                    else {
                        $scope.StationdisabledInPaymentType = true;
                        //$("#station_PaymentType").prop('disabled', 'disabled');
                        //$("#Lot_PaymentType").prop('disabled', 'disabled');
                        $scope.ActiveLocationListModel = [];
                        $scope.ActiveLotListModel = [];
                        $scope.ReportFilterModel.LocationID = 'All';
                        $scope.ReportFilterModel.LocationParkingLotID = 'All';
                    }
                }
            }
        };
        //By Payment Type End

        //By Time Start
        $scope.GetReportByTime = function () {
            var url = $("#GetReportByTime").val();
            $scope.GrandTotalforTime = 0;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'TimeFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.ReportByTimeListModel = data;

                                for (var i = 0; i < $scope.ReportByTimeListModel.length; i++) {
                                    $scope.ReportByTimeListModel[i].Cash = parseFloat($scope.ReportByTimeListModel[i].Cash).toFixed(2);
                                    $scope.ReportByTimeListModel[i].EPay = parseFloat($scope.ReportByTimeListModel[i].EPay).toFixed(2);
                                    $scope.ReportByTimeListModel[i].Amount = parseFloat($scope.ReportByTimeListModel[i].Amount).toFixed(2);
                                    $scope.GrandTotalforTime += parseFloat($scope.ReportByTimeListModel[i].Amount);
                                }
                                $scope.GrandTotalforTime = parseFloat($scope.GrandTotalforTime).toFixed(2);
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
        };
        $scope.DownloadReportByTimePDF = function () {
            if ($scope.ReportByTimeListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/TimeReportPDFDownload?GrandTotal=' + $scope.GrandTotalforTime + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByTimeExcel = function () {
            if ($scope.ReportByTimeListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/TimeReportExcelDownload?GrandTotal=' + $scope.GrandTotalforTime + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByTimePrint = function (divName) {
            if ($scope.ReportByTimeListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Time End

        //By Passes Start
        $scope.GetLocationNameInPasses = function (location) {
            for (var i = 0; i < $scope.ActiveLocationListModel.length; i++) {
                if ($scope.ActiveLocationListModel[i].LocationID == location) {
                    $scope.SelectedItemModel.SelectedStation = $scope.ActiveLocationListModel[i].LocationName;
                    if ($scope.SelectedItemModel.SelectedStation == 'All') {
                        $scope.SelectedItemModel.SelectedStation = 'All Stations';
                    }
                }
            }
            //$scope.GetActiveLots(location);
        };
        $scope.GetChannelName = function (channel) {
            $scope.SelectedItemModel.SelectedStation = '';

            for (var i = 0; i < $scope.ChannelListforPasses.length; i++) {
                if ($scope.ChannelListforPasses[i].ApplicationTypeID == channel) {
                    $scope.SelectedItemModel.SelectedChannel = $scope.ChannelListforPasses[i].ApplicationTypeName;
                    if ($scope.SelectedItemModel.SelectedChannel == 'All') {
                        $scope.SelectedItemModel.SelectedChannel = 'All Channels';
                        $scope.ActiveLocationListModel = [];
                    }

                    if ($scope.SelectedItemModel.SelectedChannel == 'OPERATOR PAY') {
                        $scope.Stationdisabled = false;
                        $scope.SelectedItemModel.SelectedStation = 'All Stations';
                        GetActiveLocationsList();
                    }
                    else {
                        $scope.Stationdisabled = true;
                        $scope.ActiveLocationListModel = [];
                        $scope.ReportFilterModel.LocationID = 'All';
                    }
                }
            }
        };
        $scope.GetReportByPasses = function () {
            var url = $("#GetReportByPasses").val();
            $scope.GrandTotalforPasses = 0;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'passFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.ReportByPassesListModel = data;
                                if ($scope.ReportByPassesListModel.length > 0) {
                                    for (var i = 0; i < $scope.ReportByPassesListModel.length; i++) {
                                        $scope.ReportByPassesListModel[i].Amount = parseFloat($scope.ReportByPassesListModel[i].Amount).toFixed(2);
                                        $scope.GrandTotalforPasses += parseFloat($scope.ReportByPassesListModel[i].Amount);
                                    }
                                }
                                $scope.GrandTotalforPasses = parseFloat($scope.GrandTotalforPasses).toFixed(2);
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
        };
        $scope.DownloadReportByPassPDF = function () {
            if ($scope.ReportByPassesListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/PassesReportPDFDownload?GrandTotal=' + $scope.GrandTotalforPasses + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByPassExcel = function () {
            if ($scope.ReportByPassesListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/PassesReportExcelDownload?GrandTotal=' + $scope.GrandTotalforPasses + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel) + '&Channel=' + $scope.SelectedItemModel.SelectedChannel;
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByPassPrint = function (divName) {
            if ($scope.ReportByPassesListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Passes End

        //By Channel Start
        $scope.GetReportByChannel = function () {
            var url = $("#GetReportByChannel").val();
            $scope.GrandTotalforChannel = 0;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'channelFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.ReportByChannelListModel = data;
                                if ($scope.ReportByChannelListModel.length > 0) {
                                    for (var i = 0; i < $scope.ReportByChannelListModel.length; i++) {
                                        $scope.ReportByChannelListModel[i].Amount = parseFloat($scope.ReportByChannelListModel[i].Amount).toFixed(2);
                                        $scope.GrandTotalforChannel += parseFloat($scope.ReportByChannelListModel[i].Amount);
                                    }
                                }
                                $scope.GrandTotalforChannel = parseFloat($scope.GrandTotalforChannel).toFixed(2);
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
        };
        $scope.DownloadReportByChannelPDF = function () {
            if ($scope.ReportByChannelListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/ChannelReportPDFDownload?GrandTotal=' + $scope.GrandTotalforChannel + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByChannelExcel = function () {
            if ($scope.ReportByChannelListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/ChannelReportExcelDownload?GrandTotal=' + $scope.GrandTotalforChannel + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByChannelPrint = function (divName) {
            if ($scope.ReportByChannelListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Channel End

        //By Supervisor Start
        GetActiveSupervisorsList();
        function GetActiveSupervisorsList() {
            var url = $("#GetActiveSupervisorList").val();
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
                                $scope.SupervisorListModel = data;
                                $scope.SupervisorListModel.splice(0, 0, { 'UserID': 'All', 'UserName': 'All' });
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
        $scope.SupervisorChange = function (SupervisorID) {
            $scope.SupervisorLocationListModel = [];
            $scope.SupervisorLotListModel = [];
            $scope.SelectedItemModel.SelectedStation = '';
            $scope.SelectedItemModel.SelectedLot = '';
            if (SupervisorID != 'All') {
                $scope.SelectedItemModel.SelectedStation = 'All Stations';
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
                GetSupervisorLocationsList(SupervisorID);
            }
            for (var i = 0; i < $scope.SupervisorListModel.length; i++) {
                if ($scope.SupervisorListModel[i].UserID == SupervisorID) {
                    $scope.SelectedItemModel.SupervisorID = $scope.SupervisorListModel[i].UserName;
                    if ($scope.SelectedItemModel.SupervisorID == 'All') {
                        $scope.SelectedItemModel.SupervisorID = 'All Supervisors';
                        $scope.SelectedItemModel.SelectedStation = 'All Stations';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        $scope.SupervisorLocationListModel.splice(0, 0, { 'LocationID': 'All', 'LocationName': 'All' });
                        $scope.SupervisorLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                        $scope.ReportFilterModel.LocationID = 'All';
                        $scope.ReportFilterModel.LocationParkingLotID = 'All';
                    }
                }
            }
        };
        function GetSupervisorLocationsList(SupervisorID) {
            var url = $("#GetSupervisorLocationList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'SupervisorID':" + SupervisorID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.SupervisorLocationListModel = data;
                                $scope.SupervisorLocationListModel.splice(0, 0, { 'LocationID': 'All', 'LocationName': 'All' });
                                $scope.SupervisorLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                                $scope.$apply();
                            }
                            else {
                                $scope.SupervisorLocationListModel.splice(0, 0, { 'LocationID': 'All', 'LocationName': 'All' });
                                $scope.SupervisorLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
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
        $scope.GetSupervisorLots = function (locationID) {
            $scope.SupervisorLotListModel = [];
            $scope.SelectedItemModel.SelectedLot = '';
            if (locationID != 'All') {
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
                GetSupervisorLotssList(locationID);
            }
            for (var i = 0; i < $scope.SupervisorLocationListModel.length; i++) {
                if ($scope.SupervisorLocationListModel[i].LocationID == locationID) {
                    $scope.SelectedItemModel.SelectedStation = $scope.SupervisorLocationListModel[i].LocationName;
                    if ($scope.SelectedItemModel.SelectedStation == 'All') {
                        $scope.SelectedItemModel.SelectedStation = 'All Stations';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        $scope.SupervisorLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                    }
                }
            }
        };
        function GetSupervisorLotssList(locationID) {
            var url = $("#GetSupervisorLotsList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'locationID':" + locationID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.SupervisorLotListModel = data;
                                $scope.SupervisorLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                                $scope.$apply();
                            }
                            else {
                                $scope.SupervisorLotListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
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
        $scope.GetSelectedSupervisorLot = function (lotID) {
            if (lotID != 'All') {
                if ($scope.ReportFilterModel.LocationID != 'All') {
                    for (var i = 0; i < $scope.SupervisorLotListModel.length; i++) {
                        if ($scope.SupervisorLotListModel[i].LocationParkingLotID == lotID) {
                            $scope.SelectedItemModel.SelectedLot = $scope.SupervisorLotListModel[i].LocationParkingLotName;
                        }
                    }
                }
            }
            else {
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
            }
        };
        $scope.GetReportBySupervisor = function () {
            var url = $("#GetReportBySupervisor").val();
            $scope.GrandTotalforSupervisor = 0;
            $scope.ReportBySupervisorListModel = [];

            var dateflag = false;
            if ($scope.ReportFilterModel.FromTime != "") {
                var todayDate = new Date().toLocaleString().split(',')[0];
                var ftime = $scope.ReportFilterModel.FromTime + ":00 " +
                    $scope.ReportFilterModel.FromMeridiem;
                var FromTime = new Date(todayDate + ' ' + ftime);
                var tTime = $scope.ReportFilterModel.ToTime + ":00 " +
                    $scope.ReportFilterModel.ToMeridiem;
                var ToTime = new Date(todayDate + ' ' + tTime);

                if (new Date(FromTime) >= new Date(ToTime)) {
                    dateflag = false;
                }
                else {
                    dateflag = true;
                }
            }
            else {
                dateflag = true;
            }
            if (dateflag) {
                if (url != undefined) {
                    $('#loader-container').show();
                    if (CheckInSession()) {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: "{'supervisorFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                if (data != "Failed") {
                                    $scope.ReportBySupervisorListModel = data;
                                    if ($scope.ReportBySupervisorListModel.length > 0) {
                                        for (var i = 0; i < $scope.ReportBySupervisorListModel.length; i++) {

                                            $scope.ReportBySupervisorListModel[i].CheckInsCash = parseFloat($scope.ReportBySupervisorListModel[i].CheckInsCash).toFixed(2);
                                            $scope.ReportBySupervisorListModel[i].CheckInsEPay = parseFloat($scope.ReportBySupervisorListModel[i].CheckInsEPay).toFixed(2);
                                            $scope.ReportBySupervisorListModel[i].PassesCash = parseFloat($scope.ReportBySupervisorListModel[i].PassesCash).toFixed(2);
                                            $scope.ReportBySupervisorListModel[i].PassesEPay = parseFloat($scope.ReportBySupervisorListModel[i].PassesEPay).toFixed(2);

                                            $scope.ReportBySupervisorListModel[i].Amount = parseFloat($scope.ReportBySupervisorListModel[i].Amount).toFixed(2);
                                            $scope.GrandTotalforSupervisor += parseFloat($scope.ReportBySupervisorListModel[i].Amount);
                                        }
                                    }
                                    $scope.GrandTotalforSupervisor = parseFloat($scope.GrandTotalforSupervisor).toFixed(2);
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
                alert('To Time must be greater than From Time.');
            }
        };
        $scope.DownloadReportBySupervisorPDF = function () {
            if ($scope.ReportBySupervisorListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/SupervisorReportPDFDownload?GrandTotal=' + $scope.GrandTotalforSupervisor + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportBySupervisorExcel = function () {
            if ($scope.ReportBySupervisorListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/SupervisorReportExcelDownload?GrandTotal=' + $scope.GrandTotalforSupervisor + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportBySupervisorPrint = function (divName) {
            if ($scope.ReportBySupervisorListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Supervisor End

        //By Violation Start
        function GetActiveReasonsList() {
            var url = $("#GetActiveReasonsList").val();
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
                                $scope.ActiveReasonListModel = data;
                                $scope.ActiveReasonListModel.splice(0, 0, { 'ViolationReasonID': 'All', 'Reason': 'All' });
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
        $scope.GetReportByViolation = function () {
            var url = $("#GetReportByViolation").val();
            $scope.GrandTotalforViolation = 0;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'violationFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.ReportByViolationListModel = data;
                                if ($scope.ReportByViolationListModel.length > 0) {
                                    for (var i = 0; i < $scope.ReportByViolationListModel.length; i++) {
                                        $scope.ReportByViolationListModel[i].Amount = parseFloat($scope.ReportByViolationListModel[i].Amount).toFixed(2);
                                        $scope.GrandTotalforViolation += parseFloat($scope.ReportByViolationListModel[i].Amount);
                                    }
                                }
                                $scope.GrandTotalforViolation = parseFloat($scope.GrandTotalforViolation).toFixed(2);
                                $scope.$apply();
                            }
                            else {
                                $scope.ReportByViolationListModel = [];
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
        $scope.DownloadReportByViolationPDF = function () {
            if ($scope.ReportByViolationListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/ViolationReportPDFDownload?GrandTotal=' + $scope.GrandTotalforViolation + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadReportByViolationExcel = function () {
            if ($scope.ReportByViolationListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;
                    window.location = 'RevenueReports/ViolationReportExcelDownload?GrandTotal=' + $scope.GrandTotalforViolation + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.ReportByViolationPrint = function (divName) {
            if ($scope.ReportByViolationListModel.length > 0) {
                if (CheckInSession()) {
                    var printContents = document.getElementById(divName).innerHTML;
                    var popupWin = window.open('', '_blank', 'width=500,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="style.css" /></head><body style="font-size:12px; line-height: 18px; color: #000000; margin: 0px; padding: 0px; font-family: pnregular; letter-spacing: 0.5px;" onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        //By Violation End

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

        $scope.toggleLimitOptions = function () {
            $scope.limitOptions = $scope.limitOptions ? undefined : [5, 10, 15];
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

    angular.module('app').config(function ($mdDateLocaleProvider) {
        $mdDateLocaleProvider.formatDate = function (date) {
            //return moment(date).format('DD-MM-YYYY');            
            return date ? moment(date).format('DD-MM-YYYY') : '';
        };
    });
})(); 
