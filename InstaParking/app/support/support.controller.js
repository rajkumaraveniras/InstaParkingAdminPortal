(function () {
    'use strict';

    angular.module('app')
        .controller('SupportCtrl', ['$scope', SupportCtrl]);

    function SupportCtrl($scope) {
        ModelDefinations();
        function ModelDefinations() {
            $scope.SupportRequestsListModel = [];
        }
        GetSupportRequestDetails();
        function GetSupportRequestDetails() {
            var url = $("#GetSupportRequestDetails").val();
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
                                $scope.SupportRequestsListModel = data;
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
    }
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
})(); 
