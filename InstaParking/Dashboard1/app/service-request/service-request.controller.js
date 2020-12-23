(function () {
    'use strict';

    angular.module('app')
        .controller('ServicerequestCtrl', ['$scope','$state','$stateParams', ServicerequestCtrl])

    function ServicerequestCtrl($scope, $state, $stateParams) {
        
        ModelDefinations();
        function ModelDefinations() {
            $scope.ServiceRequestListModel = [];
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
            $scope.query = {
                order: 'UpdatedDate',
                limit: 5,
                page: 1
            };

            $scope.filterTable = '';
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
                            //for (var items = 0; items < $scope.ServiceRequestListModel.length; items++) {
                            //    $scope.ServiceRequestListModel[items].UpdatedDate = GetLastModifiedDate($scope.ServiceRequestListModel[items].UpdatedDate);
                            //}
                            //$scope.sortpropertyName = 'CustomerName';
                            //$scope.sortorder = false;
                            //$scope.$apply();
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
                            alert("Service Request Created Successfully");
                            // window.location = $("#Index").val();    
                            $state.go("service-request")
                        }
                        else {
                            alert(data);
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
                alert('Please select Customer.');
                flag = false;
                return flag;
            }
            if (customerPOC == '' || customerPOC == null) {
                alert('Please select Customer POC.');
                flag = false;
                return flag;
            }
            if (carrier == '' || carrier == null) {
                alert('Please select Carrier.');
                flag = false;
                return flag;
            }

            return flag;
        }

        VerifyServiceRequest();
        GetStackingDependencyDetails();

        function VerifyServiceRequest() {
            var hdnFlagVal = $stateParams.requestid;
            var url = $("#ViewServiceRequestDetails").val();
           // var hdnFlagVal = angular.element("#hdnRequestid").val();

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
                            alert("Bin Created Successfully");
                            window.location = $("#Index").val();
                        }
                        else {
                            alert(data);
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
        
        function CheckInSession() {
            var url = $("#CheckSessionValue").val();
            var flagVal = false;
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
                    }
                }, error: function (xhr) {
                    flagVal = false;
                }
            });
            return flagVal;
        }


        }
 
})(); 
