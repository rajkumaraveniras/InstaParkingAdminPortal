(function () {
    'use strict';

    angular.module('app')
        .controller('DashboardCtrl', ['$scope', DashboardCtrl])

    function DashboardCtrl($scope) {
        // success: #8BC34A 139,195,74
        // info: #00BCD4 0,188,212
        // gray: #EDF0F1 237,240,241

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
                    type : 'category',
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
                    data : [0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20]
                }
            ],
            yAxis: [
                {
                    type : 'value',
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
                    name:'WOM',
                    type:'bar',
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
                    data:[75,62,45,60,73,50,31,56,70,63,49,72,76,67,46,51,69,59,85,67,56],
                    legendHoverLink: false,
                    z: 2
                },
                {
                    name:'Viral',
                    type:'line',
                    smooth:true,
                    itemStyle: {
                        normal: {
                            color: $scope.color.success,
                            areaStyle: {
                                color: 'rgba(139,195,74,.7)',
                                type: 'default'
                            }
                        }
                    },
                    data:[0,0,0,5,20,15,30,28,25,40,60,40,43,32,36,23,12,15,2,0,0],
                    symbol: 'none',
                    legendHoverLink: false,
                    z: 3
                },
                {
                    name:'Paid',
                    type:'line',
                    smooth:true,
                    itemStyle: {
                        normal: {
                            color: $scope.color.info,
                            areaStyle: {
                                color: 'rgba(0,188,212,.7)',
                                type: 'default'
                            }
                        }
                    },
                    data:[0,0,0,0,1,6,15,8,16,9,25,12,50,20,25,12,2,1,0,0,0],
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
            xAxis : [
                {
                    type : 'category',
                    show: false,
                    boundaryGap : false,
                    data : [1,2,3,4,5,6,7]
                }
            ],
            yAxis : [
                {
                    type : 'value',
                    show: false,
                    axisLabel : {
                        formatter: '{value} ??C'
                    }
                }
            ],
            series : [
                {
                    name:'*',
                    type:'line',
                    symbol: 'none',
                    data:[11, 11, 15, 13, 12, 13, 10],
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
            xAxis : [
                {
                    type : 'category',
                    show: false,
                    boundaryGap : false,
                    data : [1,2,3,4,5,6,7]
                }
            ],
            yAxis : [
                {
                    type : 'value',
                    show: false,
                    axisLabel : {
                        formatter: '{value} ??C'
                    }
                }
            ],
            series : [
                {
                    name:'*',
                    type:'line',
                    symbol: 'none',
                    data:[11, 10, 14, 12, 13, 11, 12],
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
            xAxis : [
                {
                    type : 'category',
                    show: false,
                    boundaryGap : false,
                    data : [1,2,3,4,5,6,7]
                }
            ],
            yAxis : [
                {
                    type : 'value',
                    show: false,
                    axisLabel : {
                        formatter: '{value} ??C'
                    }
                }
            ],
            series : [
                {
                    name:'*',
                    type:'line',
                    symbol: 'none',
                    data:[11, 10, 15, 13, 12, 13, 10],
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
            xAxis : [
                {
                    type : 'category',
                    show: false,
                    boundaryGap : false,
                    data : [1,2,3,4,5,6,7]
                }
            ],
            yAxis : [
                {
                    type : 'value',
                    show: false,
                    axisLabel : {
                        formatter: '{value} ??C'
                    }
                }
            ],
            series : [
                {
                    name:'*',
                    type:'line',
                    symbol: 'none',
                    data:[11, 12, 8, 10, 15, 12, 10],
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
            normal : {
                color: $scope.color.primary,
                label : {
                    show : true,
                    position : 'center',
                    formatter : '{b}',
                    textStyle: {
                        color: '#999',
                        baseline : 'top',
                        fontSize: 12
                    }
                },
                labelLine : {
                    show : false
                }
            }
        };
        var labelFromatter = {
            normal : {
                label : {
                    formatter : function (params){
                        return 100 - params.value + '%'
                    },
                    textStyle: {
                        color: $scope.color.text,
                        baseline : 'bottom',
                        fontSize: 20
                    }
                }
            },
        }
        var labelBottom = {
            normal : {
                color: '#f1f1f1',
                label : {
                    show : true,
                    position : 'center'
                },
                labelLine : {
                    show : false
                }
            }
        };        
        var radius = [55, 60];
        $scope.pie = {};
        $scope.pie.options = {
            series : [
                {
                    type : 'pie',
                    center : ['12.5%', '50%'],
                    radius : radius,
                    itemStyle : labelFromatter,
                    data : [
                        {name:'Bounce', value:36, itemStyle : labelTop},
                        {name:'other', value:64, itemStyle : labelBottom}
                    ]
                },{
                    type : 'pie',
                    center : ['37.5%', '50%'],
                    radius : radius,
                    itemStyle : labelFromatter,
                    data : [
                        {name:'Activation', value:45, itemStyle : labelTop},
                        {name:'other', value:55, itemStyle : labelBottom}
                    ]
                },{
                    type : 'pie',
                    center : ['62.5%', '50%'],
                    radius : radius,
                    itemStyle : labelFromatter,
                    data : [
                        {name:'Retention', value:25, itemStyle : labelTop},
                        {name:'other', value:75, itemStyle : labelBottom}
                    ]
                },{
                    type : 'pie',
                    center : ['87.5%', '50%'],
                    radius : radius,
                    itemStyle : labelFromatter,
                    data : [
                        {name:'Referral', value:75, itemStyle : labelTop},
                        {name:'other', value:25, itemStyle : labelBottom}
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
                "cname": "The Chennai Shopping Mall",
                "dbaname": "Chennai Jewellers",
                "ccode": { "value": 159258 },
                "phone": "+91 8143143143",
                "location": "Kukatpally"
            }, {
                "cname": "J.C Brothers",
                "dbaname": "J.C.B",
                "ccode": { "value": 237456 },
                "phone": "+91 9143143143",
                "location":"Ameerpet"
            }, {
                "cname": "Malabar Gold",
                "dbaname": "Malabar",
                "ccode": { "value":  262789 },
                "phone": "+91 8143143143",
                "location": "Kukatpally"
            }, {
                "cname": "Mangatrai Jewellers",
                "dbaname": "Mangatrai",
                "ccode": { "value":  305258 },
                "phone": "+91 9943143143",
                "location": "Panjagutta"
            }, {
                "cname": "Kalyan Jewellers",
                "dbaname": "Kalyan",
                "ccode": { "value":  375654 },
                "phone": "+91 8143143149",
                "location": "SR Nagar"
            }, {
                "cname": "Manepally Jewellers",
                "dbaname": "Manepally",
                "ccode": { "value": 392897 },
                "phone": "+91 7143143143",
                "location": "Dilsukhnagar"
            }, {
                "cname": "Khazana Jewellery",
                "dbaname": "Khazana",
                "ccode": { "value": 408756 },
                "phone": "+91 9143143143",
                "location": "KPHB Colony"
            }, {
                "cname": "Lalitha Jewellers",
                "dbaname": "Shree Jewellers",
                "ccode": { "value": 452123 },
                "phone": "+91 8143143443",
                "location": "Panjagutta"
            }, {
                "cname": "Tanishq Jewellers",
                "dbaname": "Tanishq",
                "ccode": { "value": 518789 },
                "phone": "+91 8143143143",
                "location": "Panjagutta"
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

  
})(); 
