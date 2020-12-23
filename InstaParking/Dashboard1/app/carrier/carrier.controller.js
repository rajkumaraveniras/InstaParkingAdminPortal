(function () {
    'use strict';

    angular.module('app')
        .controller('CarrierController', ['$scope', '$state', '$stateParams', CarrierController])

    function CarrierController($scope, $state, $stateParams) {
        
        ModelDefinations();
        function ModelDefinations() {
            $('.nav-left').hide();
            $scope.CarrierServiceRequestsListModel = [];
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
            var hdnFlagVal = $stateParams.requestid;

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
                                        options.url = "../Handlers/PickupDetailsHandler.ashx";
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
                            alert("Upload Carrier Signature.", {
                                "title": "",
                                "button": "Ok"
                            });
                        }
                    }
                    else {
                        alert("Upload Customer Signature.", {
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
            for (var i = 0, l = data.length; i < l; i++) {
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
