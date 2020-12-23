(function () {
    'use strict';

    angular.module('app')
        .controller('DashboardCtrl', ['$scope', '$state', '$stateParams', '$mdDialog', DashboardCtrl]);

    function DashboardCtrl($scope, $state, $stateParams, $mdDialog) {

        ModelDefinations();
        function ModelDefinations() {
            $scope.ActiveSupervisorsList = [];
            $scope.StationOccupancyListModel = [];
            $scope.StationOccupancyDetailsListModel = [];
        }

        GetActiveSupervisorsList();
        GetStationOccupancyList();
        GetOccupancyDetailsByStation();

        function GetActiveSupervisorsList() {
            var url = $("#GetSupervisorsList").val();
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
                                $scope.ActiveSupervisorsList = data;
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
        function GetStationOccupancyList() {
            var url = $("#GetListofStationOccupancy").val();
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
                                $scope.StationOccupancyListModel = data;
                                for (var i = 0; i < $scope.StationOccupancyListModel.length; i++) {
                                    $scope.StationOccupancyListModel[i].Occupancy = parseFloat($scope.StationOccupancyListModel[i].Occupancy);
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

        function GetOccupancyDetailsByStation() {
            var url = $("#GetOccupancyDetailsByStation").val();
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
                                $scope.StationOccupancyDetailsListModel = data;                               
                            }
                            else {
                                $scope.StationOccupancyDetailsListModel = [];
                            }
                            $scope.$apply();
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

        // Traffic chart
        $scope.combo = {};
        $scope.combo.options = {
            legend: {
                show: true,
                x: 'right',
                y: 'top',
                data: ['WOM', 'Viral', 'Paid']
            },
            grid: {
                x: 40,
                y: 60,
                x2: 40,
                y2: 30,
                borderWidth: 0
            },
            tooltip: {
                show: true,
                trigger: 'axis',
                axisPointer: {
                    lineStyle: {
                        color: $scope.color.gray
                    }
                }
            },
            xAxis: [
                {
                    type: 'category',
                    axisLine: {
                        show: false
                    },
                    axisTick: {
                        show: false
                    },
                    axisLabel: {
                        textStyle: {
                            color: '#607685'
                        }
                    },
                    splitLine: {
                        show: false,
                        lineStyle: {
                            color: '#f3f3f3'
                        }
                    },
                    data: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20]
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    axisLine: {
                        show: false
                    },
                    axisTick: {
                        show: false
                    },
                    axisLabel: {
                        textStyle: {
                            color: '#607685'
                        }
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: '#f3f3f3'
                        }
                    }
                }
            ],
            series: [
                {
                    name: 'WOM',
                    type: 'bar',
                    clickable: false,
                    itemStyle: {
                        normal: {
                            color: $scope.color.gray
                        },
                        emphasis: {
                            color: 'rgba(237,240,241,.7)'
                        }
                    },
                    barCategoryGap: '50%',
                    data: [75, 62, 45, 60, 73, 50, 31, 56, 70, 63, 49, 72, 76, 67, 46, 51, 69, 59, 85, 67, 56],
                    legendHoverLink: false,
                    z: 2
                },
                {
                    name: 'Viral',
                    type: 'line',
                    smooth: true,
                    itemStyle: {
                        normal: {
                            color: $scope.color.success,
                            areaStyle: {
                                color: 'rgba(139,195,74,.7)',
                                type: 'default'
                            }
                        }
                    },
                    data: [0, 0, 0, 5, 20, 15, 30, 28, 25, 40, 60, 40, 43, 32, 36, 23, 12, 15, 2, 0, 0],
                    symbol: 'none',
                    legendHoverLink: false,
                    z: 3
                },
                {
                    name: 'Paid',
                    type: 'line',
                    smooth: true,
                    itemStyle: {
                        normal: {
                            color: $scope.color.info,
                            areaStyle: {
                                color: 'rgba(0,188,212,.7)',
                                type: 'default'
                            }
                        }
                    },
                    data: [0, 0, 0, 0, 1, 6, 15, 8, 16, 9, 25, 12, 50, 20, 25, 12, 2, 1, 0, 0, 0],
                    symbol: 'none',
                    legendHoverLink: false,
                    z: 4
                }
            ]
        };


        // 
        $scope.smline1 = {};
        $scope.smline2 = {};
        $scope.smline3 = {};
        $scope.smline4 = {};
        $scope.smline1.options = {
            tooltip: {
                show: false,
                trigger: 'axis',
                axisPointer: {
                    lineStyle: {
                        color: $scope.color.gray
                    }
                }
            },
            grid: {
                x: 1,
                y: 1,
                x2: 1,
                y2: 1,
                borderWidth: 0
            },
            xAxis: [
                {
                    type: 'category',
                    show: false,
                    boundaryGap: false,
                    data: [1, 2, 3, 4, 5, 6, 7]
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    show: false,
                    axisLabel: {
                        formatter: '{value} °C'
                    }
                }
            ],
            series: [
                {
                    name: '*',
                    type: 'line',
                    symbol: 'none',
                    data: [11, 11, 15, 13, 12, 13, 10],
                    itemStyle: {
                        normal: {
                            color: $scope.color.info
                        }
                    }
                }
            ]
        };
        $scope.smline2.options = {
            tooltip: {
                show: false,
                trigger: 'axis',
                axisPointer: {
                    lineStyle: {
                        color: $scope.color.gray
                    }
                }
            },
            grid: {
                x: 1,
                y: 1,
                x2: 1,
                y2: 1,
                borderWidth: 0
            },
            xAxis: [
                {
                    type: 'category',
                    show: false,
                    boundaryGap: false,
                    data: [1, 2, 3, 4, 5, 6, 7]
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    show: false,
                    axisLabel: {
                        formatter: '{value} °C'
                    }
                }
            ],
            series: [
                {
                    name: '*',
                    type: 'line',
                    symbol: 'none',
                    data: [11, 10, 14, 12, 13, 11, 12],
                    itemStyle: {
                        normal: {
                            color: $scope.color.success
                        }
                    }
                }
            ]
        };
        $scope.smline3.options = {
            tooltip: {
                show: false,
                trigger: 'axis',
                axisPointer: {
                    lineStyle: {
                        color: $scope.color.gray
                    }
                }
            },
            grid: {
                x: 1,
                y: 1,
                x2: 1,
                y2: 1,
                borderWidth: 0
            },
            xAxis: [
                {
                    type: 'category',
                    show: false,
                    boundaryGap: false,
                    data: [1, 2, 3, 4, 5, 6, 7]
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    show: false,
                    axisLabel: {
                        formatter: '{value} °C'
                    }
                }
            ],
            series: [
                {
                    name: '*',
                    type: 'line',
                    symbol: 'none',
                    data: [11, 10, 15, 13, 12, 13, 10],
                    itemStyle: {
                        normal: {
                            color: $scope.color.danger
                        }
                    }
                }
            ]
        };
        $scope.smline4.options = {
            tooltip: {
                show: false,
                trigger: 'axis',
                axisPointer: {
                    lineStyle: {
                        color: $scope.color.gray
                    }
                }
            },
            grid: {
                x: 1,
                y: 1,
                x2: 1,
                y2: 1,
                borderWidth: 0
            },
            xAxis: [
                {
                    type: 'category',
                    show: false,
                    boundaryGap: false,
                    data: [1, 2, 3, 4, 5, 6, 7]
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    show: false,
                    axisLabel: {
                        formatter: '{value} °C'
                    }
                }
            ],
            series: [
                {
                    name: '*',
                    type: 'line',
                    symbol: 'none',
                    data: [11, 12, 8, 10, 15, 12, 10],
                    itemStyle: {
                        normal: {
                            color: $scope.color.warning
                        }
                    }
                }
            ]
        };



        // Engagment pie charts
        var labelTop = {
            normal: {
                color: $scope.color.primary,
                label: {
                    show: true,
                    position: 'center',
                    formatter: '{b}',
                    textStyle: {
                        color: '#999',
                        baseline: 'top',
                        fontSize: 12
                    }
                },
                labelLine: {
                    show: false
                }
            }
        };
        var labelFromatter = {
            normal: {
                label: {
                    formatter: function (params) {
                        return 100 - params.value + '%'
                    },
                    textStyle: {
                        color: $scope.color.text,
                        baseline: 'bottom',
                        fontSize: 20
                    }
                }
            },
        }
        var labelBottom = {
            normal: {
                color: '#f1f1f1',
                label: {
                    show: true,
                    position: 'center'
                },
                labelLine: {
                    show: false
                }
            }
        };
        var radius = [55, 60];
        $scope.pie = {};
        $scope.pie.options = {
            series: [
                {
                    type: 'pie',
                    center: ['12.5%', '50%'],
                    radius: radius,
                    itemStyle: labelFromatter,
                    data: [
                        { name: 'Bounce', value: 36, itemStyle: labelTop },
                        { name: 'other', value: 64, itemStyle: labelBottom }
                    ]
                }, {
                    type: 'pie',
                    center: ['37.5%', '50%'],
                    radius: radius,
                    itemStyle: labelFromatter,
                    data: [
                        { name: 'Activation', value: 45, itemStyle: labelTop },
                        { name: 'other', value: 55, itemStyle: labelBottom }
                    ]
                }, {
                    type: 'pie',
                    center: ['62.5%', '50%'],
                    radius: radius,
                    itemStyle: labelFromatter,
                    data: [
                        { name: 'Retention', value: 25, itemStyle: labelTop },
                        { name: 'other', value: 75, itemStyle: labelBottom }
                    ]
                }, {
                    type: 'pie',
                    center: ['87.5%', '50%'],
                    radius: radius,
                    itemStyle: labelFromatter,
                    data: [
                        { name: 'Referral', value: 75, itemStyle: labelTop },
                        { name: 'other', value: 25, itemStyle: labelBottom }
                    ]
                }
            ]
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

        $scope.desserts = {
            "count": 9,
            "data": [
                {
                    "sid": "SUP-123",
                    "sname": "Sai Krishna",
                    "cstations": "MIYP, KPHB, JNTU, BNAR, EGDA",
                    "sphone": "+91 8143143143",
                    "saddress": "Road-2, Kukatpally"
                }, {
                    "sid": "SUP-223",
                    "sname": "Rama Krishna",
                    "cstations": "DSNR, LBNR, MPET, ABID, NMPL",
                    "sphone": "+91 8143143122",
                    "saddress": "Road-1, Ameerpet"
                }, {
                    "sid": "SUP-323",
                    "sname": "Sasi Sai",
                    "cstations": "MIYP, KPHB, JNTU, BNAR, EGDA",
                    "sphone": "+91 8143143143",
                    "saddress": "Road-2, Kukatpally"
                }, {
                    "sid": "SUP-423",
                    "sname": "Ravi Kishore",
                    "cstations": "DSNR, LBNR, MPET, ABID, NMPL",
                    "sphone": "+91 8143143111",
                    "saddress": "Road-2, SR Nagar"
                }, {
                    "sid": "SUP-523",
                    "sname": "Raghunath",
                    "cstations": "MIYP, KPHB, JNTU, BNAR, EGDA",
                    "sphone": "+91 8143143155",
                    "saddress": "Road-2, Kukatpally"
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
