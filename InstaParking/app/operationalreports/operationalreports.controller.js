(function () {
    'use strict';

    angular.module('app')
        .controller('OperationalreportsCtrl', ['$scope', '$state', '$stateParams', OperationalreportsCtrl]);

    function OperationalreportsCtrl($scope, $state, $stateParams) {
        ModelDefinations();
        function ModelDefinations() {
            $scope.ActiveLocationListModel = [];
            $scope.ActiveLotListModel = [];
            $scope.ActiveChannelsListModel = [];

            $scope.datevalue = new Date().toDateString();

            $scope.ReportFilterModel = {
                'Company': 'HMRL',
                'LocationID': 'All',
                'LocationParkingLotID': 'All',
                'ApplicationTypeID': 'All',
                'Duration': 'Today',
                'FromDate': $scope.datevalue,
                'ToDate': $scope.datevalue,
                'SupervisorID': 'All',
                'OperatorID': 'All'
                , 'VehicleTypeID': 'All'
                ,'FOCReasonID':'All'
            };
            $scope.CheckInReportListModel = [];
            $scope.CheckInGrandTotal = 0;

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
                'SupervisorName': 'All Supervisors',
                'OperatorName': 'All Operators'
            };
            $scope.isPrinting = false;
            $scope.TodayDate = new Date();

            $scope.disabled = true;
            $scope.required = true;

            $scope.SupervisorListModel = [];
            $scope.OperatorsListModel = [];
            $scope.OperatorsLotsListModel = [];
            $scope.OperatorReportListModel = [];

            $scope.printFromDate = '';
            $scope.printToDate = '';

            $scope.ActiveVehicleTypesListModelWithAll = [];
            $scope.OccupancyListModel = [];

            $scope.TomaxDate = initMaxDate();

            $scope.FOCReportListModel = [];

            $scope.printFromDate = new Date($scope.datevalue);
            $scope.printToDate = new Date($scope.datevalue);

            $scope.FOCReasonListModel = [];
            $scope.AllocationsListModel = [];

            $scope.days = 1;

            $scope.DuplicateEntriesListModel = [];
        }

        function initMaxDate() {
            return new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate());
        }

        GetActiveLocationsList();
        GetActiveChannelsList();
        GetActiveVehicleTypesList();
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
                    else {
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        $scope.ReportFilterModel.LocationParkingLotID = 'All';
                    }
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
            } else {
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
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

        $scope.ChangeToDate = function () {
            validToDate();
            $scope.printFromDate = $scope.ReportFilterModel.FromDate;
            $scope.printToDate = $scope.ReportFilterModel.ToDate;
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

                $scope.days = 1;
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

                $scope.days = 1;
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

                $scope.days = 1;
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

                $scope.days = Math.ceil(($scope.printToDate - $scope.printFromDate) / (1000 * 60 * 60 * 24)) + 1;
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

                $scope.days = Math.ceil(($scope.printToDate - $scope.printFromDate) / (1000 * 60 * 60 * 24)) + 1;
            }            
            else {
                $scope.disabled = false;
                $scope.required = true;

                $scope.ReportFilterModel.FromDate = $scope.ReportFilterModel.FromDate;
                $scope.ReportFilterModel.ToDate = $scope.ReportFilterModel.ToDate;

                $scope.printFromDate = $scope.ReportFilterModel.FromDate;
                $scope.printToDate = $scope.ReportFilterModel.ToDate;
            }          
        };
        $scope.AssignToDate = function () {
            $scope.printFromDate = $scope.ReportFilterModel.FromDate;
            $scope.printToDate = $scope.ReportFilterModel.ToDate;

            $scope.days = Math.ceil(($scope.ReportFilterModel.ToDate - $scope.ReportFilterModel.FromDate) / (1000 * 60 * 60 * 24))+1;
        };

        $scope.GetCheckInReport = function () {
           
            var url = $("#GetCheckInReport").val();
            $scope.CheckInGrandTotal = 0;

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'CheckInFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.CheckInReportListModel = data;

                                for (var i = 0; i < $scope.CheckInReportListModel.length; i++) {
                                    $scope.CheckInGrandTotal += parseFloat($scope.CheckInReportListModel[i].Total);
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
        };
        $scope.DownloadCheckInReportPDF = function () {
            if ($scope.CheckInReportListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;

                    window.location = 'OperationalReports/CheckInReportPDFDownload?GrandTotal=' + JSON.stringify($scope.CheckInGrandTotal) + '&SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadCheckInReportExcel = function () {
            if ($scope.CheckInReportListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;

                    window.location = 'OperationalReports/CheckInReportExcelDownload?GrandTotal=' + JSON.stringify($scope.CheckInGrandTotal) + '&SelectedItems=' + JSON.stringify($scope.ReportFilterModel);

                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.CheckInReportPrint = function (divName) {
            if ($scope.CheckInReportListModel.length > 0) {
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
        function GetOperatorsBySupervisorID(supervisorID) {
            var url = $("#GetActiveOperatorsList").val();
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'supervisorID':" + supervisorID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.OperatorsListModel = data;
                                $scope.OperatorsListModel.splice(0, 0, { 'UserID': 'All', 'UserName': 'All' });
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
        function GetActiveLotsofOperators(operatorID) {
            var url = $("#GetOperatorLotsList").val();
           


            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'operatorID':" + operatorID + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data.length > 0) {
                                $scope.OperatorsLotsListModel = data;
                                $scope.OperatorsLotsListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                                $scope.$apply();
                            }
                            else {
                                $scope.OperatorsLotsListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
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
        $scope.GetActiveOperators = function (supervisorID) {
            $scope.SelectedItemModel.OperatorName = '';
            $scope.OperatorsListModel = [];
            if (supervisorID != 'All') {
                GetOperatorsBySupervisorID(supervisorID);
            }

            for (var i = 0; i < $scope.SupervisorListModel.length; i++) {
                if ($scope.SupervisorListModel[i].UserID == supervisorID) {
                    $scope.SelectedItemModel.SupervisorName = $scope.SupervisorListModel[i].UserName;
                    if ($scope.SelectedItemModel.SupervisorName == 'All') {
                        $scope.SelectedItemModel.SupervisorName = 'All Supervisors';
                        $scope.SelectedItemModel.OperatorName = 'All Operators';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        $scope.OperatorsListModel.splice(0, 0, { 'UserID': 'All', 'UserName': 'All' });
                        $scope.OperatorsLotsListModel.splice(0, 0, { 'LocationParkingLotID': 'All', 'LocationParkingLotName': 'All' });
                        $scope.ReportFilterModel.LocationParkingLotID = 'All';
                        $scope.ReportFilterModel.OperatorID = 'All';

                    }
                    else {
                        $scope.SelectedItemModel.OperatorName = 'All Operators';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                        $scope.ReportFilterModel.LocationParkingLotID = 'All';
                        $scope.ReportFilterModel.OperatorID = 'All';
                    }
                }
            }
        };
        $scope.GetActiveOperatorLots = function (operatorID) {
            $scope.OperatorsLotsListModel = [];
            GetActiveLotsofOperators(operatorID);
            for (var i = 0; i < $scope.OperatorsListModel.length; i++) {
                if ($scope.OperatorsListModel[i].UserID == operatorID) {
                    $scope.SelectedItemModel.OperatorName = $scope.OperatorsListModel[i].UserName;
                    if ($scope.SelectedItemModel.OperatorName == 'All') {
                        $scope.SelectedItemModel.OperatorName = 'All Operators';
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                       
                    }
                    else {
                        $scope.SelectedItemModel.SelectedLot = 'All Lots';
                    }
                }
            }
        };
        $scope.GetOperatorLot = function (lotID) {
            if ($scope.ReportFilterModel.OperatorID != 'All') {
                for (var i = 0; i < $scope.OperatorsLotsListModel.length; i++) {
                    if ($scope.OperatorsLotsListModel[i].LocationParkingLotID == lotID) {
                        $scope.SelectedItemModel.SelectedLot = $scope.OperatorsLotsListModel[i].LocationParkingLotName;
                    }
                }
            }
            else {
                $scope.SelectedItemModel.SelectedLot = 'All Lots';
            }
        };

        $scope.GetReportByOperator = function () {
           
            var url = $("#GetReportByOperator").val();
           
            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'operatorFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.OperatorReportListModel = data;
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
        $scope.DownloadOperatorReportPDF = function () {
            if ($scope.OperatorReportListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;

                    window.location = 'OperationalReports/OperatorHoursReportPDFDownload?SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadOperatorReportExcel = function () {
            if ($scope.OperatorReportListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    $scope.SelectedItemModel.Duration = $scope.ReportFilterModel.Duration;

                    window.location = 'OperationalReports/OperatorHoursReportExcelDownload?SelectedItems=' + JSON.stringify($scope.ReportFilterModel);

                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.OperatorReportPrint = function (divName) {
            if ($scope.OperatorReportListModel.length > 0) {
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

        //Occupancy Report Start
        $scope.GetOccupancyReport = function () {
            var url = $("#GetOccupancyReport").val();

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'occupancyFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.OccupancyListModel = data;
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
        $scope.DownloadOccupancyPDFReport = function () {
            if ($scope.OccupancyListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    window.location = 'OperationalReports/OccupancyReportPDFDownload?SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadOccupancyExcelReport = function () {
            if ($scope.OccupancyListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    window.location = 'OperationalReports/OccupancyReportExcelDownload?SelectedItems=' + JSON.stringify($scope.ReportFilterModel);

                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.OccupancyReportPrint = function (divName) {
            if ($scope.OccupancyListModel.length > 0) {
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
        //Occupancy Report End

        //FOC Report Start
        GetActiveFOCReasons();
        function GetActiveFOCReasons() {
            var url = $("#GetActiveFOCReasonList").val();
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
                                $scope.FOCReasonListModel = data;
                                $scope.FOCReasonListModel.splice(0, 0, { 'ViolationReasonID': 'All', 'Reason': 'All' });
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
        $scope.GetFOCReport = function () {
            var url = $("#GetFOCReport").val();

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'focFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.FOCReportListModel = data;
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
        $scope.DownloadFOCPDFReport = function () {
            if ($scope.FOCReportListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    window.location = 'OperationalReports/FOCReportPDFDownload?SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadFOCExcelReport = function () {
            if ($scope.FOCReportListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    window.location = 'OperationalReports/FOCReportExcelDownload?SelectedItems=' + JSON.stringify($scope.ReportFilterModel);

                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.FOCReportPrint = function (divName) {
            if ($scope.FOCReportListModel.length > 0) {
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
        $scope.GetSupervisorName = function (supervisorID) {
            for (var i = 0; i < $scope.SupervisorListModel.length; i++) {
                if ($scope.SupervisorListModel[i].UserID == supervisorID) {
                    $scope.SelectedItemModel.SupervisorName = $scope.SupervisorListModel[i].UserName;
                    if ($scope.SelectedItemModel.SupervisorName == 'All') {
                        $scope.SelectedItemModel.SupervisorName = 'All Supervisors';
                    }                    
                }
            }
        };
        //FOC Report End

        //Allocations Code Start
        $scope.GetAllocations = function () {
            var url = $("#GetAllAllocations").val();

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'allocationsFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.AllocationsListModel = data;
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
        //Allocations Code End

        //Duplicates Code Start
        $scope.GetDuplicateEntriesReport = function () {
            var url = $("#GetDuplicateEntries").val();

            if (url != undefined) {
                $('#loader-container').show();
                if (CheckInSession()) {
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: "{'duplicateFilterData':" + JSON.stringify($scope.ReportFilterModel) + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            if (data != "Failed") {
                                $scope.DuplicateEntriesListModel = data;
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
        $scope.DownloadDuplicatesPDFReport = function () {
            if ($scope.DuplicateEntriesListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    window.location = 'OperationalReports/DuplicateRecordsPDFDownload?SelectedItems=' + JSON.stringify($scope.SelectedItemModel);
                }
                else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DownloadDuplicatesExcelReport = function () {
            if ($scope.DuplicateEntriesListModel.length > 0) {
                if (CheckInSession()) {
                    $scope.SelectedItemModel.FromDate = $scope.ReportFilterModel.FromDate;
                    $scope.SelectedItemModel.ToDate = $scope.ReportFilterModel.ToDate;
                    window.location = 'OperationalReports/DuplicateRecordsExcelDownload?SelectedItems=' + JSON.stringify($scope.ReportFilterModel);

                } else {
                    window.location.href = $("#LogOut").val();
                }
            }
        };
        $scope.DuplicatesReportPrint = function (divName) {
            if ($scope.DuplicateEntriesListModel.length > 0) {
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
        //Duplicates Code End

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
        //select fields  

        $scope.DurationListModel = [
            { 'name': 'Select', 'value': '0' },
            { 'name': 'Today', 'value': 'Today' },
            { 'name': 'Yesterday', 'value': 'Yesterday' },
            { 'name': 'Day Before Yesterday', 'value': 'Day Before Yesterday' },
            { 'name': 'Current Month', 'value': 'Current Month' },
            { 'name': 'Previous Month', 'value': 'Previous Month' }];
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
            return date ? moment(date).format('DD-MM-YYYY') : '';
        };
    });
})(); 
