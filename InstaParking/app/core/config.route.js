var absUrl = 'http://localhost:8405';
//var absUrl = getRootWebSitePath();
//alert(absUrl);
(function () {
    'use strict';

    angular.module('app')
        .config(['$stateProvider', '$urlRouterProvider', '$ocLazyLoadProvider',
            function ($stateProvider, $urlRouterProvider, $ocLazyLoadProvider) {
                var routes, setRoutes;

                routes = [
                    'material-consumption/silver', 'material-consumption/copper', 'material-consumption/lead', 'material-consumption/gold', 'material-consumption/nitric-acid', 'material-consumption/cuples', 'material-consumption/others', 'material-consumption/dm-water',  
                      'ui/cards', 'ui/typography', 'ui/buttons', 'ui/icons', 'ui/grids', 'ui/widgets', 'ui/components', 'ui/timeline', 'ui/lists', 'ui/pricing-tables',
                    'table/static', 'table/responsive', 'table/data',                   
                    'form/elements', 'form/layouts', 'form/validation',
                    'chart/echarts', 'chart/echarts-line', 'chart/echarts-bar', 'chart/echarts-pie', 'chart/echarts-scatter', 'chart/echarts-more',
                    'page/404', 'page/500', 'page/blank', 'page/forgot-password', 'page/invoice', 'page/lock-screen', 'page/signin', 'page/signup',
                    'app/calendar'
                ]

                setRoutes = function (route) {
                    var config, url;
                    url = '/' + route;
                    config = {
                        url: url,
                        templateUrl: 'app/' + route + '.html'
                    };
                    $stateProvider.state(route, config);
                    return $stateProvider;
                };

                routes.forEach(function (route) {
                    return setRoutes(route);
                });


                $stateProvider
                    .state('home', {
                        url: '/home',
                        templateUrl: absUrl + '/Home/Home'
                    })
                    .state('logout', {
                        url: '/logout',
                        templateUrl: absUrl + '/Account/LogOut'
                    })
                    .state('dashboard', {
                        url: '/dashboard',
                        templateUrl: absUrl + '/Dashboard/Index'
                    })
                    .state('parking/zones', {
                        url: '/parking/zones',
                        templateUrl: absUrl + '/Parking/Index'
                    })
                    .state('parking/create-zones', {
                        url: '/parking/create-zones',
                        templateUrl: absUrl + '/parking/CreateZone'
                    })
                    .state('parking/edit-zones', {
                        url: '/parking/edit-zones/:zoneid',
                        templateUrl: absUrl + '/parking/EditZone'
                    })
                    
                    .state('parking/stations', {
                        url: '/parking/stations',
                        templateUrl: absUrl + '/Parking/Stations'
                    })
                    .state('parking/create-stations', {
                        url: '/parking/create-stations',
                        templateUrl: absUrl + '/parking/CreateStation'
                    })
                    .state('parking/edit-stations', {
                        url: '/parking/edit-stations/:locationid',
                        templateUrl: absUrl + '/parking/EditStation'
                    })
                    
                    .state('parking/lots', {
                        url: '/parking/lots',
                        templateUrl: absUrl + '/Parking/Lots'
                    })
                    .state('parking/create-lots', {
                        url: '/parking/create-lots',
                        templateUrl: absUrl + '/parking/CreateLot'
                    })
                    .state('parking/edit-lots', {
                        url: '/parking/edit-lots/:lotid',
                        templateUrl: absUrl + '/parking/EditLot'
                    })

                    .state('parking/charges', {
                        url: '/parking/charges',
                        templateUrl: absUrl + '/parking/Charges'
                    })


                    .state('employee/roles', {
                        url: '/employee/roles',
                        templateUrl: absUrl + '/Employee/Index'
                    })
                    .state('employee/create-roles', {
                        url: '/employee/create-roles',
                        templateUrl: absUrl + '/employee/CreateRole'
                    })
                    .state('employee/edit-roles', {
                        url: '/employee/edit-roles/:roleid',
                        templateUrl: absUrl + '/employee/EditRole'
                    })

                    .state('employee/employees', {
                        url: '/employee/employees',
                        templateUrl: absUrl + '/Employee/Employees'
                    })
                    .state('employee/create-employees', {
                        url: '/employee/create-employees',
                        templateUrl: absUrl + '/employee/CreateEmployee'
                    })
                    .state('employee/edit-employees', {
                        url: '/employee/edit-employees/:employeeid',
                        templateUrl: absUrl + '/employee/EditEmployee'
                    })


                    .state('employee/assign', {
                        url: '/employee/assign',
                        templateUrl: absUrl + '/Employee/Assign'
                    })
                    .state('employee/create-assign', {
                        url: '/employee/create-assign',
                        templateUrl: absUrl + '/employee/CreateAssign'
                    })
                    .state('employee/edit-assign', {
                        url: '/employee/edit-assign/:assignid',
                        templateUrl: absUrl + '/employee/EditAssign'
                    })

                    .state('assign-operators', {
                        url: '/assign-operators',
                        templateUrl: absUrl + '/Supervisor/AssignOperators'
                    })

                    .state('passes', {
                        url: '/passes',
                        templateUrl: absUrl + '/PassType/Index'
                    })
                    .state('create-passes', {
                        url: '/create-passes',
                        templateUrl: absUrl + '/PassType/CreatePass'
                    })
                    .state('edit-passes', {
                        url: '/edit-passes/:passtypeid',
                        templateUrl: absUrl + '/PassType/EditPass'
                    })

                    .state('vehicles', {
                        url: '/vehicles',
                        templateUrl: absUrl + '/VehicleType/Index'
                    })
                    .state('create-vehicles', {
                        url: '/create-vehicles',
                        templateUrl: absUrl + '/VehicleType/CreateVehicleType'
                    })
                    .state('edit-vehicles', {
                        url: '/edit-vehicles/:vehicletypeid',
                        templateUrl: absUrl + '/VehicleType/EditVehicleType'
                    })

                    .state('features', {
                        url: '/features',
                        templateUrl: absUrl + '/Features/features'
                    })
              
                    .state('revenuereports/report-bystation', {
                        url: '/revenuereports/report-bystation',
                        templateUrl: absUrl + '/RevenueReports/ReportByStation'
                    })
                    .state('revenuereports/report-byvehicle', {
                        url: '/revenuereports/report-byvehicle',
                        templateUrl: absUrl + '/RevenueReports/ReportByVehicle'
                    })
                    .state('revenuereports/report-bypayment', {
                        url: '/revenuereports/report-bypayment',
                        templateUrl: absUrl + '/RevenueReports/ReportByPaymentType'
                    })
                    .state('revenuereports/report-bytime', {
                        url: '/revenuereports/report-bytime',
                        templateUrl: absUrl + '/RevenueReports/ReportByTime'
                    })
                    .state('revenuereports/report-bypases', {
                        url: '/revenuereports/report-bypases',
                        templateUrl: absUrl + '/RevenueReports/ReportByPasses'
                    })
                    .state('revenuereports/report-bychannel', {
                        url: '/revenuereports/report-bychannel',
                        templateUrl: absUrl + '/RevenueReports/ReportByChannel'
                    })
                    .state('revenuereports/report-bysupervisor', {
                        url: '/revenuereports/report-bysupervisor',
                        templateUrl: absUrl + '/RevenueReports/ReportBySupervisor'
                    })
                    .state('revenuereports/report-byviolation', {
                        url: '/revenuereports/report-byviolation',
                        templateUrl: absUrl + '/RevenueReports/ReportByViolation'
                    })
                    .state('operationalreports/report-byoperator', {
                        url: '/operationalreports/report-byoperator',
                        templateUrl: absUrl + '/OperationalReports/ReportByOperator'
                    })
                    .state('operationalreports/report-checkin', {
                        url: '/operationalreports/report-checkin',
                        templateUrl: absUrl + '/OperationalReports/CheckInReport'
                    })
                    .state('operationalreports/report-occupancy', {
                        url: '/operationalreports/report-occupancy',
                        templateUrl: absUrl + '/OperationalReports/OccupancyReport'
                    })
                    .state('operationalreports/disposition-report', {
                        url: '/operationalreports/disposition-report',
                        templateUrl: absUrl + '/OperationalReports/DispositionReport'
                    })
                    .state('operationalreports/FOCreport', {
                        url: '/operationalreports/FOCreport',
                        templateUrl: absUrl + '/OperationalReports/FOCReport'
                    })
                    .state('operationalreports/Allocations', {
                        url: '/operationalreports/Allocations',
                        templateUrl: absUrl + '/OperationalReports/Allocations'
                    })
                    .state('operationalreports/DuplicateEntries', {
                        url: '/operationalreports/DuplicateEntries',
                        templateUrl: absUrl + '/OperationalReports/DuplicateEntries'
                    })
                    .state('companyinfo', {
                        url: '/companyinfo',
                        templateUrl: absUrl + '/CompanyInfo/Index'
                    })
                    .state('offered-spaces', {
                        url: '/offered-spaces',
                        templateUrl: absUrl + '/OfferSpace/OfferSpaces'
                    })
                    .state('support', {
                        url: '/support',
                        templateUrl: absUrl + '/SupportRequest/SupportRequests'
                    })

                    .state('station-occupancy', {
                        url: '/station-occupancy/:locationid',
                        templateUrl: absUrl + '/Dashboard/StationOccupancy'
                    })

                    .state('editprofile', {
                        url: '/editprofile',
                        templateUrl: absUrl + '/Home/EditProfile'
                    })

                    //.state('assign-operators', {
                    //    url: '/assign-operators',
                    //    templateUrl: absUrl + '/Supervisor/AssignOperators'
                    //})

                    .state('form/wizard', {
                        url: '/form/wizard',
                        templateUrl: "app/form/wizard.html",
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'angular-wizard'
                                ]);
                            }]
                        }
                    })
                    .state('map/maps', {
                        url: '/map/maps',
                        templateUrl: "app/map/maps.html",
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'googlemap',
                                ]);
                            }]
                        }
                    });
                //alert($urlRouterProvider.absUrl)
                $urlRouterProvider.otherwise('/home')
                // .when('/Dashboard/', '/carrier')
            }
        ]).constant('FIREBASE_URL', '/home');;

    })(); 
function getRootWebSitePath() {
    var _location = document.location.toString();
    var applicationNameIndex = _location.indexOf('/', _location.indexOf('://') + 3);
    var applicationName = _location.substring(0, applicationNameIndex) + '/';
    var webFolderIndex = _location.indexOf('/', _location.indexOf(applicationName) + applicationName.length);
    var webFolderFullPath = _location.substring(0, webFolderIndex);
    
    return webFolderFullPath;
}