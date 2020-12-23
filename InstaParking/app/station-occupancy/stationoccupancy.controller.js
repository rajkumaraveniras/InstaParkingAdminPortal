(function () {
    'use strict';

    angular.module('app')
        .controller('StationooccupancyCtrl', ['$scope', '$state', '$stateParams', StationooccupancyCtrl]);

    function StationooccupancyCtrl($scope, $state, $stateParams) {

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
