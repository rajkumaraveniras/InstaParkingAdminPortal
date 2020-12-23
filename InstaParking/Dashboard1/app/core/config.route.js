var absUrl = 'http://localhost:8405';
//var absUrl = getRootWebSitePath();
(function () {
    'use strict';

    angular.module('app')
        .config(['$stateProvider', '$urlRouterProvider', '$ocLazyLoadProvider',
                function($stateProvider, $urlRouterProvider, $ocLazyLoadProvider) {
                var routes, setRoutes;

                routes = [
                    'material-consumption/silver', 'material-consumption/copper', 'material-consumption/lead', 'material-consumption/gold', 'material-consumption/nitric-acid', 'material-consumption/cuples', 'material-consumption/others', 'material-consumption/dm-water', 'administration/customer-poc', 'administration/customer', 'administration/employee', 'administration/employee-type', 'administration/hallmark-center', 'administration/account',
                    'administration/create-customer-poc', 'administration/create-customer', 'administration/create-employee', 'administration/create-employee-type', 'administration/create-hallmark-center', 'administration/create-account', 'ui/cards', 'ui/typography', 'ui/buttons', 'ui/icons', 'ui/grids', 'ui/widgets', 'ui/components', 'ui/timeline', 'ui/lists', 'ui/pricing-tables',
                    'table/static', 'table/responsive', 'table/data',
                    'form/elements', 'form/layouts', 'form/validation',
                    'chart/echarts', 'chart/echarts-line', 'chart/echarts-bar', 'chart/echarts-pie', 'chart/echarts-scatter', 'chart/echarts-more',
                    'page/404', 'page/500', 'page/blank', 'page/forgot-password', 'page/invoice', 'page/lock-screen', 'page/profile', 'page/signin', 'page/signup',
                    'app/calendar'
                ]

                setRoutes = function(route) {
                    var config, url;
                    url = '/' + route;
                    config = {
                        url: url,
                        templateUrl: 'app/' + route + '.html'
                    };
                    $stateProvider.state(route, config);
                    return $stateProvider;
                };

                routes.forEach(function(route) {
                    return setRoutes(route);
                });


                $stateProvider
                    .state('dashboard', {
                        url: '/dashboard',
                        templateUrl: 'app/dashboard/dashboard.html'
                    })
                    .state('service-request', {
                        url: '/service-request',
                        templateUrl: absUrl +'/ServiceRequest/Index'
                    })
                    .state('create-service-request', {
                        url: '/create-service-request',
                        templateUrl: absUrl+'/ServiceRequest/CreateServiceRequest'
                        //templateUrl: 'app/service-request/create-service-request.html'
                    })
                    .state('verify-service-request', {
                        url: '/verify-service-request/:requestid',
                        templateUrl: absUrl + '/ServiceRequest/VerifyServiceRequest'
                        //templateUrl: 'app/service-request/create-service-request.html'
                    })
                    .state('carrier', {
                        url: '/carrier',
                        templateUrl: absUrl + '/Carrier/Index'
                    })
                    .state('edit-service-request-details', {
                        url: '/edit-service-request-details/:requestid',
                        templateUrl: absUrl + '/Carrier/EditServiceRequestDetails'
                        //templateUrl: 'app/service-request/create-service-request.html'
                    })

                    .state('form/wizard', {
                        url: '/form/wizard',
                        templateUrl: "app/form/wizard.html",
                        resolve: {
                            deps: ['$ocLazyLoad', function($ocLazyLoad) {
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
                            deps: ['$ocLazyLoad', function($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'googlemap',
                                ]);
                            }]
                        }
                    });

                $urlRouterProvider
                    .when('/', '/dashboard')                    
                    .otherwise('/carrier');
            }
        ]);

    })(); 
function getRootWebSitePath() {
    var _location = document.location.toString();
    var applicationNameIndex = _location.indexOf('/', _location.indexOf('://') + 3);
    var applicationName = _location.substring(0, applicationNameIndex) + '/';
    var webFolderIndex = _location.indexOf('/', _location.indexOf(applicationName) + applicationName.length);
    var webFolderFullPath = _location.substring(0, webFolderIndex);

    return webFolderFullPath;
}