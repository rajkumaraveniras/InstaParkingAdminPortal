(function () {
    'use strict';

    angular.module('app')
        .controller('AdministrationCtrl', ['$scope', AdministrationCtrl])

    function AdministrationCtrl($scope) {
        
        
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
                    "cpoc": "Raaju",
                    "carname": "Venkat",
                    "status": "Open",
                    "madeon": "01/20/2020 3.44 PM"
                }, {
                    "cname": "J.C Brothers",
                    "cpoc": "Suprith",
                    "carname": "Ramarao",
                    "status": "Assayer Assigned",
                    "madeon":"01/21/2020 1.20 PM"
                }, {
                    "cname": "Malabar Gold",
                    "cpoc": "Sagar",
                    "carname": "Krishna",
                    "status": "Closed",
                    "madeon": "01/21/2020 4.40 PM"
                }
            ]
        };

        $scope.accountlist = {
            "count": 2,
            "data": [
                {
                    "aname": "The Chennai Shopping Mall",
                    "description": "Account Description",
                    "status": "Active",
                    "modified": "01/20/2020 3.44 PM"
                }, {
                    "aname": "J.C Brothers",
                    "description": "Account Description",
                    "status": "Active",
                    "modified":"01/21/2020 1.20 PM"
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

        //create account start
        $scope.account = {
            name: '',
            description: ''
        };
        //create account close

        //select fields        
        $scope.customerddl = {};
        $scope.customerddl.customername = '';
        $scope.customerddl.customer = [
        {'abbrev':'RS Brothers'},
        {'abbrev':'Chennai Shopping Mall'},
        {'abbrev':'Lalitha Jewellery'}]

        $scope.customerpocddl = {};
        $scope.customerpocddl.customerpocname = '';
        $scope.customerpocddl.customerpoc = [
        {'abbrev':'Umakanth'},
        {'abbrev':'Sagar'},
        {'abbrev':'Ramarao'}] 
        
        $scope.carrierddl = {};
        $scope.carrierddl.carriercname = '';
        $scope.carrierddl.carrier = [
        {'abbrev':'Krishna'},
        {'abbrev':'Venkat'},
        {'abbrev':'Suresh'}]


        
        }

                 
     
})(); 
